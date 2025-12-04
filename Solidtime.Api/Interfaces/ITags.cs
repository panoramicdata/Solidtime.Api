namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing tags
/// </summary>
/// <remarks>
/// NOTE: The Solidtime API returns all tags in a single request - no pagination is supported.
/// </remarks>
public interface ITags
{
	/// <summary>
	/// Gets all tags in an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of tags</returns>
	/// <remarks>
	/// Note: This endpoint does not support pagination. All tags are returned in a single request.
	/// </remarks>
	[Get("/v1/organizations/{organization}/tags")]
	Task<PaginatedResponse<Tag>> GetAsync(
		[AliasAs("organization")] string organizationId,
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
	/// Updates a tag
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="tagId">The tag ID</param>
	/// <param name="request">The tag update request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The updated tag</returns>
	[Put("/v1/organizations/{organization}/tags/{tag}")]
	Task<DataWrapper<Tag>> UpdateAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("tag")] string tagId,
		[Body] TagUpdateRequest request,
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
