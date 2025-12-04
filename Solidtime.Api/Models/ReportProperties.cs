namespace Solidtime.Api.Models;

/// <summary>
/// Report properties defining what data to include in the report
/// </summary>
public class ReportProperties
{
	/// <summary>
	/// Start date for the report (ISO 8601 format: Y-m-d\TH:i:s\Z, e.g., "2024-01-01T00:00:00Z")
	/// </summary>
	[JsonPropertyName("start")]
	public required string Start { get; set; }

	/// <summary>
	/// End date for the report (ISO 8601 format: Y-m-d\TH:i:s\Z, e.g., "2024-12-31T23:59:59Z")
	/// </summary>
	[JsonPropertyName("end")]
	public required string End { get; set; }

	/// <summary>
	/// Primary grouping for the report.
	/// Valid values: "day", "week", "month", "year", "user", "project", "task", "client", "billable", "description", "tag"
	/// </summary>
	[JsonPropertyName("group")]
	public required string Group { get; set; }

	/// <summary>
	/// Secondary grouping for the report. Required field.
	/// Valid values: "day", "week", "month", "year", "user", "project", "task", "client", "billable", "description", "tag"
	/// </summary>
	[JsonPropertyName("sub_group")]
	public required string SubGroup { get; set; }

	/// <summary>
	/// Historical grouping for the report. Required field.
	/// Valid values: "day", "week", "month", "year"
	/// </summary>
	[JsonPropertyName("history_group")]
	public required string HistoryGroup { get; set; }

	/// <summary>
	/// Whether the report properties are active
	/// </summary>
	[JsonPropertyName("active")]
	public bool? Active { get; set; }

	/// <summary>
	/// List of member IDs to filter the report by
	/// </summary>
	[JsonPropertyName("member_ids")]
	public List<string>? MemberIds { get; set; }

	/// <summary>
	/// Filter by billable status
	/// </summary>
	[JsonPropertyName("billable")]
	public bool? Billable { get; set; }

	/// <summary>
	/// List of project IDs to filter the report by
	/// </summary>
	[JsonPropertyName("project_ids")]
	public List<string>? ProjectIds { get; set; }

	/// <summary>
	/// List of tag IDs to filter the report by
	/// </summary>
	[JsonPropertyName("tag_ids")]
	public List<string>? TagIds { get; set; }

	/// <summary>
	/// List of task IDs to filter the report by
	/// </summary>
	[JsonPropertyName("task_ids")]
	public List<string>? TaskIds { get; set; }

	/// <summary>
	/// List of client IDs to filter the report by
	/// </summary>
	[JsonPropertyName("client_ids")]
	public List<string>? ClientIds { get; set; }

	/// <summary>
	/// Starting day of the week
	/// </summary>
	[JsonPropertyName("week_start")]
	public string? WeekStart { get; set; }

	/// <summary>
	/// Timezone (e.g., "Europe/Berlin" or "America/New_York")
	/// </summary>
	[JsonPropertyName("timezone")]
	public string? Timezone { get; set; }

	/// <summary>
	/// Rounding type for time entries (up, down, nearest)
	/// </summary>
	[JsonPropertyName("rounding_type")]
	public string? RoundingType { get; set; }

	/// <summary>
	/// Rounding minutes for time entries
	/// </summary>
	[JsonPropertyName("rounding_minutes")]
	public int? RoundingMinutes { get; set; }
}
