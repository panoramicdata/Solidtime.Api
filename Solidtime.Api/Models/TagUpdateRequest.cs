using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Request object for updating a tag
/// </summary>
public class TagUpdateRequest
{
	/// <summary>
	/// The tag's name
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }
}
