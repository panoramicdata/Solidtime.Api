namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing imports from other time tracking systems
/// </summary>
public interface IImports
{
	/// <summary>
	/// Gets all imports for an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="page">The page number (optional)</param>
	/// <param name="perPage">The number of items per page (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of imports</returns>
	[Get("/v1/organizations/{organization}/imports")]
	Task<PaginatedResponse<Import>> GetAsync(
		[AliasAs("organization")] string organizationId,
		[Query] int? page,
		[Query("per_page")] int? perPage,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets a specific import by ID
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="importId">The import ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The import</returns>
	[Get("/v1/organizations/{organization}/imports/{import}")]
	Task<DataWrapper<Import>> GetByIdAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("import")] string importId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Creates a new import operation
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="request">The import creation request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The created import</returns>
	[Post("/v1/organizations/{organization}/imports")]
	Task<DataWrapper<Import>> CreateAsync(
		[AliasAs("organization")] string organizationId,
		[Body] ImportStoreRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Deletes an import
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="importId">The import ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	[Delete("/v1/organizations/{organization}/imports/{import}")]
	Task DeleteAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("import")] string importId,
		CancellationToken cancellationToken);
}
