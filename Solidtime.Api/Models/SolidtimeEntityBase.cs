namespace Solidtime.Api.Models;

/// <summary>
/// Base class for Solidtime entities that have Id, CreatedAt, and UpdatedAt properties
/// </summary>
public abstract class SolidtimeEntityBase
{
	/// <summary>
	/// The unique identifier for the entity (UUID string for most entities, SHA-256 hash for API tokens)
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	/// <summary>
	/// The date and time when the entity was created.
	/// May be null for endpoints that don't return this field (e.g., /v1/users/me).
	/// </summary>
	[JsonPropertyName("created_at")]
	public DateTimeOffset? CreatedAt { get; set; }

	/// <summary>
	/// The date and time when the entity was last updated.
	/// May be null for entities that don't track updates or endpoints that don't return this field.
	/// </summary>
	[JsonPropertyName("updated_at")]
	public DateTimeOffset? UpdatedAt { get; set; }

	/// <summary>
	/// Captures any additional JSON properties not explicitly mapped.
	/// Useful for discovering new API fields and maintaining forward compatibility.
	/// </summary>
	[JsonExtensionData]
	public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
