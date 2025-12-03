using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Wrapper for single-item API responses
/// </summary>
/// <typeparam name="T">The type of data being wrapped</typeparam>
public class DataWrapper<T>
{
	/// <summary>
	/// The wrapped data
	/// </summary>
	[JsonPropertyName("data")]
	public required T Data { get; set; }
}
