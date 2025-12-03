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
	/// Note: Not always returned by the API (e.g., during creation)
	/// </summary>
	[JsonPropertyName("organization_id")]
	public string? OrganizationId { get; set; }

	/// <summary>
	/// The billable rate for the project (in cents per hour)
	/// </summary>
	[JsonPropertyName("billable_rate")]
	public int? BillableRate { get; set; }

	/// <summary>
	/// The estimated time for the project (in seconds)
	/// </summary>
	[JsonPropertyName("estimated_time")]
	public int? EstimatedTime { get; set; }

	/// <summary>
	/// The spent time on the project (in seconds)
	/// </summary>
	[JsonPropertyName("spent_time")]
	public int? SpentTime { get; set; }

	/// <summary>
	/// Gets or sets whether the project is public
	/// </summary>
	[JsonPropertyName("is_public")]
	public bool? IsPublic { get; set; }
}
