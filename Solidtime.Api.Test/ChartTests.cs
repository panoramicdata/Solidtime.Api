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
		var result = await ForOrganizationAsync(SolidtimeClient.Charts.GetWeeklyProjectOverviewAsync);

		// Data may be empty if there are no time entries
		result.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that weekly project overview chart data has valid structure
	/// </summary>
	[Fact]
	public async Task Charts_WeeklyProjectOverview_HasValidStructure()
	{
		var result = await ForOrganizationAsync(SolidtimeClient.Charts.GetWeeklyProjectOverviewAsync);

		result.Should().NotBeNull();
		foreach (var dataPoint in result)
		{
			dataPoint.Name.Should().NotBeNullOrWhiteSpace();
			dataPoint.Color.Should().NotBeNullOrWhiteSpace();
			dataPoint.Value.Should().BeGreaterThanOrEqualTo(0);
		}
	}

	/// <summary>
	/// Tests that getting weekly history chart succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetWeeklyHistory_Succeeds()
	{
		var result = await ForOrganizationAsync(SolidtimeClient.Charts.GetWeeklyHistoryAsync);

		result.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that weekly history chart data has valid structure
	/// </summary>
	[Fact]
	public async Task Charts_WeeklyHistory_HasValidStructure()
	{
		var result = await ForOrganizationAsync(SolidtimeClient.Charts.GetWeeklyHistoryAsync);

		Verify.DateDurationPoints(result, dataPoint => dataPoint.Date, dataPoint => dataPoint.Duration);
	}

	/// <summary>
	/// Tests that getting total weekly time succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetTotalWeeklyTime_Succeeds()
	{
		var result = await ForOrganizationAsync(SolidtimeClient.Charts.GetTotalWeeklyTimeAsync);

		// Result is total seconds, should be 0 or greater
		result.Should().BeGreaterThanOrEqualTo(0);
	}

	/// <summary>
	/// Tests that getting total weekly billable time succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetTotalWeeklyBillableTime_Succeeds()
	{
		var result = await ForOrganizationAsync(SolidtimeClient.Charts.GetTotalWeeklyBillableTimeAsync);

		// Result is total billable seconds, should be 0 or greater
		result.Should().BeGreaterThanOrEqualTo(0);
	}

	/// <summary>
	/// Tests that getting daily tracked hours succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetDailyTrackedHours_Succeeds()
	{
		var result = await ForOrganizationAsync(SolidtimeClient.Charts.GetDailyTrackedHoursAsync);

		result.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that daily tracked hours data has valid structure
	/// </summary>
	[Fact]
	public async Task Charts_DailyTrackedHours_HasValidStructure()
	{
		var result = await ForOrganizationAsync(SolidtimeClient.Charts.GetDailyTrackedHoursAsync);

		Verify.DateDurationPoints(result, dataPoint => dataPoint.Date, dataPoint => dataPoint.Duration);
	}

	/// <summary>
	/// Tests that getting last seven days chart succeeds
	/// </summary>
	[Fact]
	public async Task Charts_GetLastSevenDays_Succeeds()
	{
		var result = await ForOrganizationAsync(SolidtimeClient.Charts.GetLastSevenDaysAsync);

		result.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that last seven days data has valid structure
	/// </summary>
	[Fact]
	public async Task Charts_LastSevenDays_HasValidStructure()
	{
		var result = await ForOrganizationAsync(SolidtimeClient.Charts.GetLastSevenDaysAsync);

		Verify.DateDurationPoints(result, dataPoint => dataPoint.Date, dataPoint => dataPoint.Duration);
		foreach (var dataPoint in result)
		{
			dataPoint.History.Should().NotBeNull();
		}
	}
}
