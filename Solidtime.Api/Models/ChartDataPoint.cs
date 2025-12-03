namespace Solidtime.Api.Models;

/// <summary>
/// Represents a chart data point from the Solidtime charts API
/// </summary>
public class ChartDataPoint
{
	/// <summary>
	/// The value of this data point (e.g., total hours, amount)
	/// </summary>
	[JsonPropertyName("value")]
	public required decimal Value { get; set; }

	/// <summary>
	/// The ID of the related entity (e.g., project ID, task ID)
	/// May be null for aggregated data like "No project"
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	/// <summary>
	/// The name of this data point (e.g., project name, "No project")
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The color associated with this data point (hex color code)
	/// </summary>
	[JsonPropertyName("color")]
	public required string Color { get; set; }
}
