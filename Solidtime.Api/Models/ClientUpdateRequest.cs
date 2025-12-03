namespace Solidtime.Api.Models;

/// <summary>
/// Request object for updating a client (PUT - full replacement)
/// </summary>
public class ClientUpdateRequest
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
}
