namespace Solidtime.Api.Models;

/// <summary>
/// Pagination links for navigating through paginated results
/// </summary>
public class PaginationLinks
{
	/// <summary>
	/// The URL for the first page
	/// </summary>
	[JsonPropertyName("first")]
	public string? First { get; set; }

	/// <summary>
	/// The URL for the last page
	/// </summary>
	[JsonPropertyName("last")]
	public string? Last { get; set; }

	/// <summary>
	/// The URL for the previous page
	/// </summary>
	[JsonPropertyName("prev")]
	public string? Prev { get; set; }

	/// <summary>
	/// The URL for the next page
	/// </summary>
	[JsonPropertyName("next")]
	public string? Next { get; set; }
}
