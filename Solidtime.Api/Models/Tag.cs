namespace Solidtime.Api.Models;

/// <summary>
/// Represents a tag in the Solidtime system
/// </summary>
public class Tag : SolidtimeEntityBase
{
	/// <summary>
	/// The tag's name
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The organization ID this tag belongs to (not always returned by the API)
	/// </summary>
	[JsonPropertyName("organization_id")]
	public string? OrganizationId { get; set; }
}
