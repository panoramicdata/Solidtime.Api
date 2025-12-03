using Solidtime.Api.Models;

namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the TimeEntries endpoint
/// </summary>
public class TimeEntryTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting all time entries succeeds
	/// </summary>
	[Fact]
	public async Task TimeEntries_Get_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.TimeEntries
			.GetAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Links.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a time entry succeeds
	/// </summary>
	[Fact]
	public async Task TimeEntries_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var now = DateTimeOffset.UtcNow;

		// Create
		var createRequest = new TimeEntryStoreRequest
		{
			Description = $"Test Time Entry {Guid.NewGuid()}",
			Start = now.AddHours(-2),
			End = now.AddHours(-1),
			Billable = true
		};

		var createResult = await SolidtimeClient
			.TimeEntries
			.CreateAsync(organizationId, createRequest, CancellationToken);

		createResult.Should().NotBeNull();
		createResult.Data.Should().NotBeNull();
		createResult.Data.Description.Should().Be(createRequest.Description);
		createResult.Data.Id.Should().NotBeNullOrWhiteSpace();
		createResult.Data.OrganizationId.Should().Be(organizationId);
		createResult.Data.Billable.Should().BeTrue();
		createResult.Data.End.Should().NotBeNull();
		createResult.Data.Duration.Should().BePositive();

		var timeEntryId = createResult.Data.Id;

		// Get by ID
		var getResult = await SolidtimeClient
			.TimeEntries
			.GetByIdAsync(organizationId, timeEntryId, CancellationToken);

		getResult.Should().NotBeNull();
		getResult.Data.Id.Should().Be(timeEntryId);
		getResult.Data.Description.Should().Be(createRequest.Description);

		// Update
		var updateRequest = new TimeEntryUpdateRequest
		{
			Description = $"Updated Time Entry {Guid.NewGuid()}",
			Billable = false
		};

		var updateResult = await SolidtimeClient
			.TimeEntries
			.UpdateAsync(organizationId, timeEntryId, updateRequest, CancellationToken);

		updateResult.Should().NotBeNull();
		updateResult.Data.Id.Should().Be(timeEntryId);
		updateResult.Data.Description.Should().Be(updateRequest.Description);
		updateResult.Data.Billable.Should().BeFalse();

		// Delete
		await SolidtimeClient
			.TimeEntries
			.DeleteAsync(organizationId, timeEntryId, CancellationToken);

		// Verify deletion by checking it doesn't appear in the list
		var allTimeEntries = await SolidtimeClient
			.TimeEntries
			.GetAsync(organizationId, null, null, CancellationToken);

		allTimeEntries.Data.Should().NotContain(t => t.Id == timeEntryId);
	}

	/// <summary>
	/// Tests that creating a running time entry works
	/// </summary>
	[Fact]
	public async Task TimeEntries_CreateRunning_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		// Create a running time entry (no end time)
		var createRequest = new TimeEntryStoreRequest
		{
			Description = $"Running Timer {Guid.NewGuid()}",
			Start = DateTimeOffset.UtcNow.AddMinutes(-5)
		};

		var createResult = await SolidtimeClient
			.TimeEntries
			.CreateAsync(organizationId, createRequest, CancellationToken);

		createResult.Should().NotBeNull();
		createResult.Data.End.Should().BeNull();
		createResult.Data.Duration.Should().BeNull();

		// Stop the timer
		var stopRequest = new TimeEntryUpdateRequest
		{
			End = DateTimeOffset.UtcNow
		};

		var stopResult = await SolidtimeClient
			.TimeEntries
			.UpdateAsync(organizationId, createResult.Data.Id, stopRequest, CancellationToken);

		stopResult.Data.End.Should().NotBeNull();
		stopResult.Data.Duration.Should().BePositive();

		// Clean up
		await SolidtimeClient
			.TimeEntries
			.DeleteAsync(organizationId, createResult.Data.Id, CancellationToken);
	}

	/// <summary>
	/// Tests that pagination works correctly
	/// </summary>
	[Fact]
	public async Task TimeEntries_Pagination_Works()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.TimeEntries
			.GetAsync(organizationId, 1, 5, CancellationToken);

		result.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Meta!.CurrentPage.Should().Be(1);
		result.Meta.PerPage.Should().Be(5);
		result.Data.Count.Should().BeLessThanOrEqualTo(5);
	}

	/// <summary>
	/// Tests that time entries have valid timestamps
	/// </summary>
	[Fact]
	public async Task TimeEntries_Get_HasValidTimestamps()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.TimeEntries
			.GetAsync(organizationId, null, null, CancellationToken);

		if (result.Data.Count != 0)
		{
			var timeEntry = result.Data.First();
			timeEntry.CreatedAt.Should().NotBeNull();
			timeEntry.CreatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);
			timeEntry.UpdatedAt.Should().NotBeNull();
			timeEntry.UpdatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);
			timeEntry.UpdatedAt.Value.Should().BeOnOrAfter(timeEntry.CreatedAt.Value);
			timeEntry.Start.Should().BeBefore(DateTimeOffset.UtcNow);
		}
	}
}
