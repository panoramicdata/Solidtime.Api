namespace Solidtime.Api.Models;

/// <summary>
/// Represents an API token (Personal Access Token) for authenticating with the Solidtime API
/// </summary>
public class ApiToken : SolidtimeEntityBase
{
	/// <summary>
	/// The name/description of the API token
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

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
}
