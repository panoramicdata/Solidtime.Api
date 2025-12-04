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
	/// <param name="archived">Filter by archived status: "true", "false", or "all" (optional)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A list of clients wrapped in a data wrapper</returns>
	/// <remarks>
	/// Note: The API does not support per_page parameter. Use page for pagination.
	/// </remarks>
	[Get("/v1/organizations/{organization}/clients")]
	Task<DataWrapper<List<Client>>> GetAsync(
		[AliasAs("organization")] string organizationId,
		[Query] int? page,
		[Query] string? archived,
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
	[Put("/v1/organizations/{organization}/clients/{client}")]
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
