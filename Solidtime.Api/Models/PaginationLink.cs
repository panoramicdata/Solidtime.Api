namespace Solidtime.Api.Models;

/// <summary>
/// Represents a pagination link with label
/// </summary>
public class PaginationLink
{
	/// <summary>
	/// The URL for this page
	/// </summary>
	[JsonPropertyName("url")]
	public required string? Url { get; set; }

	/// <summary>
	/// The label for this page link
	/// </summary>
	[JsonPropertyName("label")]
	public required string Label { get; set; }

	/// <summary>
	/// Gets or sets whether this page is the active/current page
	/// </summary>
	[JsonPropertyName("active")]
	public required bool Active { get; set; }
}
