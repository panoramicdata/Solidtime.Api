namespace Solidtime.Api.Models;

/// <summary>
/// Represents a data point in the last seven days chart
/// </summary>
public class LastSevenDaysDataPoint
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

	/// <summary>
	/// History data as an array of integers
	/// </summary>
	[JsonPropertyName("history")]
	public required List<int> History { get; set; }
}
