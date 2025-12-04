namespace Solidtime.Api.Models;

/// <summary>
/// Represents a data point in the daily tracked hours chart
/// </summary>
public class DailyTrackedHoursDataPoint
{
	/// <summary>
	/// The date for this data point
	/// </summary>
	[JsonPropertyName("date")]
	public required string Date { get; set; }

	/// <summary>
	/// The duration in seconds for this date
	/// </summary>
	[JsonPropertyName("duration")]
	public required int Duration { get; set; }
}
