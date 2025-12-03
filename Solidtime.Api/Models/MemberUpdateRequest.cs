namespace Solidtime.Api.Models;

/// <summary>
/// Request object for updating an organization member
/// </summary>
public class MemberUpdateRequest
{
	/// <summary>
	/// The member's role in the organization
	/// </summary>
	[JsonPropertyName("role")]
	public string? Role { get; set; }

	/// <summary>
	/// The billable rate for this member (in cents)
	/// </summary>
	[JsonPropertyName("billable_rate")]
	public int? BillableRate { get; set; }
}
