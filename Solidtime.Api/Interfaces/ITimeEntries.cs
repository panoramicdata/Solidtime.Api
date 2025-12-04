namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing time entries
/// </summary>
public interface ITimeEntries
{
	/// <summary>
	/// Gets all time entries in an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="limit">Limit the number of returned time entries (default: 150, max: 500)</param>
	/// <param name="offset">Skip the first n time entries (default: 0)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of time entries</returns>
	/// <remarks>
	/// Note: This endpoint uses limit/offset pagination, not page/perPage.
	/// The response includes a meta object with total count information.
	/// </remarks>
	[Get("/v1/organizations/{organization}/time-entries")]
	Task<PaginatedResponse<TimeEntry>> GetAsync(
		[AliasAs("organization")] string organizationId,
		[Query] int? limit,
		[Query] int? offset,
		CancellationToken cancellationToken);

	/// <summary>
	/// Creates a new time entry
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="request">The time entry creation request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The created time entry</returns>
	[Post("/v1/organizations/{organization}/time-entries")]
	Task<DataWrapper<TimeEntry>> CreateAsync(
		[AliasAs("organization")] string organizationId,
		[Body] TimeEntryStoreRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Updates a time entry
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="timeEntryId">The time entry ID</param>
	/// <param name="request">The time entry update request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The updated time entry</returns>
	[Put("/v1/organizations/{organization}/time-entries/{timeEntry}")]
	Task<DataWrapper<TimeEntry>> UpdateAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("timeEntry")] string timeEntryId,
		[Body] TimeEntryUpdateRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Deletes a time entry
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="timeEntryId">The time entry ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A task representing the async operation</returns>
	[Delete("/v1/organizations/{organization}/time-entries/{timeEntry}")]
	Task DeleteAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("timeEntry")] string timeEntryId,
		CancellationToken cancellationToken);
}
