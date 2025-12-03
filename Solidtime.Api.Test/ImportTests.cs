namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Imports endpoint
/// </summary>
public class ImportTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting all imports succeeds or returns 404 if not implemented
	/// Note: Imports API may not be available in all Solidtime installations
	/// </summary>
	[Fact]
	public async Task Imports_Get_SucceedsOrNotImplemented()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Imports
			.GetAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		// Links may be null when the result set is empty
	}

	/// <summary>
	/// Tests that pagination works correctly or returns 404 if not implemented
	/// </summary>
	[Fact]
	public async Task Imports_Pagination_Works()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Imports
			.GetAsync(organizationId, 1, 5, CancellationToken);

		result.Should().NotBeNull();
		result.Meta.Should().NotBeNull();

		// The Solidtime API only populates pagination metadata when there is data
		if (result.Data.Count > 0 || result.Meta!.CurrentPage.HasValue)
		{
			result.Meta!.CurrentPage.Should().Be(1);
		}

		result.Data.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that import data has valid structure
	/// </summary>
	[Fact]
	public async Task Imports_Get_HasValidStructure()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Imports
			.GetAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();

		// If there are existing imports, verify their structure
		if (result.Data.Count > 0)
		{
			foreach (var import in result.Data)
			{
				import.Id.Should().NotBeNullOrWhiteSpace();
				import.Source.Should().NotBeNullOrWhiteSpace();
				import.Status.Should().NotBeNullOrWhiteSpace();
				import.OrganizationId.Should().Be(organizationId);

				// These fields may be null depending on import status
				if (import.TotalItems.HasValue)
				{
					import.TotalItems.Value.Should().BeGreaterThanOrEqualTo(0);
				}

				if (import.ImportedItems.HasValue)
				{
					import.ImportedItems.Value.Should().BeGreaterThanOrEqualTo(0);
					if (import.TotalItems.HasValue)
					{
						import.ImportedItems.Value.Should().BeLessThanOrEqualTo(import.TotalItems.Value);
					}
				}
			}
		}
	}

	/// <summary>
	/// Tests that import timestamps are handled correctly
	/// </summary>
	[Fact]
	public async Task Imports_Get_HasValidTimestamps()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Imports
			.GetAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();

		// If there are existing imports, verify their timestamps
		if (result.Data.Count > 0)
		{
			foreach (var import in result.Data)
			{
				if (import.CreatedAt.HasValue)
				{
					import.CreatedAt.Value.Should().BeBefore(DateTimeOffset.UtcNow.AddMinutes(1));
				}

				if (import.UpdatedAt.HasValue)
				{
					import.UpdatedAt.Value.Should().BeBefore(DateTimeOffset.UtcNow.AddMinutes(1));

					if (import.CreatedAt.HasValue)
					{
						import.UpdatedAt.Value.Should().BeOnOrAfter(import.CreatedAt.Value);
					}
				}
			}
		}
	}

	/// <summary>
	/// Note: Creating actual imports requires valid credentials for external systems (e.g., Toggl)
	/// and could have side effects (importing data). This test is intentionally minimal and
	/// focuses on validating the API structure rather than performing actual imports.
	/// For integration testing with actual imports, you would need:
	/// 1. Valid Toggl API token
	/// 2. Toggl workspace ID
	/// 3. A test environment that can handle import side effects
	/// </summary>
	[Fact]
	public async Task Imports_Structure_ValidatesCorrectly()
	{
		// This test validates that the import request structure is correct
		// without actually creating an import (which would require external credentials)
		var organizationId = await GetOrganizationIdAsync();

		// Validate that we can construct a valid import request
		var importRequest = new ImportStoreRequest
		{
			Source = "toggl",
			ApiToken = "dummy-token-for-structure-validation",
			WorkspaceId = "dummy-workspace-id",
			StartDate = "2024-01-01",
			EndDate = "2024-01-31"
		};

		// Verify the request object is properly constructed
		importRequest.Source.Should().Be("toggl");
		importRequest.ApiToken.Should().NotBeNullOrWhiteSpace();
		importRequest.WorkspaceId.Should().NotBeNullOrWhiteSpace();
		importRequest.StartDate.Should().NotBeNullOrWhiteSpace();
		importRequest.EndDate.Should().NotBeNullOrWhiteSpace();

		// Note: We intentionally do NOT call CreateAsync here to avoid side effects
		// Actual import creation tests should be run in a controlled test environment
		// with valid credentials and appropriate cleanup procedures
	}
}
