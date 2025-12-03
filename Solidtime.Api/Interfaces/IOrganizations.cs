namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing organizations
/// </summary>
public interface IOrganizations
{
	/// <summary>
	/// Gets information about a specific organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The organization information</returns>
	[Get("/v1/organizations/{organization}")]
	Task<DataWrapper<Organization>> GetAsync(
		[AliasAs("organization")] string organizationId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Updates an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="request">The update request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The updated organization</returns>
	[Put("/v1/organizations/{organization}")]
	Task<DataWrapper<Organization>> UpdateAsync(
		[AliasAs("organization")] string organizationId,
		[Body] OrganizationUpdateRequest request,
		CancellationToken cancellationToken);
}
