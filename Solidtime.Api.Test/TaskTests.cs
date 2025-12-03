using Solidtime.Api.Models;

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
		result.Links.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a task succeeds
	/// </summary>
	[Fact]
	public async Task Tasks_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();
		var projectId = await GetProjectIdAsync();

		// Create
		var createRequest = new TaskStoreRequest
		{
			Name = $"Test Task {Guid.NewGuid()}",
			ProjectId = projectId
		};

		var createResult = await SolidtimeClient
			.Tasks
			.CreateAsync(organizationId, createRequest, CancellationToken);

		createResult.Should().NotBeNull();
		createResult.Data.Should().NotBeNull();
		createResult.Data.Name.Should().Be(createRequest.Name);
		createResult.Data.Id.Should().NotBeNullOrWhiteSpace();
		createResult.Data.ProjectId.Should().Be(projectId);
		createResult.Data.IsDone.Should().BeFalse();
		createResult.Data.OrganizationId.Should().Be(organizationId);

		var taskId = createResult.Data.Id;

		// Get by ID
		var getResult = await SolidtimeClient
			.Tasks
			.GetByIdAsync(organizationId, taskId, CancellationToken);

		getResult.Should().NotBeNull();
		getResult.Data.Id.Should().Be(taskId);
		getResult.Data.Name.Should().Be(createRequest.Name);

		// Update
		var updateRequest = new TaskUpdateRequest
		{
			Name = $"Updated Task {Guid.NewGuid()}"
		};

		var updateResult = await SolidtimeClient
			.Tasks
			.UpdateAsync(organizationId, taskId, updateRequest, CancellationToken);

		updateResult.Should().NotBeNull();
		updateResult.Data.Id.Should().Be(taskId);
		updateResult.Data.Name.Should().Be(updateRequest.Name);

		// Mark as done
		var doneRequest = new TaskUpdateRequest
		{
			IsDone = true
		};

		var doneResult = await SolidtimeClient
			.Tasks
			.UpdateAsync(organizationId, taskId, doneRequest, CancellationToken);

		doneResult.Data.IsDone.Should().BeTrue();

		// Delete
		await SolidtimeClient
			.Tasks
			.DeleteAsync(organizationId, taskId, CancellationToken);

		// Verify deletion by checking it doesn't appear in the list
		var allTasks = await SolidtimeClient
			.Tasks
			.GetAsync(organizationId, null, null, CancellationToken);

		allTasks.Data.Should().NotContain(t => t.Id == taskId);
	}

	/// <summary>
	/// Tests that pagination works correctly
	/// </summary>
	[Fact]
	public async Task Tasks_Pagination_Works()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Tasks
			.GetAsync(organizationId, 1, 5, CancellationToken);

		result.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Meta!.CurrentPage.Should().Be(1);
		result.Meta.PerPage.Should().Be(5);
		result.Data.Count.Should().BeLessThanOrEqualTo(5);
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
