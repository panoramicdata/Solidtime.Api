namespace Solidtime.Api.Models;

/// <summary>
/// Represents a report in the Solidtime system
/// </summary>
public class Report : SolidtimeEntityBase
{
	/// <summary>
	/// The report's name
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The report's description
	/// </summary>
	[JsonPropertyName("description")]
	public required string? Description { get; set; }

	/// <summary>
	/// Whether the report is public
	/// </summary>
	[JsonPropertyName("is_public")]
	public required bool IsPublic { get; set; }

	/// <summary>
	/// Date until the report is public
	/// </summary>
	[JsonPropertyName("public_until")]
	public required string? PublicUntil { get; set; }

	/// <summary>
	/// Get link to access the report externally, not set if the report is private
	/// </summary>
	[JsonPropertyName("shareable_link")]
	public required string? ShareableLink { get; set; }

	/// <summary>
	/// Report properties defining what data to include
	/// </summary>
	[JsonPropertyName("properties")]
	public ReportProperties? Properties { get; set; }
}
