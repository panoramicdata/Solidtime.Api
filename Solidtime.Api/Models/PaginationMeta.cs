namespace Solidtime.Api.Models;

/// <summary>
/// Metadata about paginated results
/// </summary>
public class PaginationMeta
{
	/// <summary>
	/// The current page number
	/// </summary>
	[JsonPropertyName("current_page")]
	public required int CurrentPage { get; set; }

	/// <summary>
	/// The index of the first item on the current page
	/// </summary>
	[JsonPropertyName("from")]
	public required int? From { get; set; }

	/// <summary>
	/// The last page number
	/// </summary>
	[JsonPropertyName("last_page")]
	public required int LastPage { get; set; }

	/// <summary>
	/// The pagination links with labels
	/// </summary>
	[JsonPropertyName("links")]
	public required List<PaginationLink> Links { get; set; }

	/// <summary>
	/// The path to the API endpoint
	/// </summary>
	[JsonPropertyName("path")]
	public required string Path { get; set; }

	/// <summary>
	/// The number of items per page
	/// </summary>
	[JsonPropertyName("per_page")]
	public required int PerPage { get; set; }

	/// <summary>
	/// The index of the last item on the current page
	/// </summary>
	[JsonPropertyName("to")]
	public required int? To { get; set; }

	/// <summary>
	/// The total number of items across all pages
	/// </summary>
	[JsonPropertyName("total")]
	public required int Total { get; set; }
}
