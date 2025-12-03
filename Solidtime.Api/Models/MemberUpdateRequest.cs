using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Request object for updating an organization member
/// </summary>
public class MemberUpdateRequest
{
	/// <summary>
	/// Gets or sets the member's role in the organization
	/// </summary>
	[JsonPropertyName("role")]
	public string? Role { get; set; }

	/// <summary>
	/// Gets or sets the billable rate for this member (in cents)
	/// </summary>
	[JsonPropertyName("billable_rate")]
	public int? BillableRate { get; set; }
}
