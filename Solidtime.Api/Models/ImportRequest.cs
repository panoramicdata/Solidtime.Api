namespace Solidtime.Api.Models;

/// <summary>
/// Request to import data from an external time tracking system
/// </summary>
public class ImportRequest
{
	/// <summary>
	/// The type of import (e.g., "toggl")
	/// </summary>
	[JsonPropertyName("type")]
	public required string Type { get; set; }

	/// <summary>
	/// The base64 encoded import data
	/// </summary>
	[JsonPropertyName("data")]
	public required string Data { get; set; }
}
