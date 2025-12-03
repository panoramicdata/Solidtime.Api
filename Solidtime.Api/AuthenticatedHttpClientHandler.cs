using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Solidtime.Api;

/// <summary>
/// HTTP client handler that adds Bearer token authentication to requests
/// </summary>
public partial class AuthenticatedHttpClientHandler : DelegatingHandler
{
	private readonly SolidtimeClientOptions _options;
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="AuthenticatedHttpClientHandler"/> class
	/// </summary>
	/// <param name="options">The client options containing the API token</param>
	public AuthenticatedHttpClientHandler(SolidtimeClientOptions options)
		: base(new HttpClientHandler())
	{
		ArgumentNullException.ThrowIfNull(options);
		_options = options;
		_logger = options.Logger;
	}

	/// <summary>
	/// Sends an HTTP request with Bearer token authentication
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

		// Log the request
		if (_logger.IsEnabled(LogLevel.Debug))
		{
			LogRequestStart();
			LogRequestMethod(request.Method.ToString());
			LogRequestUri(request.RequestUri?.ToString() ?? string.Empty);

			if (request.Headers.Any())
			{
				LogRequestHeadersStart();
				foreach (var header in request.Headers)
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

			if (request.Content != null)
			{
				// Read content for logging but preserve it for sending
				var requestBody = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
				LogRequestBody(requestBody);

				// Re-create the content so it can be sent
				request.Content = new StringContent(
					requestBody,
					request.Content.Headers.ContentType?.CharSet != null
						? System.Text.Encoding.GetEncoding(request.Content.Headers.ContentType.CharSet)
						: System.Text.Encoding.UTF8,
					request.Content.Headers.ContentType?.MediaType ?? "application/json");
			}

			LogRequestEnd();
		}

		var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

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

		return response;
	}

	// LoggerMessage delegates for high-performance logging
	[LoggerMessage(Level = LogLevel.Debug, Message = "┌─ HTTP Request ─────────────────────────────────")]
	private partial void LogRequestStart();

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
}
