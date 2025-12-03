using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Solidtime.Api;

/// <summary>
/// HTTP client handler that adds Bearer token authentication to requests
/// </summary>
public class AuthenticatedHttpClientHandler : DelegatingHandler
{
	private readonly SolidtimeClientOptions _options;

	/// <summary>
	/// Initializes a new instance of the <see cref="AuthenticatedHttpClientHandler"/> class
	/// </summary>
	/// <param name="options">The client options containing the API token</param>
	public AuthenticatedHttpClientHandler(SolidtimeClientOptions options)
		: base(new HttpClientHandler())
	{
		ArgumentNullException.ThrowIfNull(options);
		_options = options;
	}

	/// <summary>
	/// Sends an HTTP request with Bearer token authentication
	/// </summary>
	/// <param name="request">The HTTP request message</param>
	/// <param name="cancellationToken">A cancellation token</param>
	/// <returns>The HTTP response message</returns>
	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		// Add Bearer token authentication
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);

		return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
	}
}
