namespace Solidtime.Api.Models;

/// <summary>
/// Represents a time entry in the Solidtime system
/// </summary>
public class TimeEntry : SolidtimeEntityBase
{
	/// <summary>
	/// The description of the time entry
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	/// <summary>
	/// The start date and time
	/// </summary>
	[JsonPropertyName("start")]
	public required DateTimeOffset Start { get; set; }

	/// <summary>
	/// The end date and time (null if timer is running)
	/// </summary>
	[JsonPropertyName("end")]
	public DateTimeOffset? End { get; set; }

	/// <summary>
	/// The duration in seconds (null if timer is running)
	/// </summary>
	[JsonPropertyName("duration")]
	public int? Duration { get; set; }

	/// <summary>
	/// The project ID this time entry belongs to
	/// </summary>
	[JsonPropertyName("project_id")]
	public string? ProjectId { get; set; }

	/// <summary>
	/// The task ID this time entry belongs to
	/// </summary>
	[JsonPropertyName("task_id")]
	public string? TaskId { get; set; }

	/// <summary>
	/// The user ID who owns this time entry
	/// </summary>
	[JsonPropertyName("user_id")]
	public required string UserId { get; set; }

	/// <summary>
	/// The organization ID this time entry belongs to
	/// </summary>
	[JsonPropertyName("organization_id")]
	public required string OrganizationId { get; set; }

	/// <summary>
	/// Gets or sets whether the time entry is billable
	/// </summary>
	[JsonPropertyName("billable")]
	public required bool Billable { get; set; }

	/// <summary>
	/// The list of tag IDs associated with this time entry
	/// </summary>
	[JsonPropertyName("tags")]
	public List<string>? Tags { get; set; }
}
