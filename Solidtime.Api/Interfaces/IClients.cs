using Refit;
using Solidtime.Api.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing clients
/// </summary>
public interface IClients
{
	/// <summary>
	/// Gets all clients in an organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="page">Page number for pagination (optional)</param>
	/// <param name="perPage">Number of items per page (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A paginated list of clients</returns>
	[Get("/v1/organizations/{organization}/clients")]
	Task<PaginatedResponse<Client>> GetAsync(
		[AliasAs("organization")] string organizationId,
		[Query] int? page,
		[Query("per_page")] int? perPage,
		CancellationToken cancellationToken);

	/// <summary>
	/// Gets a specific client by ID
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="clientId">The client ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The client</returns>
	[Get("/v1/organizations/{organization}/clients/{client}")]
	Task<DataWrapper<Client>> GetByIdAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("client")] string clientId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Creates a new client
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="request">The client creation request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The created client</returns>
	[Post("/v1/organizations/{organization}/clients")]
	Task<DataWrapper<Client>> CreateAsync(
		[AliasAs("organization")] string organizationId,
		[Body] ClientStoreRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Updates a client
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="clientId">The client ID</param>
	/// <param name="request">The client update request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The updated client</returns>
	[Patch("/v1/organizations/{organization}/clients/{client}")]
	Task<DataWrapper<Client>> UpdateAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("client")] string clientId,
		[Body] ClientUpdateRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Deletes a client
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="clientId">The client ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A task representing the async operation</returns>
	[Delete("/v1/organizations/{organization}/clients/{client}")]
	Task DeleteAsync(
		[AliasAs("organization")] string organizationId,
		[AliasAs("client")] string clientId,
		CancellationToken cancellationToken);
}
