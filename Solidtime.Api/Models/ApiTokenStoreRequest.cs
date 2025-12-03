using System;
using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Request object for creating a new API token
/// </summary>
public class ApiTokenStoreRequest
{
	/// <summary>
	/// The name/description for the new API token
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The expiration date and time for the token (optional)
	/// </summary>
	[JsonPropertyName("expires_at")]
	public DateTimeOffset? ExpiresAt { get; set; }
}
