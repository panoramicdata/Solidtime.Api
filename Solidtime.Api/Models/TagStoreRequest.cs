using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Request object for creating a new tag
/// </summary>
public class TagStoreRequest
{
	/// <summary>
	/// The tag's name
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }
}
