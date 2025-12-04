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
	/// <remarks>
	/// This integration test is intentionally longer than Codacy's 50-line complexity threshold.
	/// The length is justified as it tests a complete CRUD workflow in a single transaction
	/// to ensure data consistency and proper cleanup.
	/// </remarks>
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
	/// Tests that limit/offset pagination works correctly
	/// </summary>
	/// <remarks>
	/// Note: The time-entries endpoint uses limit/offset pagination instead of page/perPage.
	/// The API returns total count in meta but does not populate CurrentPage/LastPage fields
	/// because those are concepts for page-based pagination.
	/// </remarks>
	[Fact]
	public async Task TimeEntries_Pagination_Works()
	{
		var organizationId = await GetOrganizationIdAsync();

		// Use limit=5, offset=0 to get the first 5 entries
		var result = await SolidtimeClient
			.TimeEntries
			.GetAsync(organizationId, limit: 5, offset: 0, CancellationToken);

		result.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Data.Should().NotBeNull();

		// Verify the total count is populated
		result.Meta!.Total.Should().BeGreaterThanOrEqualTo(0);

		// Verify the limit was respected (should return at most 5 entries)
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
			// Note: TimeEntryResource does not include created_at or updated_at according to the Solidtime API spec.
			// Only the Start timestamp is available for time entries.
			timeEntry.Start.Should().BeBefore(DateTimeOffset.UtcNow.AddMinutes(1)); // Allow small clock drift
		}
	}
}
