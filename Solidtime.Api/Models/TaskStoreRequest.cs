using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Request object for creating a new task
/// </summary>
public class TaskStoreRequest
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
}
