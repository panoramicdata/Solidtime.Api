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
	/// The organization ID this tag belongs to
	/// </summary>
	[JsonPropertyName("organization_id")]
	public required string OrganizationId { get; set; }
}
