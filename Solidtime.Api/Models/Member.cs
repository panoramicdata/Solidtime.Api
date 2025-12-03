namespace Solidtime.Api.Models;

/// <summary>
/// Represents an organization member in the Solidtime system
/// </summary>
public class Member : SolidtimeEntityBase
{
	/// <summary>
	/// The user ID of the member
	/// </summary>
	[JsonPropertyName("user_id")]
	public required string UserId { get; set; }

	/// <summary>
	/// The organization ID
	/// </summary>
	[JsonPropertyName("organization_id")]
	public required string OrganizationId { get; set; }

	/// <summary>
	/// The member's role in the organization
	/// </summary>
	[JsonPropertyName("role")]
	public required string Role { get; set; }

	/// <summary>
	/// The billable rate for this member (in cents)
	/// </summary>
	[JsonPropertyName("billable_rate")]
	public int? BillableRate { get; set; }
}
