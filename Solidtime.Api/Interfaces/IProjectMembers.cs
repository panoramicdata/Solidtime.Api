using System.Threading;
using System.Threading.Tasks;
using Refit;
using Solidtime.Api.Models;

namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing project members
/// </summary>
public interface IProjectMembers
{
	/// <summary>
	/// Gets all members of a project
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="projectId">The project ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of project members</returns>
	[Get("/v1/organizations/{organization}/projects/{project}/project-members")]
	Task<PaginatedResponse<ProjectMember>> GetAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("project")] string projectId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Adds a member to a project
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="projectId">The project ID</param>
	/// <param name="request">The project member creation request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The created project member</returns>
	[Post("/v1/organizations/{organization}/projects/{project}/project-members")]
	Task<DataWrapper<ProjectMember>> CreateAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("project")] string projectId,
		[Body] ProjectMemberStoreRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Updates a project member
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="projectId">The project ID</param>
	/// <param name="memberId">The project member ID</param>
	/// <param name="request">The project member update request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The updated project member</returns>
	[Patch("/v1/organizations/{organization}/projects/{project}/project-members/{member}")]
	Task<DataWrapper<ProjectMember>> UpdateAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("project")] string projectId,
		[AliasAs("member")] string memberId,
		[Body] ProjectMemberUpdateRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Removes a member from a project
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="projectId">The project ID</param>
	/// <param name="memberId">The project member ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A task representing the async operation</returns>
	[Delete("/v1/organizations/{organization}/projects/{project}/project-members/{member}")]
	Task DeleteAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("project")] string projectId,
		[AliasAs("member")] string memberId,
		CancellationToken cancellationToken);
}
