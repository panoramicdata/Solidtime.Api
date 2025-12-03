namespace Solidtime.Api.Test;

/// <summary>
/// Configuration for Solidtime API tests
/// </summary>
public class Configuration
{
	/// <summary>
	/// The API token (Personal Access Token)
	/// </summary>
	public required string ApiToken { get; init; }

	/// <summary>
	/// A sample organization ID for testing (optional - will be fetched from /v1/me if not provided)
	/// </summary>
	public string? SampleOrganizationId { get; init; }

	/// <summary>
	/// A sample project ID for testing
	/// </summary>
	public string? SampleProjectId { get; init; }

	/// <summary>
	/// A sample client ID for testing
	/// </summary>
	public string? SampleClientId { get; init; }

	/// <summary>
	/// A sample client name to use for testing CRUD operations
	/// </summary>
	public string CrudClientName { get; init; } = "Test Client for CRUD";
}
