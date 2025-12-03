namespace Solidtime.Api.Models;

/// <summary>
/// Request object for updating a project (PUT - full replacement)
/// </summary>
public class ProjectUpdateRequest
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
	public bool? IsBillable { get; set; }

	/// <summary>
	/// Gets or sets whether the project is archived
	/// </summary>
	[JsonPropertyName("is_archived")]
	public bool? IsArchived { get; set; }
}
