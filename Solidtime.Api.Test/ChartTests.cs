using System.Globalization;

namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Charts endpoint
/// </summary>
public class ChartTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting weekly hours chart succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetWeeklyHours_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetWeeklyHoursAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		// Data may be empty if there are no time entries
	}

	/// <summary>
	/// Tests that getting weekly hours chart with date range succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetWeeklyHours_WithDateRange_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		// Get last 4 weeks of data
		var endDate = DateTimeOffset.UtcNow;
		var startDate = endDate.AddDays(-28);

		var result = await SolidtimeClient
			.Charts
			.GetWeeklyHoursAsync(
				organizationId,
				startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				CancellationToken);

		result.Should().NotBeNull();

		// If there is data, verify the structure
		if (result.Count > 0)
		{
			var dataPoint = result.First();
			dataPoint.Name.Should().NotBeNullOrWhiteSpace();
			dataPoint.Color.Should().NotBeNullOrWhiteSpace();
			dataPoint.Value.Should().BeGreaterThanOrEqualTo(0);
		}
	}

	/// <summary>
	/// Tests that chart data has valid structure
	/// </summary>
	[Fact]
	public async Task Charts_WeeklyHours_HasValidStructure()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetWeeklyHoursAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();

		// If there is data, verify all required fields are present
		if (result.Count > 0)
		{
			foreach (var dataPoint in result)
			{
				dataPoint.Name.Should().NotBeNullOrWhiteSpace();
				dataPoint.Color.Should().NotBeNullOrWhiteSpace();
				dataPoint.Value.Should().BeGreaterThanOrEqualTo(0);
				// ID may be null for aggregated data like "No project"
			}
		}
	}

	/// <summary>
	/// Tests that getting weekly project overview chart succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetWeeklyProjectOverview_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetWeeklyProjectOverviewAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		// Data may be empty if there are no time entries
	}

	/// <summary>
	/// Tests that getting weekly project overview chart with date range succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetWeeklyProjectOverview_WithDateRange_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		// Get last 4 weeks of data
		var endDate = DateTimeOffset.UtcNow;
		var startDate = endDate.AddDays(-28);

		var result = await SolidtimeClient
			.Charts
			.GetWeeklyProjectOverviewAsync(
				organizationId,
				startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				CancellationToken);

		result.Should().NotBeNull();

		// If there is data, verify the structure
		if (result.Count > 0)
		{
			var dataPoint = result.First();
			dataPoint.Name.Should().NotBeNullOrWhiteSpace();
			dataPoint.Color.Should().NotBeNullOrWhiteSpace();
			dataPoint.Value.Should().BeGreaterThanOrEqualTo(0);
		}
	}

	/// <summary>
	/// Tests that weekly project overview chart data has valid structure
	/// </summary>
	[Fact]
	public async Task Charts_WeeklyProjectOverview_HasValidStructure()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetWeeklyProjectOverviewAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();

		// If there is data, verify all required fields are present
		if (result.Count > 0)
		{
			foreach (var dataPoint in result)
			{
				dataPoint.Name.Should().NotBeNullOrWhiteSpace();
				dataPoint.Color.Should().NotBeNullOrWhiteSpace();
				dataPoint.Value.Should().BeGreaterThanOrEqualTo(0);
				// ID may be null for aggregated data
			}
		}
	}

	/// <summary>
	/// Tests that chart data can be retrieved for a specific month
	/// NOTE: This test may fail with 404 if the charts feature is not enabled/available in the test organization.
	/// The charts endpoints may be a premium feature or require specific configuration in Solidtime.
	/// </summary>
	[Fact]
	public async Task Charts_GetDataForSpecificMonth_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		// Get data for current month
		var now = DateTimeOffset.UtcNow;
		var startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
		var endDate = startDate.AddMonths(1).AddDays(-1);

		var hoursResult = await SolidtimeClient
			.Charts
			.GetWeeklyHoursAsync(
				organizationId,
				startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				CancellationToken);

		hoursResult.Should().NotBeNull();

		var projectResult = await SolidtimeClient
			.Charts
			.GetWeeklyProjectOverviewAsync(
				organizationId,
				startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				CancellationToken);

		projectResult.Should().NotBeNull();
	}
}
