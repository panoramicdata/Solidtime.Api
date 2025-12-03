namespace Solidtime.Api.Models;

/// <summary>
/// Represents a task in the Solidtime system
/// Note: Named TaskModel to avoid conflict with System.Threading.Tasks.Task
/// </summary>
public class TaskModel : SolidtimeEntityBase
{
	/// <summary>
	/// The task's name
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The project ID this task belongs to
	/// </summary>
	[JsonPropertyName("project_id")]
	public required string ProjectId { get; set; }

	/// <summary>
	/// Gets or sets whether the task is done/completed
	/// </summary>
	[JsonPropertyName("is_done")]
	public required bool IsDone { get; set; }

	/// <summary>
	/// The organization ID this task belongs to
	/// </summary>
	[JsonPropertyName("organization_id")]
	public required string OrganizationId { get; set; }
}
