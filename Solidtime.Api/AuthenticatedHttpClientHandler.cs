using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

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

			// Log the request
			if (_logger.IsEnabled(LogLevel.Debug))
			{
				LogRequestStart();
				if (attempt > 0)
				{
					LogRetryAttempt(attempt, MaxRetries);
				}

				LogRequestMethod(requestToSend.Method.ToString());
				LogRequestUri(requestToSend.RequestUri?.ToString() ?? string.Empty);

				if (requestToSend.Headers.Any())
				{
					LogRequestHeadersStart();
					foreach (var header in requestToSend.Headers)
					{
						// Mask the authorization token for security
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

				if (requestToSend.Content != null)
				{
					// Read content for logging but preserve it for sending
					var requestBody = await requestToSend.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
					LogRequestBody(requestBody);

					// Re-create the content so it can be sent
					requestToSend.Content = new StringContent(
						requestBody,
						requestToSend.Content.Headers.ContentType?.CharSet != null
							? System.Text.Encoding.GetEncoding(requestToSend.Content.Headers.ContentType.CharSet)
							: System.Text.Encoding.UTF8,
						requestToSend.Content.Headers.ContentType?.MediaType ?? "application/json");
				}

				LogRequestEnd();
			}

			var response = await base.SendAsync(requestToSend, cancellationToken).ConfigureAwait(false);

			// Log the response
			if (_logger.IsEnabled(LogLevel.Debug))
			{
				LogResponseStart();
				LogResponseStatus((int)response.StatusCode, response.ReasonPhrase ?? string.Empty);

				if (response.Headers.Any() || response.Content?.Headers.Count() > 0)
				{
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

				if (response.Content != null)
				{
					// Read the response content for logging
					var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

					// Log non-empty bodies
					if (!string.IsNullOrWhiteSpace(responseBody))
					{
						LogResponseBody(responseBody);
					}

					// Re-wrap the content so it can be read again by Refit
					response.Content = new StringContent(
						responseBody,
						response.Content.Headers.ContentType?.CharSet != null
							? System.Text.Encoding.GetEncoding(response.Content.Headers.ContentType.CharSet)
							: System.Text.Encoding.UTF8,
						response.Content.Headers.ContentType?.MediaType ?? "application/json");
				}

				LogResponseEnd();
			}

			// Check if we got rate limited (429 Too Many Requests)
			if (response.StatusCode == HttpStatusCode.TooManyRequests)
			{
				// Don't retry if we've exhausted attempts
				if (attempt >= MaxRetries)
				{
					LogMaxRetriesExceeded();
					return response;
				}

				// Calculate backoff delay
				var backoffDelay = GetBackoffDelay(response, attempt);

				LogRateLimitBackoff(backoffDelay.TotalSeconds);

				// Wait before retrying
				await Task.Delay(backoffDelay, cancellationToken).ConfigureAwait(false);

				// Dispose the failed response before retrying
				response.Dispose();
				continue;
			}

			// Success or non-retryable error - return the response
			return response;
		}

		// Should never reach here, but return a failure response just in case
		throw new InvalidOperationException("Retry loop completed without returning a response");
	}

	/// <summary>
	/// Calculates the backoff delay based on response headers and attempt number
	/// </summary>
	private static TimeSpan GetBackoffDelay(HttpResponseMessage response, int attempt)
	{
		// Try to get Retry-After header (can be in seconds or HTTP date)
		if (response.Headers.RetryAfter != null)
		{
			if (response.Headers.RetryAfter.Delta.HasValue)
			{
				// Retry-After specified as seconds
				return response.Headers.RetryAfter.Delta.Value;
			}
			else if (response.Headers.RetryAfter.Date.HasValue)
			{
				// Retry-After specified as HTTP date
				var delay = response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
				if (delay.TotalSeconds > 0)
				{
					return delay;
				}
			}
		}

		// Check for X-RateLimit-Reset header (Unix timestamp)
		if (response.Headers.TryGetValues("X-RateLimit-Reset", out var resetValues))
		{
			var resetValue = resetValues.FirstOrDefault();
			if (!string.IsNullOrEmpty(resetValue) && long.TryParse(resetValue, out var resetTimestamp))
			{
				var resetTime = DateTimeOffset.FromUnixTimeSeconds(resetTimestamp);
				var delay = resetTime - DateTimeOffset.UtcNow;
				if (delay.TotalSeconds > 0)
				{
					return delay;
				}
			}
		}

		// Fallback to exponential backoff: 1s, 2s, 4s
		var exponentialDelay = InitialBackoffMs * Math.Pow(2, attempt);
		return TimeSpan.FromMilliseconds(exponentialDelay);
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
