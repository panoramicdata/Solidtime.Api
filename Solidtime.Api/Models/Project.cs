using System;
using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Represents a project in the Solidtime system
/// </summary>
public class Project
{
	/// <summary>
	/// The unique identifier for the project
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; set; }

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

	/// <summary>
	/// The date and time when the project was created
	/// </summary>
	[JsonPropertyName("created_at")]
	public required DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// The date and time when the project was last updated
	/// </summary>
	[JsonPropertyName("updated_at")]
	public required DateTimeOffset UpdatedAt { get; set; }
}
