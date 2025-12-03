namespace Solidtime.Api.Models;

/// <summary>
/// Represents a client in the Solidtime system
/// </summary>
public class Client : SolidtimeEntityBase
{
	/// <summary>
	/// The client's name
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// Gets or sets whether the client is archived
	/// </summary>
	[JsonPropertyName("is_archived")]
	public required bool IsArchived { get; set; }

	/// <summary>
	/// The organization ID this client belongs to (not always returned by the API)
	/// </summary>
	[JsonPropertyName("organization_id")]
	public string? OrganizationId { get; set; }
}
