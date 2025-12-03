using System;
using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Response object returned when creating a new API token
/// Contains the actual token value which is only shown once
/// </summary>
public class ApiTokenCreated
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
	/// The actual token value (only shown once upon creation)
	/// </summary>
	[JsonPropertyName("token")]
	public required string Token { get; set; }

	/// <summary>
	/// The date and time when the token was created
	/// </summary>
	[JsonPropertyName("created_at")]
	public required DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// The expiration date and time for the token
	/// </summary>
	[JsonPropertyName("expires_at")]
	public DateTimeOffset? ExpiresAt { get; set; }
}
