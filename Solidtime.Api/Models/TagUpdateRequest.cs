namespace Solidtime.Api.Models;

/// <summary>
/// Request object for updating a tag
/// </summary>
public class TagUpdateRequest
{
	/// <summary>
	/// The name of the tag (1-255 characters)
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }
}
