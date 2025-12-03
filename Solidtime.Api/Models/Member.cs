namespace Solidtime.Api.Models;

/// <summary>
/// Represents an organization member in the Solidtime system
/// </summary>
public class Member
{
	/// <summary>
	/// The unique identifier for the member
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	/// <summary>
	/// The user ID of the member
	/// </summary>
	[JsonPropertyName("user_id")]
	public required string UserId { get; set; }

	/// <summary>
	/// The organization ID (not always returned by the API)
	/// </summary>
	[JsonPropertyName("organization_id")]
	public string? OrganizationId { get; set; }

	/// <summary>
	/// The member's name
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The member's email address
	/// </summary>
	[JsonPropertyName("email")]
	public required string Email { get; set; }

	/// <summary>
	/// The member's role in the organization
	/// </summary>
	[JsonPropertyName("role")]
	public required string Role { get; set; }

	/// <summary>
	/// Whether this member is a placeholder
	/// </summary>
	[JsonPropertyName("is_placeholder")]
	public required bool IsPlaceholder { get; set; }

	/// <summary>
	/// The billable rate for this member (in cents)
	/// </summary>
	[JsonPropertyName("billable_rate")]
	public int? BillableRate { get; set; }

	/// <summary>
	/// Captures any additional JSON properties not explicitly mapped.
	/// Useful for discovering new API fields and maintaining forward compatibility.
	/// </summary>
	[JsonExtensionData]
	public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
