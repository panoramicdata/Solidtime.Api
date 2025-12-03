using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Request object for updating a task
/// </summary>
public class TaskUpdateRequest
{
	/// <summary>
	/// The task's name
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	/// <summary>
	/// The project ID this task belongs to
	/// </summary>
	[JsonPropertyName("project_id")]
	public string? ProjectId { get; set; }

	/// <summary>
	/// Gets or sets whether the task is done/completed
	/// </summary>
	[JsonPropertyName("is_done")]
	public bool? IsDone { get; set; }
}
