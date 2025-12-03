using Refit;
using Solidtime.Api.Models;
using System.Threading;
using System.Threading.Tasks;

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
	/// <param name="page">Page number for pagination (optional)</param>
	/// <param name="perPage">Number of items per page (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of time entries</returns>
	[Get("/v1/organizations/{organization}/time-entries")]
	Task<PaginatedResponse<TimeEntry>> GetAsync(
		[AliasAs("organization")] string organizationId,
		[Query] int? page,
		[Query("per_page")] int? perPage,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets a specific time entry by ID
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="timeEntryId">The time entry ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The time entry</returns>
	[Get("/v1/organizations/{organization}/time-entries/{timeEntry}")]
	Task<DataWrapper<TimeEntry>> GetByIdAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("timeEntry")] string timeEntryId,
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
	[Patch("/v1/organizations/{organization}/time-entries/{timeEntry}")]
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
