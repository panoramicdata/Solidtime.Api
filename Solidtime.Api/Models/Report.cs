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
	public string? Description { get; set; }

	/// <summary>
	/// Whether the report is public
	/// </summary>
	[JsonPropertyName("is_public")]
	public required bool IsPublic { get; set; }

	/// <summary>
	/// The organization ID this report belongs to
	/// </summary>
	[JsonPropertyName("organization_id")]
	public required string OrganizationId { get; set; }

	/// <summary>
	/// Report properties defining what data to include
	/// </summary>
	[JsonPropertyName("properties")]
	public ReportProperties? Properties { get; set; }
}
