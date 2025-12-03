namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for accessing chart data
/// </summary>
/// <remarks>
/// NOTE: Charts endpoints may return 404 if the feature is not enabled/available in the organization.
/// This may be a premium feature or require specific Solidtime instance configuration.
/// Query parameters use snake_case naming (start_date, end_date) via [Query, AliasAs] attributes.
/// </remarks>
public interface ICharts
{
	/// <summary>
	/// Gets weekly hours chart data for an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="startDate">The start date for the chart (optional, format: yyyy-MM-dd)</param>
	/// <param name="endDate">The end date for the chart (optional, format: yyyy-MM-dd)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Chart data points</returns>
	[Get("/v1/organizations/{organization}/charts/weekly-hours")]
	Task<List<ChartDataPoint>> GetWeeklyHoursAsync(
		[AliasAs("organization")] string organizationId,
		[Query, AliasAs("start_date")] string? startDate,
		[Query, AliasAs("end_date")] string? endDate,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets weekly project overview chart data for an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="startDate">The start date for the chart (optional, format: yyyy-MM-dd)</param>
	/// <param name="endDate">The end date for the chart (optional, format: yyyy-MM-dd)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Chart data points</returns>
	[Get("/v1/organizations/{organization}/charts/weekly-project-overview")]
	Task<List<ChartDataPoint>> GetWeeklyProjectOverviewAsync(
		[AliasAs("organization")] string organizationId,
		[Query, AliasAs("start_date")] string? startDate,
		[Query, AliasAs("end_date")] string? endDate,
		CancellationToken cancellationToken);
}
