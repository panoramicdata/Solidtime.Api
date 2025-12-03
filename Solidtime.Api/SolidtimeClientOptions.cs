using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Solidtime.Api;

/// <summary>
/// Options for configuring the Solidtime API client
/// </summary>
public class SolidtimeClientOptions
{
	/// <summary>
	/// The API token (Personal Access Token) for authentication
	/// </summary>
	public required string ApiToken { get; set; }

	/// <summary>
	/// The base URL for the Solidtime API
	/// Default: https://app.solidtime.io/api
	/// </summary>
	public string BaseUrl { get; set; } = "https://app.solidtime.io/api";

	/// <summary>
	/// The request timeout in seconds
	/// Default: 30 seconds
	/// </summary>
	public int TimeoutSeconds { get; set; } = 30;

	/// <summary>
	/// The logger for diagnostic information
	/// </summary>
	public ILogger Logger { get; set; } = NullLogger.Instance;

	/// <summary>
	/// The behavior for unmapped JSON members
	/// Default: Skip (for backward compatibility)
	/// Set to Disallow during development/testing to catch API changes
	/// </summary>
	public JsonUnmappedMemberHandling JsonUnmappedMemberHandling { get; set; } = JsonUnmappedMemberHandling.Skip;

	/// <summary>
	/// Validates the options
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when required options are missing or invalid</exception>
	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(ApiToken))
		{
			throw new InvalidOperationException("ApiToken is required");
		}

		if (string.IsNullOrWhiteSpace(BaseUrl))
		{
			throw new InvalidOperationException("BaseUrl is required");
		}

		if (TimeoutSeconds <= 0)
		{
			throw new InvalidOperationException("TimeoutSeconds must be greater than 0");
		}

		// Validate BaseUrl format
		if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri))
		{
			throw new InvalidOperationException($"BaseUrl '{BaseUrl}' is not a valid URI");
		}

		if (uri.Scheme != "https" && uri.Scheme != "http")
		{
			throw new InvalidOperationException("BaseUrl must use http or https scheme");
		}
	}
}
