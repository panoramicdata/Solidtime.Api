using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Solidtime.Api;

/// <summary>
/// HTTP client handler that adds Bearer token authentication to requests and handles rate limiting (429) with backoff
/// </summary>
public partial class AuthenticatedBackingOffHttpClientHandler : DelegatingHandler
{
	private readonly SolidtimeClientOptions _options;
	private readonly ILogger _logger;
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

			await LogRequestAsync(requestToSend, attempt, cancellationToken).ConfigureAwait(false);

			var response = await base.SendAsync(requestToSend, cancellationToken).ConfigureAwait(false);

			await LogResponseAsync(response, cancellationToken).ConfigureAwait(false);

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
	/// Logs the HTTP request details if debug logging is enabled
	/// </summary>
	private async Task LogRequestAsync(HttpRequestMessage request, int attempt, CancellationToken cancellationToken)
	{
		if (!_logger.IsEnabled(LogLevel.Debug))
		{
			return;
		}

		LogRequestStart();

		if (attempt > 0)
		{
			LogRetryAttempt(attempt, MaxRetries);
		}

		LogRequestMethod(request.Method.ToString());
		LogRequestUri(request.RequestUri?.ToString() ?? string.Empty);

		LogRequestHeaders(request);

		if (request.Content != null)
		{
			await LogAndPreserveRequestContentAsync(request, cancellationToken).ConfigureAwait(false);
		}

		LogRequestEnd();
	}

	/// <summary>
	/// Logs request headers, masking sensitive values
	/// </summary>
	private void LogRequestHeaders(HttpRequestMessage request)
	{
		if (!request.Headers.Any())
		{
			return;
		}

		LogRequestHeadersStart();
		foreach (var header in request.Headers)
		{
			if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
			{
				LogRequestHeaderRedacted(header.Key);
			}
			else
			{
				LogRequestHeader(header.Key, string.Join(", ", header.Value));
			}
		}
	}

	/// <summary>
	/// Logs request content and preserves it for sending
	/// </summary>
	private async Task LogAndPreserveRequestContentAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var requestBody = await request.Content!.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
		LogRequestBody(requestBody);

		// Re-create the content so it can be sent
		var encoding = request.Content.Headers.ContentType?.CharSet != null
			? System.Text.Encoding.GetEncoding(request.Content.Headers.ContentType.CharSet)
			: System.Text.Encoding.UTF8;
		var mediaType = request.Content.Headers.ContentType?.MediaType ?? "application/json";

		request.Content = new StringContent(requestBody, encoding, mediaType);
	}

	/// <summary>
	/// Logs the HTTP response details if debug logging is enabled
	/// </summary>
	private async Task LogResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
	{
		if (!_logger.IsEnabled(LogLevel.Debug))
		{
			return;
		}

		LogResponseStart();
		LogResponseStatus((int)response.StatusCode, response.ReasonPhrase ?? string.Empty);

		LogResponseHeaders(response);

		if (response.Content != null)
		{
			await LogAndPreserveResponseContentAsync(response, cancellationToken).ConfigureAwait(false);
		}

		LogResponseEnd();
	}

	/// <summary>
	/// Logs response headers
	/// </summary>
	private void LogResponseHeaders(HttpResponseMessage response)
	{
		if (!response.Headers.Any() && response.Content?.Headers.Count() == 0)
		{
			return;
		}

		LogResponseHeadersStart();
		foreach (var header in response.Headers)
		{
			LogResponseHeader(header.Key, string.Join(", ", header.Value));
		}

		if (response.Content?.Headers != null)
		{
			foreach (var header in response.Content.Headers)
			{
				LogResponseHeader(header.Key, string.Join(", ", header.Value));
			}
		}
	}

	/// <summary>
	/// Logs response content and preserves it for reading by Refit
	/// </summary>
	private async Task LogAndPreserveResponseContentAsync(HttpResponseMessage response, CancellationToken cancellationToken)
	{
		var responseBody = await response.Content!.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

		if (!string.IsNullOrWhiteSpace(responseBody))
		{
			LogResponseBody(responseBody);
		}

		// Re-wrap the content so it can be read again by Refit
		var encoding = response.Content.Headers.ContentType?.CharSet != null
			? System.Text.Encoding.GetEncoding(response.Content.Headers.ContentType.CharSet)
			: System.Text.Encoding.UTF8;
		var mediaType = response.Content.Headers.ContentType?.MediaType ?? "application/json";

		response.Content = new StringContent(responseBody, encoding, mediaType);
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

	// LoggerMessage delegates for high-performance logging
	[LoggerMessage(Level = LogLevel.Debug, Message = "┌─ HTTP Request ─────────────────────────────────")]
	private partial void LogRequestStart();

	[LoggerMessage(Level = LogLevel.Debug, Message = "│ Retry attempt {attempt} of {maxRetries}")]
	private partial void LogRetryAttempt(int attempt, int maxRetries);

	[LoggerMessage(Level = LogLevel.Debug, Message = "│ Method: {method}")]
	private partial void LogRequestMethod(string method);

	[LoggerMessage(Level = LogLevel.Debug, Message = "│ URI: {uri}")]
	private partial void LogRequestUri(string uri);

	[LoggerMessage(Level = LogLevel.Debug, Message = "│ Headers:")]
	private partial void LogRequestHeadersStart();

	[LoggerMessage(Level = LogLevel.Debug, Message = "│   {headerName}: Bearer ***REDACTED***")]
	private partial void LogRequestHeaderRedacted(string headerName);

	[LoggerMessage(Level = LogLevel.Debug, Message = "│   {headerName}: {headerValue}")]
	private partial void LogRequestHeader(string headerName, string headerValue);

	[LoggerMessage(Level = LogLevel.Debug, Message = "│ Body: {requestBody}")]
	private partial void LogRequestBody(string requestBody);

	[LoggerMessage(Level = LogLevel.Debug, Message = "└────────────────────────────────────────────────")]
	private partial void LogRequestEnd();

	[LoggerMessage(Level = LogLevel.Debug, Message = "┌─ HTTP Response ────────────────────────────────")]
	private partial void LogResponseStart();

	[LoggerMessage(Level = LogLevel.Debug, Message = "│ Status: {statusCode} {reasonPhrase}")]
	private partial void LogResponseStatus(int statusCode, string reasonPhrase);

	[LoggerMessage(Level = LogLevel.Debug, Message = "│ Headers:")]
	private partial void LogResponseHeadersStart();

	[LoggerMessage(Level = LogLevel.Debug, Message = "│   {headerName}: {headerValue}")]
	private partial void LogResponseHeader(string headerName, string headerValue);

	[LoggerMessage(Level = LogLevel.Debug, Message = "│ Body: {responseBody}")]
	private partial void LogResponseBody(string responseBody);

	[LoggerMessage(Level = LogLevel.Debug, Message = "└────────────────────────────────────────────────")]
	private partial void LogResponseEnd();

	[LoggerMessage(Level = LogLevel.Warning, Message = "Rate limit exceeded (429), backing off for {seconds} seconds")]
	private partial void LogRateLimitBackoff(double seconds);

	[LoggerMessage(Level = LogLevel.Warning, Message = "Maximum retry attempts exceeded for rate limiting")]
	private partial void LogMaxRetriesExceeded();
}
