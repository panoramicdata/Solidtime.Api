namespace Solidtime.Api.Models;

/// <summary>
/// Request object for updating a time entry
/// </summary>
public class TimeEntryUpdateRequest
{
	/// <summary>
	/// The member ID (user) for this time entry
	/// </summary>
	[JsonPropertyName("member_id")]
	public string? MemberId { get; set; }

	/// <summary>
	/// The description of the time entry
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	/// <summary>
	/// The start date and time (ISO 8601 format, UTC)
	/// </summary>
	[JsonPropertyName("start")]
	public string? Start { get; set; }

	/// <summary>
	/// The end date and time (ISO 8601 format, UTC)
	/// </summary>
	[JsonPropertyName("end")]
	public string? End { get; set; }

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
	/// Gets or sets whether the time entry is billable
	/// </summary>
	[JsonPropertyName("billable")]
	public bool? Billable { get; set; }

	/// <summary>
	/// The list of tag IDs to associate with this time entry
	/// </summary>
	[JsonPropertyName("tags")]
	public List<string>? Tags { get; set; }
}
