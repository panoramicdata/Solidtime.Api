namespace Solidtime.Api.Models;

/// <summary>
/// Request to create a new import operation
/// </summary>
public class ImportStoreRequest
{
	/// <summary>
	/// The source of the import (e.g., "toggl")
	/// </summary>
	[JsonPropertyName("source")]
	public required string Source { get; set; }

	/// <summary>
	/// The API token for the source system
	/// </summary>
	[JsonPropertyName("api_token")]
	public required string ApiToken { get; set; }

	/// <summary>
	/// The workspace ID from the source system (optional)
	/// </summary>
	[JsonPropertyName("workspace_id")]
	public string? WorkspaceId { get; set; }

	/// <summary>
	/// The start date for importing data (optional)
	/// </summary>
	[JsonPropertyName("start_date")]
	public string? StartDate { get; set; }

	/// <summary>
	/// The end date for importing data (optional)
	/// </summary>
	[JsonPropertyName("end_date")]
	public string? EndDate { get; set; }
}
