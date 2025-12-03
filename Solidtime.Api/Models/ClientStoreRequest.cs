using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Request object for creating a new client
/// </summary>
public class ClientStoreRequest
{
	/// <summary>
	/// The client's name
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }
}
