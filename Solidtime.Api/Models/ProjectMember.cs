namespace Solidtime.Api.Models;

/// <summary>
/// Represents a project member in the Solidtime system
/// </summary>
public class ProjectMember : SolidtimeEntityBase
{
	/// <summary>
	/// The user ID of the member
	/// </summary>
	[JsonPropertyName("user_id")]
	public required string UserId { get; set; }

	/// <summary>
	/// The project ID
	/// </summary>
	[JsonPropertyName("project_id")]
	public required string ProjectId { get; set; }

	/// <summary>
	/// The billable rate for this project member (in cents)
	/// </summary>
	[JsonPropertyName("billable_rate")]
	public int? BillableRate { get; set; }
}
