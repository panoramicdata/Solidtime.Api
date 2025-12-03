namespace Solidtime.Api.Models;

/// <summary>
/// Request object for adding a member to a project
/// </summary>
public class ProjectMemberStoreRequest
{
	/// <summary>
	/// The user ID of the member to add
	/// </summary>
	[JsonPropertyName("user_id")]
	public required string UserId { get; set; }

	/// <summary>
	/// The billable rate for this project member (in cents)
	/// </summary>
	[JsonPropertyName("billable_rate")]
	public int? BillableRate { get; set; }
}
