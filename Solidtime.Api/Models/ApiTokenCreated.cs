namespace Solidtime.Api.Models;

/// <summary>
/// Response object returned when creating a new API token
/// Contains the actual token value which is only shown once
/// </summary>
public class ApiTokenCreated : SolidtimeEntityBase
{
	/// <summary>
	/// The name/description of the API token
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The actual token value (only shown once upon creation)
	/// </summary>
	[JsonPropertyName("token")]
	public string? Token { get; set; }

	/// <summary>
	/// The expiration date and time for the token
	/// </summary>
	[JsonPropertyName("expires_at")]
	public DateTimeOffset? ExpiresAt { get; set; }

	/// <summary>
	/// The ID of the entity that owns this token (typically the user ID)
	/// </summary>
	[JsonPropertyName("tokenable_id")]
	public string? TokenableId { get; set; }

	/// <summary>
	/// The type of entity that owns this token (e.g., "App\\Models\\User")
	/// </summary>
	[JsonPropertyName("tokenable_type")]
	public string? TokenableType { get; set; }

	/// <summary>
	/// The abilities/permissions granted to this token
	/// </summary>
	[JsonPropertyName("abilities")]
	public List<string>? Abilities { get; set; }

	/// <summary>
	/// The plain text value of the token (only available immediately after creation)
	/// </summary>
	[JsonPropertyName("plain_text_token")]
	public string? PlainTextToken { get; set; }
}
