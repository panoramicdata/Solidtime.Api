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
	public PaginationLinks? Links { get; set; }

	/// <summary>
	/// Gets or sets pagination metadata
	/// </summary>
	[JsonPropertyName("meta")]
	public PaginationMeta? Meta { get; set; }
}
