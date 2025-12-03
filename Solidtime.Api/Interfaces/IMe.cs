using System.Threading;
using System.Threading.Tasks;
using Refit;
using Solidtime.Api.Models;

namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for accessing information about the currently authenticated user
/// </summary>
public interface IMe
{
	/// <summary>
	/// Gets information about the currently authenticated user
	/// </summary>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The current user's information</returns>
	[Get("/v1/me")]
	Task<DataWrapper<User>> GetAsync(
		CancellationToken cancellationToken);
}
