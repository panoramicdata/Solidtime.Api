namespace Solidtime.Api.Models;

/// <summary>
/// Represents an import operation in the Solidtime system
/// </summary>
public class Import : SolidtimeEntityBase
{
	/// <summary>
	/// The source of the import (e.g., "toggl")
	/// </summary>
	[JsonPropertyName("source")]
	public required string Source { get; set; }

	/// <summary>
	/// The status of the import operation
	/// </summary>
	[JsonPropertyName("status")]
	public required string Status { get; set; }

	/// <summary>
	/// The organization ID this import belongs to
	/// </summary>
	[JsonPropertyName("organization_id")]
	public required string OrganizationId { get; set; }

	/// <summary>
	/// The total number of items to import
	/// </summary>
	[JsonPropertyName("total_items")]
	public int? TotalItems { get; set; }

	/// <summary>
	/// The number of items already imported
	/// </summary>
	[JsonPropertyName("imported_items")]
	public int? ImportedItems { get; set; }

	/// <summary>
	/// Error message if the import failed
	/// </summary>
	[JsonPropertyName("error_message")]
	public string? ErrorMessage { get; set; }
}
