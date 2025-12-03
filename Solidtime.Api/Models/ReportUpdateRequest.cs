namespace Solidtime.Api.Models;

/// <summary>
/// Request to update an existing report
/// </summary>
public class ReportUpdateRequest
{
	/// <summary>
	/// The report's name
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	/// <summary>
	/// The report's description
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	/// <summary>
	/// Whether the report is public
	/// </summary>
	[JsonPropertyName("is_public")]
	public bool? IsPublic { get; set; }

	/// <summary>
	/// Report properties defining what data to include
	/// </summary>
	[JsonPropertyName("properties")]
	public ReportProperties? Properties { get; set; }
}
