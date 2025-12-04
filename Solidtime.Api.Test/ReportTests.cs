namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Reports endpoint
/// </summary>
public class ReportTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting all reports succeeds
	/// </summary>
	[Fact]
	public async Task Reports_Get_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Reports
			.GetAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		// Links may be null when the result set is empty
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a report succeeds
	/// </summary>
	/// <remarks>
	/// This integration test is intentionally longer than Codacy's 50-line complexity threshold.
	/// The length is justified as it tests a complete CRUD workflow in a single transaction
	/// to ensure data consistency and proper cleanup.
	/// </remarks>
	[Fact]
	public async Task Reports_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();
		string? reportId = null;

		try
		{
			// Create
			var createRequest = new ReportStoreRequest
			{
				Name = $"Test Report {Guid.NewGuid()}",
				Description = "Test report description",
				IsPublic = false,
				Properties = new ReportProperties
				{
					Start = "2024-01-01T00:00:00Z",
					End = "2024-12-31T23:59:59Z",
					Group = "project",
					SubGroup = "task",
					HistoryGroup = "month"
				}
			};

			var createResult = await SolidtimeClient
				.Reports
				.CreateAsync(organizationId, createRequest, CancellationToken);

			createResult.Should().NotBeNull();
			createResult.Data.Should().NotBeNull();
			createResult.Data.Name.Should().Be(createRequest.Name);
			createResult.Data.Description.Should().Be(createRequest.Description);
			createResult.Data.Id.Should().NotBeNullOrWhiteSpace();

			reportId = createResult.Data.Id;

			// Get by ID
			var getResult = await SolidtimeClient
				.Reports
				.GetByIdAsync(organizationId, reportId, CancellationToken);

			getResult.Should().NotBeNull();
			getResult.Data.Id.Should().Be(reportId);
			getResult.Data.Name.Should().Be(createRequest.Name);

			// Update
			var updateRequest = new ReportUpdateRequest
			{
				Name = $"Updated Report {Guid.NewGuid()}",
				Description = "Updated description",
				IsPublic = true
			};

			var updateResult = await SolidtimeClient
				.Reports
				.UpdateAsync(organizationId, reportId, updateRequest, CancellationToken);

			updateResult.Should().NotBeNull();
			updateResult.Data.Id.Should().Be(reportId);
			updateResult.Data.Name.Should().Be(updateRequest.Name);
			updateResult.Data.Description.Should().Be(updateRequest.Description);
			updateResult.Data.IsPublic.Should().BeTrue();

			// Delete
			await SolidtimeClient
				.Reports
				.DeleteAsync(organizationId, reportId, CancellationToken);

			// Verify deletion by checking it doesn't appear in the list
			var allReports = await SolidtimeClient
				.Reports
				.GetAsync(organizationId, CancellationToken);

			allReports.Data.Should().NotContain(r => r.Id == reportId);
		}
		finally
		{
			// Ensure cleanup even if test fails
			if (reportId != null)
			{
				try
				{
					await SolidtimeClient
						.Reports
						.DeleteAsync(organizationId, reportId, CancellationToken);
				}
				catch
				{
					// Report may already be deleted, ignore errors
				}
			}
		}
	}

	/// <summary>
	/// Tests that the paginated response contains proper metadata
	/// </summary>
	[Fact]
	public async Task Reports_Get_HasPaginationMetadata()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Reports
			.GetAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();
		result.Meta.Should().NotBeNull();

		// The Solidtime API returns paginated response but doesn't accept pagination params
		if (result.Data.Count > 0)
		{
			result.Meta!.Total.Should().BeGreaterThanOrEqualTo(result.Data.Count);
		}

		result.Data.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that report timestamps are handled correctly
	/// </summary>
	[Fact]
	public async Task Reports_Get_HasValidTimestamps()
	{
		var organizationId = await GetOrganizationIdAsync();
		string? reportId = null;

		try
		{
			// Create a test report
			var createRequest = new ReportStoreRequest
			{
				Name = $"Timestamp Test Report {Guid.NewGuid()}",
				Description = "For testing timestamps",
				IsPublic = false,
				Properties = new ReportProperties
				{
					Start = "2024-01-01T00:00:00Z",
					End = "2024-12-31T23:59:59Z",
					Group = "project",
					SubGroup = "task",
					HistoryGroup = "day"
				}
			};

			var createResult = await SolidtimeClient
				.Reports
				.CreateAsync(organizationId, createRequest, CancellationToken);

			reportId = createResult.Data.Id;

			// Timestamps should be present and valid
			if (createResult.Data.CreatedAt.HasValue)
			{
				createResult.Data.CreatedAt.Value.Should().BeBefore(DateTimeOffset.UtcNow.AddMinutes(1));
				createResult.Data.CreatedAt.Value.Should().BeAfter(DateTimeOffset.UtcNow.AddMinutes(-5));
			}

			if (createResult.Data.UpdatedAt.HasValue)
			{
				createResult.Data.UpdatedAt.Value.Should().BeBefore(DateTimeOffset.UtcNow.AddMinutes(1));
			}
		}
		finally
		{
			if (reportId != null)
			{
				try
				{
					await SolidtimeClient
						.Reports
						.DeleteAsync(organizationId, reportId, CancellationToken);
				}
				catch
				{
					// Ignore cleanup errors
				}
			}
		}
	}

	/// <summary>
	/// Tests that getting a report by ID succeeds
	/// </summary>
	[Fact]
	public async Task Reports_GetById_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();
		string? reportId = null;

		try
		{
			// Create a test report first
			var createRequest = new ReportStoreRequest
			{
				Name = $"GetById Test Report {Guid.NewGuid()}",
				Description = "For testing GetById",
				IsPublic = false,
				Properties = new ReportProperties
				{
					Start = "2024-01-01T00:00:00Z",
					End = "2024-12-31T23:59:59Z",
					Group = "project",
					SubGroup = "task",
					HistoryGroup = "week"
				}
			};

			var createResult = await SolidtimeClient
				.Reports
				.CreateAsync(organizationId, createRequest, CancellationToken);

			reportId = createResult.Data.Id;

			// Get by ID
			var getResult = await SolidtimeClient
				.Reports
				.GetByIdAsync(organizationId, reportId, CancellationToken);

			getResult.Should().NotBeNull();
			getResult.Data.Should().NotBeNull();
			getResult.Data.Id.Should().Be(reportId);
			getResult.Data.Name.Should().Be(createRequest.Name);
			getResult.Data.Description.Should().Be(createRequest.Description);
		}
		finally
		{
			if (reportId != null)
			{
				try
				{
					await SolidtimeClient
						.Reports
						.DeleteAsync(organizationId, reportId, CancellationToken);
				}
				catch
				{
					// Ignore cleanup errors
				}
			}
		}
	}
}
