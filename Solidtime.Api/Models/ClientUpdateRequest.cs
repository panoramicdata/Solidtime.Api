using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Request object for updating a client
/// </summary>
public class ClientUpdateRequest
{
	/// <summary>
	/// The client's name
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	/// <summary>
	/// Gets or sets whether the client is archived
	/// </summary>
	[JsonPropertyName("is_archived")]
	public bool? IsArchived { get; set; }
}
