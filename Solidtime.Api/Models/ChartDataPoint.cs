namespace Solidtime.Api.Models;

/// <summary>
/// Represents a chart data point from the Solidtime charts API
/// Used by weekly-project-overview endpoint
/// </summary>
public class ChartDataPoint
{
	/// <summary>
	/// The value of this data point (duration in seconds)
	/// </summary>
	[JsonPropertyName("value")]
	public required int Value { get; set; }

	/// <summary>
	/// The ID of the related entity (e.g., project ID)
	/// Note: This field is not documented in OpenAPI but is returned by the API
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
