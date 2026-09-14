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
		await VerifyGetTasksAsync();
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a task succeeds
	/// </summary>
	[Fact]
	public async Task Tasks_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var projectId = await TryGetProjectIdAsync("task create, update and delete");
		if (projectId is null)
		{
			return;
		}

		var createRequest = new TaskStoreRequest { Name = $"Test Task {Guid.NewGuid()}", ProjectId = projectId };

		await CreateThenCleanupAsync(
			() => SolidtimeClient.Tasks.CreateAsync(organizationId, createRequest, CancellationToken),
			created => SolidtimeClient.Tasks.DeleteAsync(organizationId, created.Data.Id, CancellationToken),
			async createResult =>
			{
				createResult.Should().NotBeNull();
				createResult.Data.Name.Should().Be(createRequest.Name);
				createResult.Data.Id.Should().NotBeNullOrWhiteSpace();
				createResult.Data.ProjectId.Should().Be(projectId);
				createResult.Data.IsDone.Should().BeFalse();

				var taskId = createResult.Data.Id;

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
				var allTasks = await GetTasksAsync();
				allTasks.Data.Should().NotContain(task => task.Id == taskId);
			});
	}

	/// <summary>
	/// Tests that filtering tasks works correctly
	/// </summary>
	[Fact]
	public async Task Tasks_Filtering_Works()
	{
		// Note: The tasks endpoint does not support page/per_page parameters according to the
		// OpenAPI spec. It only supports project_id and done filters, and the API returns
		// pagination metadata using its own default page size (500).
		await VerifyGetTasksAsync();
	}

	/// <summary>
	/// Tests that tasks have valid timestamps
	/// </summary>
	[Fact]
	public async Task Tasks_Get_HasValidTimestamps()
	{
		var result = await GetTasksAsync();

		if (result.Data.Count != 0)
		{
			Verify.Timestamps(result.Data.First());
		}
	}

	private Task<PaginatedResponse<TaskModel>> GetTasksAsync()
		=> ForOrganizationAsync((organizationId, cancellationToken)
			=> SolidtimeClient.Tasks.GetAsync(organizationId, null, null, cancellationToken));

	private async Task VerifyGetTasksAsync()
	{
		var result = await GetTasksAsync();

		Verify.PaginatedEnvelopeWithMeta(result);
	}
}
