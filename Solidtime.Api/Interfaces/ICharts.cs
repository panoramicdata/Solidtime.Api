namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for accessing chart data
/// </summary>
/// <remarks>
/// Charts endpoints provide aggregated time tracking data for visualization.
/// All endpoints are scoped to an organization.
/// </remarks>
public interface ICharts
{
	/// <summary>
	/// Gets weekly project overview chart data for an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Chart data points showing project breakdown</returns>
	[Get("/v1/organizations/{organization}/charts/weekly-project-overview")]
	Task<List<ChartDataPoint>> GetWeeklyProjectOverviewAsync(
		[AliasAs("organization")] string organizationId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets weekly history chart data showing daily tracked hours
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Array of date/duration pairs</returns>
	[Get("/v1/organizations/{organization}/charts/weekly-history")]
	Task<List<WeeklyHistoryDataPoint>> GetWeeklyHistoryAsync(
		[AliasAs("organization")] string organizationId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets total weekly time in seconds
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Total seconds tracked this week</returns>
	[Get("/v1/organizations/{organization}/charts/total-weekly-time")]
	Task<int> GetTotalWeeklyTimeAsync(
		[AliasAs("organization")] string organizationId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets total weekly billable time in seconds
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Total billable seconds tracked this week</returns>
	[Get("/v1/organizations/{organization}/charts/total-weekly-billable-time")]
	Task<int> GetTotalWeeklyBillableTimeAsync(
		[AliasAs("organization")] string organizationId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets daily tracked hours data for the last 7 days
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Array of daily tracked hours data</returns>
	[Get("/v1/organizations/{organization}/charts/daily-tracked-hours")]
	Task<List<DailyTrackedHoursDataPoint>> GetDailyTrackedHoursAsync(
		[AliasAs("organization")] string organizationId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets last seven days chart data
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Array of chart data for last 7 days</returns>
	[Get("/v1/organizations/{organization}/charts/last-seven-days")]
	Task<List<LastSevenDaysDataPoint>> GetLastSevenDaysAsync(
		[AliasAs("organization")] string organizationId,
		CancellationToken cancellationToken);
}
