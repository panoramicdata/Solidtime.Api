using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Solidtime.Api.Models;

namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing API tokens (Personal Access Tokens)
/// </summary>
public interface IApiTokens
{
	/// <summary>
	/// Gets all API tokens for the current user
	/// </summary>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A list of API tokens</returns>
	[Get("/v1/users/me/api-tokens")]
	Task<DataWrapper<List<ApiToken>>> GetAsync(
		CancellationToken cancellationToken);

	/// <summary>
	/// Creates a new API token
	/// </summary>
	/// <param name="request">The API token creation request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The created API token with its token value (only shown once)</returns>
	[Post("/v1/users/me/api-tokens")]
	Task<DataWrapper<ApiTokenCreated>> CreateAsync(
		[Body] ApiTokenStoreRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Revokes (deletes) an API token
	/// </summary>
	/// <param name="tokenId">The ID of the token to revoke</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A task representing the async operation</returns>
	[Delete("/v1/users/me/api-tokens/{tokenId}")]
	Task RevokeAsync(
		[AliasAs("tokenId")] int tokenId,
		CancellationToken cancellationToken);
}
