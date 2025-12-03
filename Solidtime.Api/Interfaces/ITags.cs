using Refit;
using Solidtime.Api.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing tags
/// </summary>
/// <remarks>
/// NOTE: The Solidtime API has limited support for tag operations.
/// Only List, Create, and Delete operations are currently supported.
/// Get by ID and Update operations return 404/405 errors.
/// </remarks>
public interface ITags
{
	/// <summary>
	/// Gets all tags in an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="page">Page number for pagination (optional)</param>
	/// <param name="perPage">Number of items per page (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of tags</returns>
	[Get("/v1/organizations/{organization}/tags")]
	Task<PaginatedResponse<Tag>> GetAsync(
		[AliasAs("organization")] string organizationId,
		[Query] int? page,
		[Query("per_page")] int? perPage,
		CancellationToken cancellationToken);

	/// <summary>
	/// Creates a new tag
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="request">The tag creation request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The created tag</returns>
	[Post("/v1/organizations/{organization}/tags")]
	Task<DataWrapper<Tag>> CreateAsync(
		[AliasAs("organization")] string organizationId,
		[Body] TagStoreRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Deletes a tag
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="tagId">The tag ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A task representing the async operation</returns>
	[Delete("/v1/organizations/{organization}/tags/{tag}")]
	Task DeleteAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("tag")] string tagId,
		CancellationToken cancellationToken);
}
