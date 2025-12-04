namespace Solidtime.Api.Models;

/// <summary>
/// Represents an available importer
/// </summary>
public class Importer
{
	/// <summary>
	/// The unique key/identifier of the importer
	/// </summary>
	[JsonPropertyName("key")]
	public required string Key { get; set; }

	/// <summary>
	/// The display name of the importer
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// A description of the importer
	/// </summary>
	[JsonPropertyName("description")]
	public required string Description { get; set; }
}
