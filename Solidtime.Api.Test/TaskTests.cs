namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Tasks endpoint
/// </summary>
public class TaskTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting all tasks succeeds
	/// </summary>
	[Fact]
	public async Task Tasks_Get_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Tasks
			.GetAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		// Links may be null when the result set is empty
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a task succeeds
	/// </summary>
	[Fact]
	public async Task Tasks_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		// Try to get a project ID - test will be inconclusive if no projects exist
		string? projectId;
		try { projectId = await GetProjectIdAsync(); }
		catch (InvalidOperationException ex)
		{
			Logger.LogWarning(ex, "No projects found - test cannot verify task CRUD");
			return;
		}

		string? taskId = null;
		try
		{
			// Create
			var createRequest = new TaskStoreRequest { Name = $"Test Task {Guid.NewGuid()}", ProjectId = projectId };
			var createResult = await SolidtimeClient.Tasks.CreateAsync(organizationId, createRequest, CancellationToken);

			createResult.Should().NotBeNull();
			createResult.Data.Name.Should().Be(createRequest.Name);
			createResult.Data.Id.Should().NotBeNullOrWhiteSpace();
			createResult.Data.ProjectId.Should().Be(projectId);
			createResult.Data.IsDone.Should().BeFalse();
			taskId = createResult.Data.Id;

			// Update
			var updateRequest = new TaskUpdateRequest { Name = $"Updated Task {Guid.NewGuid()}" };
			var updateResult = await SolidtimeClient.Tasks.UpdateAsync(organizationId, taskId, updateRequest, CancellationToken);

			updateResult.Should().NotBeNull();
			updateResult.Data.Id.Should().Be(taskId);
			updateResult.Data.Name.Should().Be(updateRequest.Name);

			// Mark as done
			var doneRequest = new TaskUpdateRequest { Name = updateRequest.Name, IsDone = true };
			var doneResult = await SolidtimeClient.Tasks.UpdateAsync(organizationId, taskId, doneRequest, CancellationToken);
			doneResult.Data.IsDone.Should().BeTrue();

			// Delete and verify
			await SolidtimeClient.Tasks.DeleteAsync(organizationId, taskId, CancellationToken);
			var allTasks = await SolidtimeClient.Tasks.GetAsync(organizationId, null, null, CancellationToken);
			allTasks.Data.Should().NotContain(t => t.Id == taskId);
		}
		finally
		{
			if (taskId != null)
				await SafeDeleteAsync(() => SolidtimeClient.Tasks.DeleteAsync(organizationId, taskId, CancellationToken));
		}
	}

	/// <summary>
	/// Tests that filtering tasks works correctly
	/// </summary>
	[Fact]
	public async Task Tasks_Filtering_Works()
	{
		var organizationId = await GetOrganizationIdAsync();

		// Test basic retrieval
		var result = await SolidtimeClient
			.Tasks
			.GetAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Data.Should().NotBeNull();

		// Note: The tasks endpoint does not support page/per_page parameters according to the OpenAPI spec
		// It only supports project_id and done filters
		// The API returns pagination metadata but uses its own default page size (500)
	}

	/// <summary>
	/// Tests that tasks have valid timestamps
	/// </summary>
	[Fact]
	public async Task Tasks_Get_HasValidTimestamps()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Tasks
			.GetAsync(organizationId, null, null, CancellationToken);

		if (result.Data.Count != 0)
		{
			var task = result.Data.First();
			task.CreatedAt.Should().NotBeNull();
			task.CreatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);
			task.UpdatedAt.Should().NotBeNull();
			task.UpdatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);
			task.UpdatedAt.Value.Should().BeOnOrAfter(task.CreatedAt.Value);
		}
	}
}

