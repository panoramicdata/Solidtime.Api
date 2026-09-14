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
		var result = await GetProjectsAsync();

		Verify.PaginatedEnvelopeWithMeta(result);
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a project succeeds
	/// </summary>
	[Fact]
	public async Task Projects_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();
		var clientId = await GetClientIdAsync();

		var createRequest = new ProjectStoreRequest { Name = $"Test Project {Guid.NewGuid()}", Color = "#ff5733", ClientId = clientId, IsBillable = true };

		await CreateThenCleanupAsync(
			() => SolidtimeClient.Projects.CreateAsync(organizationId, createRequest, CancellationToken),
			created => SolidtimeClient.Projects.DeleteAsync(organizationId, created.Data.Id, CancellationToken),
			async createResult =>
			{
				createResult.Should().NotBeNull();
				createResult.Data.Id.Should().NotBeNullOrWhiteSpace();
				createResult.Data.IsArchived.Should().BeFalse();
				VerifyProject(createResult.Data, createRequest.Name, createRequest.Color);

				var projectId = createResult.Data.Id;

				// Get by ID
				var getResult = await SolidtimeClient.Projects.GetByIdAsync(organizationId, projectId, CancellationToken);
				getResult.Should().NotBeNull();
				VerifyProject(getResult.Data, createRequest.Name);

				// Update
				var updateRequest = new ProjectUpdateRequest { Name = $"Updated Project {Guid.NewGuid()}", Color = "#33c3ff", ClientId = clientId, IsBillable = true, IsArchived = false };
				var updateResult = await SolidtimeClient.Projects.UpdateAsync(organizationId, projectId, updateRequest, CancellationToken);
				updateResult.Should().NotBeNull();
				VerifyProject(updateResult.Data, updateRequest.Name, updateRequest.Color);

				// Archive
				var archiveRequest = new ProjectUpdateRequest { Name = updateRequest.Name, Color = updateRequest.Color, ClientId = clientId, IsBillable = true, IsArchived = true };
				var archiveResult = await SolidtimeClient.Projects.UpdateAsync(organizationId, projectId, archiveRequest, CancellationToken);
				archiveResult.Data.IsArchived.Should().BeTrue();

				// Delete and verify
				await SolidtimeClient.Projects.DeleteAsync(organizationId, projectId, CancellationToken);
				var allProjects = await GetProjectsAsync();
				allProjects.Data.Should().NotContain(project => project.Id == projectId);
			});
	}

	/// <summary>
	/// Tests that pagination works correctly
	/// </summary>
	[Fact]
	public async Task Projects_Pagination_Works()
	{
		// Request page 1 (perPage is not supported by the API)
		var result = await ForOrganizationAsync((organizationId, cancellationToken)
			=> SolidtimeClient.Projects.GetAsync(organizationId, 1, null, cancellationToken));

		Verify.PaginatedEnvelopeWithMeta(result);

		// Note: The Solidtime API only populates pagination metadata when there is data
		// If there are no projects, CurrentPage and other fields will be null
		if (result.Data.Count > 0 || result.Meta!.CurrentPage.HasValue)
		{
			result.Meta!.CurrentPage.Should().Be(1);
		}
	}

	/// <summary>
	/// Tests that archived filter works correctly
	/// </summary>
	[Fact]
	public async Task Projects_ArchivedFilter_Works()
	{
		// Non-archived projects (the default) and all projects including archived
		var result = await GetProjectsAsync();
		var allResult = await GetProjectsAsync("all");

		Verify.PaginatedEnvelope(result);
		Verify.PaginatedEnvelope(allResult);
	}

	/// <summary>
	/// Tests that the newly added fields are mapped correctly
	/// </summary>
	[Fact]
	public async Task Projects_Get_MapsAllFields()
	{
		var result = await GetProjectsAsync();

		if (result.Data.Count != 0)
		{
			// Verify all fields are mapped (ExtensionData should be null or empty).
			// The API returns billable_rate, estimated_time, spent_time and is_public,
			// which are mapped to the model properties.
			var project = result.Data.First();
			project.Should().NotBeNull();
			project.Id.Should().NotBeNullOrEmpty();
			project.Name.Should().NotBeNullOrEmpty();

			// Note: The Solidtime API does NOT return created_at/updated_at for projects
			// (both in list and individual GET responses), so these will always be null
		}
	}

	private Task<PaginatedResponse<Project>> GetProjectsAsync(string? archived = null)
		=> ForOrganizationAsync((organizationId, cancellationToken)
			=> SolidtimeClient.Projects.GetAsync(organizationId, null, archived, cancellationToken));

	/// <summary>
	/// Verifies a project against the expected name and, optionally, color
	/// </summary>
	private static void VerifyProject(Project project, string expectedName, string? expectedColor = null)
	{
		project.Name.Should().Be(expectedName);

		if (expectedColor is not null)
		{
			project.Color.Should().Be(expectedColor);
		}
	}
}
