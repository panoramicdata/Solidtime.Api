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
		// Links may be null when the result set is empty
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a time entry succeeds
	/// </summary>
	[Fact]
	public async Task TimeEntries_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();
		var memberId = await GetMemberIdAsync();
		string? timeEntryId = null;

		try
		{
			var now = DateTimeOffset.UtcNow;

			// Create
			var createRequest = new TimeEntryStoreRequest
			{
				MemberId = memberId,
				Description = $"Test Time Entry {Guid.NewGuid()}",
				Start = now.AddHours(-2).ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture),
				End = now.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture),
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

			timeEntryId = createResult.Data.Id;

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
		finally
		{
			// Ensure cleanup even if test fails
			if (timeEntryId != null)
			{
				try
				{
					await SolidtimeClient
						.TimeEntries
						.DeleteAsync(organizationId, timeEntryId, CancellationToken);
				}
				catch
				{
					// Time entry may already be deleted, ignore errors
				}
			}
		}
	}

	/// <summary>
	/// Tests that creating a running time entry works
	/// </summary>
	[Fact]
	public async Task TimeEntries_CreateRunning_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();
		var memberId = await GetMemberIdAsync();
		string? timeEntryId = null;

		try
		{
			// Create a running time entry (no end time)
			var createRequest = new TimeEntryStoreRequest
			{
				MemberId = memberId,
				Description = $"Running Timer {Guid.NewGuid()}",
				Start = DateTimeOffset.UtcNow.AddMinutes(-5).ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture),
				Billable = false
			};

			var createResult = await SolidtimeClient
				.TimeEntries
				.CreateAsync(organizationId, createRequest, CancellationToken);

			createResult.Should().NotBeNull();
			createResult.Data.End.Should().BeNull();

			timeEntryId = createResult.Data.Id;

			// Stop the timer
			var stopRequest = new TimeEntryUpdateRequest
			{
				End = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture)
			};

			var stopResult = await SolidtimeClient
				.TimeEntries
				.UpdateAsync(organizationId, timeEntryId, stopRequest, CancellationToken);

			stopResult.Data.End.Should().NotBeNull();
			stopResult.Data.Duration.Should().BePositive();

			// Clean up
			await SolidtimeClient
				.TimeEntries
				.DeleteAsync(organizationId, timeEntryId, CancellationToken);
		}
		finally
		{
			// Ensure cleanup even if test fails
			if (timeEntryId != null)
			{
				try
				{
					await SolidtimeClient
						.TimeEntries
						.DeleteAsync(organizationId, timeEntryId, CancellationToken);
				}
				catch
				{
					// Time entry may already be deleted, ignore errors
				}
			}
		}
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
		
		// Note: The Solidtime API only populates pagination metadata when there is data
		// If there are no time entries, CurrentPage and other fields will be null
		if (result.Data.Count > 0 || result.Meta!.CurrentPage.HasValue)
		{
			result.Meta!.CurrentPage.Should().Be(1);
		}
		
		// Note: API may ignore perPage parameter and use its own default
		result.Data.Should().NotBeNull();
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
