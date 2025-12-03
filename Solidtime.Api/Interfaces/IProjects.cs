namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing projects
/// </summary>
public interface IProjects
{
	/// <summary>
	/// Gets all projects in an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="page">Page number for pagination (optional)</param>
	/// <param name="perPage">Number of items per page (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of projects</returns>
	[Get("/v1/organizations/{organization}/projects")]
	Task<PaginatedResponse<Project>> GetAsync(
		[AliasAs("organization")] string organizationId,
		[Query] int? page,
		[Query, AliasAs("per_page")] int? perPage,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets a specific project by ID
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="projectId">The project ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The project</returns>
	[Get("/v1/organizations/{organization}/projects/{project}")]
	Task<DataWrapper<Project>> GetByIdAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("project")] string projectId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Creates a new project
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="request">The project creation request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The created project</returns>
	[Post("/v1/organizations/{organization}/projects")]
	Task<DataWrapper<Project>> CreateAsync(
		[AliasAs("organization")] string organizationId,
		[Body] ProjectStoreRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Updates a project
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="projectId">The project ID</param>
	/// <param name="request">The project update request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The updated project</returns>
	[Put("/v1/organizations/{organization}/projects/{project}")]
	Task<DataWrapper<Project>> UpdateAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("project")] string projectId,
		[Body] ProjectUpdateRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Deletes a project
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="projectId">The project ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A task representing the async operation</returns>
	[Delete("/v1/organizations/{organization}/projects/{project}")]
	Task DeleteAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("project")] string projectId,
		CancellationToken cancellationToken);
}
