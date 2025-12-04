using System.Globalization;

namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Charts endpoint
/// </summary>
public class ChartTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting weekly project overview chart succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetWeeklyProjectOverview_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetWeeklyProjectOverviewAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();
		// Data may be empty if there are no time entries
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
			.GetWeeklyProjectOverviewAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();

		// If there is data, verify all required fields are present
		if (result.Count > 0)
		{
			foreach (var dataPoint in result)
			{
				dataPoint.Name.Should().NotBeNullOrWhiteSpace();
				dataPoint.Color.Should().NotBeNullOrWhiteSpace();
				dataPoint.Value.Should().BeGreaterThanOrEqualTo(0);
			}
		}
	}

	/// <summary>
	/// Tests that getting weekly history chart succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetWeeklyHistory_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetWeeklyHistoryAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that weekly history chart data has valid structure
	/// </summary>
	[Fact]
	public async Task Charts_WeeklyHistory_HasValidStructure()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetWeeklyHistoryAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();

		// If there is data, verify all required fields are present
		if (result.Count > 0)
		{
			foreach (var dataPoint in result)
			{
				dataPoint.Date.Should().NotBeNullOrWhiteSpace();
				dataPoint.Duration.Should().BeGreaterThanOrEqualTo(0);
			}
		}
	}

	/// <summary>
	/// Tests that getting total weekly time succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetTotalWeeklyTime_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetTotalWeeklyTimeAsync(organizationId, CancellationToken);

		// Result is total seconds, should be 0 or greater
		result.Should().BeGreaterThanOrEqualTo(0);
	}

	/// <summary>
	/// Tests that getting total weekly billable time succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetTotalWeeklyBillableTime_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetTotalWeeklyBillableTimeAsync(organizationId, CancellationToken);

		// Result is total billable seconds, should be 0 or greater
		result.Should().BeGreaterThanOrEqualTo(0);
	}

	/// <summary>
	/// Tests that getting daily tracked hours succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetDailyTrackedHours_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetDailyTrackedHoursAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that daily tracked hours data has valid structure
	/// </summary>
	[Fact]
	public async Task Charts_DailyTrackedHours_HasValidStructure()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetDailyTrackedHoursAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();

		// If there is data, verify all required fields are present
		if (result.Count > 0)
		{
			foreach (var dataPoint in result)
			{
				dataPoint.Date.Should().NotBeNullOrWhiteSpace();
				dataPoint.Duration.Should().BeGreaterThanOrEqualTo(0);
			}
		}
	}

	/// <summary>
	/// Tests that getting last seven days chart succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetLastSevenDays_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetLastSevenDaysAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that last seven days data has valid structure
	/// </summary>
	[Fact]
	public async Task Charts_LastSevenDays_HasValidStructure()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Charts
			.GetLastSevenDaysAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();

		// If there is data, verify all required fields are present
		if (result.Count > 0)
		{
			foreach (var dataPoint in result)
			{
				dataPoint.Date.Should().NotBeNullOrWhiteSpace();
				dataPoint.Duration.Should().BeGreaterThanOrEqualTo(0);
				dataPoint.History.Should().NotBeNull();
			}
		}
	}
}
