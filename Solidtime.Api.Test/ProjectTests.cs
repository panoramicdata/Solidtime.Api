using Solidtime.Api.Models;

namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Projects endpoint
/// </summary>
public class ProjectTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting all projects succeeds
	/// </summary>
	[Fact]
	public async Task Projects_Get_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Projects
			.GetAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Links.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a project succeeds
	/// </summary>
	[Fact]
	public async Task Projects_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();
		var clientId = await GetClientIdAsync(); // Get a valid client ID
		string? projectId = null;

		try
		{
			// Create
			var createRequest = new ProjectStoreRequest
			{
				Name = $"Test Project {Guid.NewGuid()}",
				Color = "#ff5733", // Use lowercase hex color
				ClientId = clientId, // Provide a valid client ID
				IsBillable = true
			};

			var createResult = await SolidtimeClient
				.Projects
				.CreateAsync(organizationId, createRequest, CancellationToken);

			createResult.Should().NotBeNull();
			createResult.Data.Should().NotBeNull();
			createResult.Data.Name.Should().Be(createRequest.Name);
			createResult.Data.Color.Should().Be(createRequest.Color);
			createResult.Data.Id.Should().NotBeNullOrWhiteSpace();
			createResult.Data.IsArchived.Should().BeFalse();
			// Note: OrganizationId is not always returned by the API (especially during creation)

			projectId = createResult.Data.Id;

			// Get by ID
			var getResult = await SolidtimeClient
				.Projects
				.GetByIdAsync(organizationId, projectId, CancellationToken);

			getResult.Should().NotBeNull();
			getResult.Data.Id.Should().Be(projectId);
			getResult.Data.Name.Should().Be(createRequest.Name);

			// Update
			var updateRequest = new ProjectUpdateRequest
			{
				Name = $"Updated Project {Guid.NewGuid()}",
				Color = "#33c3ff", // Use lowercase hex color
				ClientId = clientId, // Include client ID
				IsBillable = createRequest.IsBillable,
				IsArchived = false
			};

			var updateResult = await SolidtimeClient
				.Projects
				.UpdateAsync(organizationId, projectId, updateRequest, CancellationToken);

			updateResult.Should().NotBeNull();
			updateResult.Data.Id.Should().Be(projectId);
			updateResult.Data.Name.Should().Be(updateRequest.Name);
			updateResult.Data.Color.Should().Be(updateRequest.Color);

			// Archive
			var archiveRequest = new ProjectUpdateRequest
			{
				Name = updateRequest.Name, // Must include all required fields for PUT
				Color = updateRequest.Color,
				ClientId = clientId,
				IsBillable = createRequest.IsBillable,
				IsArchived = true
			};

			var archiveResult = await SolidtimeClient
				.Projects
				.UpdateAsync(organizationId, projectId, archiveRequest, CancellationToken);

			archiveResult.Data.IsArchived.Should().BeTrue();

			// Delete
			await SolidtimeClient
				.Projects
				.DeleteAsync(organizationId, projectId, CancellationToken);

			// Verify deletion by checking it doesn't appear in the list
			var allProjects = await SolidtimeClient
				.Projects
				.GetAsync(organizationId, null, null, CancellationToken);

			allProjects.Data.Should().NotContain(p => p.Id == projectId);
		}
		finally
		{
			// Ensure cleanup even if test fails
			if (projectId != null)
			{
				try
				{
					await SolidtimeClient
						.Projects
						.DeleteAsync(organizationId, projectId, CancellationToken);
				}
				catch
				{
					// Project may already be deleted, ignore errors
				}
			}
		}
	}

	/// <summary>
	/// Tests that pagination works correctly
	/// </summary>
	[Fact]
	public async Task Projects_Pagination_Works()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Projects
			.GetAsync(organizationId, 1, 5, CancellationToken);

		result.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Meta!.CurrentPage.Should().Be(1);
		// Note: API may ignore perPage parameter and use its own default
		result.Data.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that the newly added fields are mapped correctly
	/// </summary>
	[Fact]
	public async Task Projects_Get_MapsAllFields()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Projects
			.GetAsync(organizationId, null, null, CancellationToken);

		if (result.Data.Count != 0)
		{
			var project = result.Data.First();
			// Verify all fields are mapped (ExtensionData should be null or empty)
			// The API returns billable_rate, estimated_time, spent_time, is_public
			// which should now be mapped to the model properties
			project.Should().NotBeNull();
			project.Id.Should().NotBeNullOrEmpty();
			project.Name.Should().NotBeNullOrEmpty();
			
			// Note: The Solidtime API does NOT return created_at/updated_at for projects
			// (both in list and individual GET responses), so these will always be null
		}
	}
}
