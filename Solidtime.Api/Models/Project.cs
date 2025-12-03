namespace Solidtime.Api.Models;

/// <summary>
/// Represents a project in the Solidtime system
/// </summary>
public class Project : SolidtimeEntityBase
{
	/// <summary>
	/// The project's name
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The project's color (hex color code)
	/// </summary>
	[JsonPropertyName("color")]
	public required string Color { get; set; }

	/// <summary>
	/// The client ID this project belongs to
	/// </summary>
	[JsonPropertyName("client_id")]
	public string? ClientId { get; set; }

	/// <summary>
	/// Gets or sets whether the project is billable
	/// </summary>
	[JsonPropertyName("is_billable")]
	public required bool IsBillable { get; set; }

	/// <summary>
	/// Gets or sets whether the project is archived
	/// </summary>
	[JsonPropertyName("is_archived")]
	public required bool IsArchived { get; set; }

	/// <summary>
	/// The organization ID this project belongs to
	/// </summary>
	[JsonPropertyName("organization_id")]
	public required string OrganizationId { get; set; }
}
