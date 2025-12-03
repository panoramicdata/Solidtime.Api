using Refit;
using Solidtime.Api.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing tasks
/// </summary>
public interface ITasks
{
	/// <summary>
	/// Gets all tasks in an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="page">Page number for pagination (optional)</param>
	/// <param name="perPage">Number of items per page (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of tasks</returns>
	[Get("/v1/organizations/{organization}/tasks")]
	Task<PaginatedResponse<TaskModel>> GetAsync(
		[AliasAs("organization")] string organizationId,
		[Query] int? page,
		[Query("per_page")] int? perPage,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets a specific task by ID
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="taskId">The task ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The task</returns>
	[Get("/v1/organizations/{organization}/tasks/{task}")]
	Task<DataWrapper<TaskModel>> GetByIdAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("task")] string taskId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Creates a new task
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="request">The task creation request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The created task</returns>
	[Post("/v1/organizations/{organization}/tasks")]
	Task<DataWrapper<TaskModel>> CreateAsync(
		[AliasAs("organization")] string organizationId,
		[Body] TaskStoreRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Updates a task
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="taskId">The task ID</param>
	/// <param name="request">The task update request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The updated task</returns>
	[Patch("/v1/organizations/{organization}/tasks/{task}")]
	Task<DataWrapper<TaskModel>> UpdateAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("task")] string taskId,
		[Body] TaskUpdateRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Deletes a task
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="taskId">The task ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A task representing the async operation</returns>
	[Delete("/v1/organizations/{organization}/tasks/{task}")]
	Task DeleteAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("task")] string taskId,
		CancellationToken cancellationToken);
}
