namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing organization members
/// </summary>
public interface IMembers
{
	/// <summary>
	/// Gets all members in an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of members</returns>
	/// <remarks>
	/// Note: The API returns paginated results but does not accept pagination query parameters.
	/// All members are returned in a single request.
	/// </remarks>
	[Get("/v1/organizations/{organization}/members")]
	Task<PaginatedResponse<Member>> GetAsync(
		[AliasAs("organization")] string organizationId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Updates an organization member
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="memberId">The member ID</param>
	/// <param name="request">The member update request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The updated member</returns>
	[Put("/v1/organizations/{organization}/members/{member}")]
	Task<DataWrapper<Member>> UpdateAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("member")] string memberId,
		[Body] MemberUpdateRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Removes a member from an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="memberId">The member ID</param>
	/// <param name="deleteRelated">Whether to delete related time entries: "true" or "false" (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A task representing the async operation</returns>
	[Delete("/v1/organizations/{organization}/members/{member}")]
	Task DeleteAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("member")] string memberId,
		[Query, AliasAs("delete_related")] string? deleteRelated,
		CancellationToken cancellationToken);

	/// <summary>
	/// Invites a placeholder member to become a real member
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="memberId">The member ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A task representing the async operation</returns>
	[Post("/v1/organizations/{organization}/members/{member}/invite-placeholder")]
	Task InvitePlaceholderAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("member")] string memberId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Makes a member a placeholder member
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="memberId">The member ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A task representing the async operation</returns>
	[Post("/v1/organizations/{organization}/members/{member}/make-placeholder")]
	Task MakePlaceholderAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("member")] string memberId,
		CancellationToken cancellationToken);
}
