using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Solidtime.Api;

/// <summary>
/// Writes HTTP request and response detail to the debug log, re-creating message content after
/// reading it so that the request can still be sent and the response still deserialized.
/// This lives apart from <see cref="AuthenticatedBackingOffHttpClientHandler"/> so that the
/// handler deals only with authentication and rate-limit backoff.
/// </summary>
#pragma warning disable S2333 // "partial" is required for LoggerMessage source generator
internal sealed partial class HttpMessageLogger
#pragma warning restore S2333
{
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="HttpMessageLogger"/> class
	/// </summary>
	/// <param name="logger">The logger to write to</param>
	internal HttpMessageLogger(ILogger logger) => _logger = logger;

	/// <summary>
	/// Logs the HTTP request details if debug logging is enabled
	/// </summary>
	/// <param name="request">The request being sent</param>
	/// <param name="attempt">The zero-based attempt number</param>
	/// <param name="maxRetries">The maximum number of retries</param>
	/// <param name="cancellationToken">Cancellation token</param>
	internal async Task LogRequestAsync(
		HttpRequestMessage request,
		int attempt,
		int maxRetries,
		CancellationToken cancellationToken)
	{
		if (!_logger.IsEnabled(LogLevel.Debug))
		{
			return;
		}

		LogRequestStart();

		if (attempt > 0)
		{
			LogRetryAttempt(attempt, maxRetries);
		}

		_logger.LogDebug("│ Method: {Method}", request.Method);
		_logger.LogDebug("│ URI: {Uri}", request.RequestUri);

		LogRequestHeaders(request);

		if (request.Content != null)
		{
			await LogAndPreserveRequestContentAsync(request, cancellationToken).ConfigureAwait(false);
		}

		LogRequestEnd();
	}

	/// <summary>
	/// Logs the HTTP response details if debug logging is enabled
	/// </summary>
	/// <param name="response">The response that was received</param>
	/// <param name="cancellationToken">Cancellation token</param>
	internal async Task LogResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
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
				LogHeader(header.Key, header.Value);
			}
		}
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
			LogHeader(header.Key, header.Value);
		}

		if (response.Content?.Headers != null)
		{
			foreach (var header in response.Content.Headers)
			{
				LogHeader(header.Key, header.Value);
			}
		}
	}

	private void LogHeader(string name, IEnumerable<string> values)
	{
		if (!_logger.IsEnabled(LogLevel.Debug))
		{
			return;
		}

		_logger.LogDebug("│   {HeaderName}: {HeaderValue}", name, string.Join(", ", values));
	}

	/// <summary>
	/// Logs request content and preserves it for sending
	/// </summary>
	private async Task LogAndPreserveRequestContentAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var requestBody = await request.Content!.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
		LogRequestBody(requestBody);

		// Re-create the content so it can be sent
		request.Content = CloneAsStringContent(requestBody, request.Content);
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
		response.Content = CloneAsStringContent(responseBody, response.Content);
	}

	/// <summary>
	/// Rebuilds content that has already been read, keeping the original encoding and media type
	/// </summary>
	private static StringContent CloneAsStringContent(string body, HttpContent original)
	{
		var charSet = original.Headers.ContentType?.CharSet;
		var encoding = charSet != null ? Encoding.GetEncoding(charSet) : Encoding.UTF8;
		var mediaType = original.Headers.ContentType?.MediaType ?? "application/json";

		return new StringContent(body, encoding, mediaType);
	}

	// LoggerMessage delegates for high-performance logging
	[LoggerMessage(Level = LogLevel.Debug, Message = "┌─ HTTP Request ─────────────────────────────────")]
	private partial void LogRequestStart();

	[LoggerMessage(Level = LogLevel.Debug, Message = "│ Retry attempt {attempt} of {maxRetries}")]
	private partial void LogRetryAttempt(int attempt, int maxRetries);

	[LoggerMessage(Level = LogLevel.Debug, Message = "│ Headers:")]
	private partial void LogRequestHeadersStart();

	[LoggerMessage(Level = LogLevel.Debug, Message = "│   {headerName}: Bearer ***REDACTED***")]
	private partial void LogRequestHeaderRedacted(string headerName);

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

	[LoggerMessage(Level = LogLevel.Debug, Message = "│ Body: {responseBody}")]
	private partial void LogResponseBody(string responseBody);

	[LoggerMessage(Level = LogLevel.Debug, Message = "└────────────────────────────────────────────────")]
	private partial void LogResponseEnd();
}
