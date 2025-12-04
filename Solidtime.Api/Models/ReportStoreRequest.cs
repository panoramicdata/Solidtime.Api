namespace Solidtime.Api.Models;

/// <summary>
/// Request to create a new report
/// </summary>
public class ReportStoreRequest
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
	/// Report properties defining what data to include
	/// </summary>
	[JsonPropertyName("properties")]
	public required ReportProperties Properties { get; set; }

	/// <summary>
	/// After this date the report will be automatically set to private
	/// (Format: "Y-m-d\TH:i:s\Z", UTC timezone, Example: "2000-02-22T14:58:59Z")
	/// </summary>
	[JsonPropertyName("public_until")]
	public string? PublicUntil { get; set; }
}
