using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Solidtime.Api;

/// <summary>
/// HTTP client handler that adds Bearer token authentication to requests and handles rate limiting (429) with backoff
/// </summary>
#pragma warning disable S2333 // "partial" is required for LoggerMessage source generator
public partial class AuthenticatedBackingOffHttpClientHandler : DelegatingHandler
#pragma warning restore S2333
{
	private readonly SolidtimeClientOptions _options;
	private readonly ILogger _logger;
	private readonly HttpMessageLogger _messageLogger;
	private const int MaxRetries = 3;
	private const int InitialBackoffMs = 1000;

	/// <summary>
	/// Initializes a new instance of the <see cref="AuthenticatedBackingOffHttpClientHandler"/> class
	/// </summary>
	/// <param name="options">The client options containing the API token</param>
	public AuthenticatedBackingOffHttpClientHandler(SolidtimeClientOptions options)
		: base(new HttpClientHandler())
	{
		ArgumentNullException.ThrowIfNull(options);
		_options = options;
		_logger = options.Logger;
		_messageLogger = new HttpMessageLogger(_logger);
	}

	/// <summary>
	/// Sends an HTTP request with Bearer token authentication and handles rate limiting with backoff
	/// </summary>
	/// <param name="request">The HTTP request message</param>
	/// <param name="cancellationToken">A cancellation token</param>
	/// <returns>The HTTP response message</returns>
	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		// Add Bearer token authentication
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);

		// Retry loop for rate limiting
		for (var attempt = 0; attempt <= MaxRetries; attempt++)
		{
			// Clone the request for potential retries (except on first attempt)
			var requestToSend = attempt == 0 ? request : await CloneHttpRequestMessageAsync(request);

			await _messageLogger.LogRequestAsync(requestToSend, attempt, MaxRetries, cancellationToken).ConfigureAwait(false);

			var response = await base.SendAsync(requestToSend, cancellationToken).ConfigureAwait(false);

			await _messageLogger.LogResponseAsync(response, cancellationToken).ConfigureAwait(false);

			// Check if we got rate limited (429 Too Many Requests)
			if (response.StatusCode == HttpStatusCode.TooManyRequests)
			{
				if (await HandleRateLimitAsync(response, attempt, cancellationToken).ConfigureAwait(false))
				{
					continue; // Retry
				}

				return response; // Max retries exceeded
			}

			// Success or non-retryable error - return the response
			return response;
		}

		// Should never reach here, but return a failure response just in case
		throw new InvalidOperationException("Retry loop completed without returning a response");
	}

	/// <summary>
	/// Handles rate limit response by waiting and determining if retry should continue
	/// </summary>
	/// <returns>True if should retry, false if max retries exceeded</returns>
	private async Task<bool> HandleRateLimitAsync(HttpResponseMessage response, int attempt, CancellationToken cancellationToken)
	{
		// Don't retry if we've exhausted attempts
		if (attempt >= MaxRetries)
		{
			LogMaxRetriesExceeded();
			return false;
		}

		// Calculate backoff delay
		var backoffDelay = GetBackoffDelay(response, attempt);

		LogRateLimitBackoff(backoffDelay.TotalSeconds);

		// Wait before retrying
		await Task.Delay(backoffDelay, cancellationToken).ConfigureAwait(false);

		// Dispose the failed response before retrying
		response.Dispose();
		return true;
	}

	/// <summary>
	/// Calculates the backoff delay based on response headers and attempt number
	/// </summary>
	private static TimeSpan GetBackoffDelay(HttpResponseMessage response, int attempt)
	{
		// Try Retry-After header first
		var retryAfterDelay = GetRetryAfterDelay(response);
		if (retryAfterDelay.HasValue)
		{
			return retryAfterDelay.Value;
		}

		// Try X-RateLimit-Reset header
		var rateLimitDelay = GetRateLimitResetDelay(response);
		if (rateLimitDelay.HasValue)
		{
			return rateLimitDelay.Value;
		}

		// Fallback to exponential backoff: 1s, 2s, 4s
		var exponentialDelay = InitialBackoffMs * Math.Pow(2, attempt);
		return TimeSpan.FromMilliseconds(exponentialDelay);
	}

	/// <summary>
	/// Gets delay from Retry-After header if present
	/// </summary>
	private static TimeSpan? GetRetryAfterDelay(HttpResponseMessage response)
	{
		if (response.Headers.RetryAfter == null)
		{
			return null;
		}

		if (response.Headers.RetryAfter.Delta.HasValue)
		{
			return response.Headers.RetryAfter.Delta.Value;
		}

		if (response.Headers.RetryAfter.Date.HasValue)
		{
			var delay = response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
			if (delay.TotalSeconds > 0)
			{
				return delay;
			}
		}

		return null;
	}

	/// <summary>
	/// Gets delay from X-RateLimit-Reset header if present
	/// </summary>
	private static TimeSpan? GetRateLimitResetDelay(HttpResponseMessage response)
	{
		if (!response.Headers.TryGetValues("X-RateLimit-Reset", out var resetValues))
		{
			return null;
		}

		var resetValue = resetValues.FirstOrDefault();
		if (string.IsNullOrEmpty(resetValue) || !long.TryParse(resetValue, out var resetTimestamp))
		{
			return null;
		}

		var resetTime = DateTimeOffset.FromUnixTimeSeconds(resetTimestamp);
		var delay = resetTime - DateTimeOffset.UtcNow;

		return delay.TotalSeconds > 0 ? delay : null;
	}

	/// <summary>
	/// Clones an HTTP request message for retry attempts
	/// </summary>
	private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
	{
		var clone = new HttpRequestMessage(request.Method, request.RequestUri)
		{
			Version = request.Version
		};

		// Copy headers
		foreach (var header in request.Headers)
		{
			clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
		}

		// Copy content if present
		if (request.Content != null)
		{
			var contentBytes = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
			clone.Content = new ByteArrayContent(contentBytes);

			// Copy content headers
			foreach (var header in request.Content.Headers)
			{
				clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
			}
		}

		return clone;
	}

	[LoggerMessage(Level = LogLevel.Warning, Message = "Rate limit exceeded (429), backing off for {seconds} seconds")]
	private partial void LogRateLimitBackoff(double seconds);

	[LoggerMessage(Level = LogLevel.Warning, Message = "Maximum retry attempts exceeded for rate limiting")]
	private partial void LogMaxRetriesExceeded();
}
