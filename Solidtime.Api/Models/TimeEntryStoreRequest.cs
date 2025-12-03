using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Request object for creating a new time entry
/// </summary>
public class TimeEntryStoreRequest
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
	/// The end date and time (optional for running timer)
	/// </summary>
	[JsonPropertyName("end")]
	public DateTimeOffset? End { get; set; }

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
