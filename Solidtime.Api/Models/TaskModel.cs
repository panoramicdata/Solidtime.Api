using System;
using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Represents a task in the Solidtime system
/// Note: Named TaskModel to avoid conflict with System.Threading.Tasks.Task
/// </summary>
public class TaskModel
{
	/// <summary>
	/// The unique identifier for the task
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; set; }

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

	/// <summary>
	/// The date and time when the task was created
	/// </summary>
	[JsonPropertyName("created_at")]
	public required DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// The date and time when the task was last updated
	/// </summary>
	[JsonPropertyName("updated_at")]
	public required DateTimeOffset UpdatedAt { get; set; }
}
