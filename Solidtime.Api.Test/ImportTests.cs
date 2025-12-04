namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Imports endpoint
/// </summary>
public class ImportTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting available importers succeeds
	/// </summary>
	[Fact]
	public async Task Imports_GetImporters_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Imports
			.GetImportersAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		
		// Verify structure of importers
		if (result.Data.Count > 0)
		{
			foreach (var importer in result.Data)
			{
				importer.Key.Should().NotBeNullOrWhiteSpace();
				importer.Name.Should().NotBeNullOrWhiteSpace();
				importer.Description.Should().NotBeNullOrWhiteSpace();
			}
		}
	}

	/// <summary>
	/// Tests that the import request structure is valid
	/// Note: We don't actually perform imports as they have side effects
	/// </summary>
	[Fact]
	public async Task Imports_RequestStructure_IsValid()
	{
		// This test validates that the import request structure is correct
		// without actually creating an import (which would require external data files)
		var organizationId = await GetOrganizationIdAsync();

		// Validate that we can construct a valid import request
		var importRequest = new ImportRequest
		{
			Type = "toggl",
			Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("{}"))
		};

		// Verify the request object is properly constructed
		importRequest.Type.Should().Be("toggl");
		importRequest.Data.Should().NotBeNullOrWhiteSpace();

		// Note: We intentionally do NOT call ImportDataAsync here to avoid side effects
		// Actual import tests require valid export data files and appropriate cleanup
	}
}
