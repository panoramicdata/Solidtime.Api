using Refit;
using Solidtime.Api.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for accessing chart data
/// </summary>
public interface ICharts
{
	/// <summary>
	/// Gets weekly hours chart data for an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="startDate">The start date for the chart (optional)</param>
	/// <param name="endDate">The end date for the chart (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Chart data points</returns>
	[Get("/v1/organizations/{organization}/charts/weekly-hours")]
	Task<List<ChartDataPoint>> GetWeeklyHoursAsync(
		[AliasAs("organization")] string organizationId,
		[Query("start_date")] string? startDate,
		[Query("end_date")] string? endDate,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets weekly project overview chart data for an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="startDate">The start date for the chart (optional)</param>
	/// <param name="endDate">The end date for the chart (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Chart data points</returns>
	[Get("/v1/organizations/{organization}/charts/weekly-project-overview")]
	Task<List<ChartDataPoint>> GetWeeklyProjectOverviewAsync(
		[AliasAs("organization")] string organizationId,
		[Query("start_date")] string? startDate,
		[Query("end_date")] string? endDate,
		CancellationToken cancellationToken);
}
