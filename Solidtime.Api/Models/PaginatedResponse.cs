using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Wrapper for paginated API responses
/// </summary>
/// <typeparam name="T">The type of data in the collection</typeparam>
public class PaginatedResponse<T>
{
	/// <summary>
	/// The collection of data items
	/// </summary>
	[JsonPropertyName("data")]
	public required List<T> Data { get; set; }

	/// <summary>
	/// Gets or sets pagination links
	/// </summary>
	[JsonPropertyName("links")]
	public required PaginationLinks Links { get; set; }

	/// <summary>
	/// Gets or sets pagination metadata
	/// </summary>
	[JsonPropertyName("meta")]
	public required PaginationMeta Meta { get; set; }
}

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
	public int? From { get; set; }

	/// <summary>
	/// The last page number
	/// </summary>
	[JsonPropertyName("last_page")]
	public required int LastPage { get; set; }

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
	public int? To { get; set; }

	/// <summary>
	/// The total number of items across all pages
	/// </summary>
	[JsonPropertyName("total")]
	public required int Total { get; set; }
}
