namespace Solidtime.Api.Interfaces;

/// <summary>
/// Interface for managing imports from other time tracking systems
/// </summary>
public interface IImports
{
	/// <summary>
	/// Gets information about available importers
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A list of available importers</returns>
	[Get("/v1/organizations/{organization}/importers")]
	Task<DataWrapper<List<Importer>>> GetImportersAsync(
		[AliasAs("organization")] string organizationId,
		CancellationToken cancellationToken);

	/// <summary>
	/// Imports data into the organization
	/// </summary>
	/// <param name="organizationId">The organization ID</param>
	/// <param name="request">The import request containing the type and base64 encoded data</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The import report with counts of created entities</returns>
	[Post("/v1/organizations/{organization}/import")]
	Task<ImportResponse> ImportDataAsync(
		[AliasAs("organization")] string organizationId,
		[Body] ImportRequest request,
		CancellationToken cancellationToken);
}
