namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing reports
/// </summary>
public interface IReports
{
	/// <summary>
	/// Gets all reports for an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="page">The page number (optional)</param>
	/// <param name="perPage">The number of items per page (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of reports</returns>
	[Get("/v1/organizations/{organization}/reports")]
	Task<PaginatedResponse<Report>> GetAsync(
		[AliasAs("organization")] string organizationId,
		[Query] int? page,
		[Query("per_page")] int? perPage,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets a specific report by ID
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="reportId">The report ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The report</returns>
	[Get("/v1/organizations/{organization}/reports/{report}")]
	Task<DataWrapper<Report>> GetByIdAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("report")] string reportId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Creates a new report
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="request">The report creation request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The created report</returns>
	[Post("/v1/organizations/{organization}/reports")]
	Task<DataWrapper<Report>> CreateAsync(
		[AliasAs("organization")] string organizationId,
		[Body] ReportStoreRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Updates an existing report
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="reportId">The report ID</param>
	/// <param name="request">The report update request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The updated report</returns>
	[Put("/v1/organizations/{organization}/reports/{report}")]
	Task<DataWrapper<Report>> UpdateAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("report")] string reportId,
		[Body] ReportUpdateRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Deletes a report
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="reportId">The report ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	[Delete("/v1/organizations/{organization}/reports/{report}")]
	Task DeleteAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("report")] string reportId,
		CancellationToken cancellationToken);
}
