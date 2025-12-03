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
	/// <param name="page">Page number for pagination (optional)</param>
	/// <param name="perPage">Number of items per page (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of members</returns>
	[Get("/v1/organizations/{organization}/members")]
	Task<PaginatedResponse<Member>> GetAsync(
		[AliasAs("organization")] string organizationId,
		[Query] int? page,
		[Query, AliasAs("per_page")] int? perPage,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets a specific member by ID
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="memberId">The member ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The member</returns>
	[Get("/v1/organizations/{organization}/members/{member}")]
	Task<DataWrapper<Member>> GetByIdAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("member")] string memberId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Updates an organization member
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="memberId">The member ID</param>
	/// <param name="request">The member update request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The updated member</returns>
	[Patch("/v1/organizations/{organization}/members/{member}")]
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
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A task representing the async operation</returns>
	[Delete("/v1/organizations/{organization}/members/{member}")]
	Task DeleteAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("member")] string memberId,
		CancellationToken cancellationToken);
}
