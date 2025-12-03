using System;
using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Represents an API token (Personal Access Token) for authenticating with the Solidtime API
/// </summary>
public class ApiToken
{
	/// <summary>
	/// The unique identifier for the API token
	/// </summary>
	[JsonPropertyName("id")]
	public required int Id { get; set; }

	/// <summary>
	/// The name/description of the API token
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The date and time when the token was created
	/// </summary>
	[JsonPropertyName("created_at")]
	public required DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// The date and time when the token was last used
	/// </summary>
	[JsonPropertyName("last_used_at")]
	public DateTimeOffset? LastUsedAt { get; set; }

	/// <summary>
	/// The expiration date and time for the token
	/// </summary>
	[JsonPropertyName("expires_at")]
	public DateTimeOffset? ExpiresAt { get; set; }
}
