namespace Solidtime.Api.Models;

/// <summary>
/// Report properties defining what data to include in the report
/// </summary>
public class ReportProperties
{
	/// <summary>
	/// Start date for the report (ISO 8601 format)
	/// </summary>
	[JsonPropertyName("start")]
	public required string Start { get; set; }

	/// <summary>
	/// End date for the report (ISO 8601 format)
	/// </summary>
	[JsonPropertyName("end")]
	public required string End { get; set; }

	/// <summary>
	/// Primary grouping for the report (e.g., "project", "task", "client", "tag")
	/// </summary>
	[JsonPropertyName("group")]
	public required string Group { get; set; }

	/// <summary>
	/// Secondary grouping for the report (e.g., "project", "task", "client", "tag", "none")
	/// </summary>
	[JsonPropertyName("sub_group")]
	public required string SubGroup { get; set; }

	/// <summary>
	/// Historical grouping for the report (e.g., "day", "week", "month", "year", "none")
	/// </summary>
	[JsonPropertyName("history_group")]
	public required string HistoryGroup { get; set; }
}
