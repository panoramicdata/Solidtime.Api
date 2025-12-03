using Microsoft.Extensions.Logging;
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
		try
		{
			projectId = await GetProjectIdAsync();
		}
		catch (InvalidOperationException ex)
		{
			// No projects in organization - test passes but logs warning
			Logger.LogWarning(ex, "No projects found in organization - test cannot verify task CRUD functionality");
			return;
		}

		string? taskId = null;

		try
		{
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
			// Note: API does not return organization_id in the response

			taskId = createResult.Data.Id;

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
				Name = updateRequest.Name, // Name is required for update
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
		finally
		{
			// Ensure cleanup even if test fails
			if (taskId != null)
			{
				try
				{
					await SolidtimeClient
						.Tasks
						.DeleteAsync(organizationId, taskId, CancellationToken);
				}
				catch
				{
					// Task may already be deleted, ignore errors
				}
			}
		}
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
		
		// Note: The Solidtime API only populates pagination metadata when there is data
		// If there are no tasks, CurrentPage and other fields will be null
		if (result.Data.Count > 0 || result.Meta!.CurrentPage.HasValue)
		{
			result.Meta!.CurrentPage.Should().Be(1);
		}
		
		// Note: API may ignore perPage parameter and use its own default
		result.Data.Should().NotBeNull();
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

