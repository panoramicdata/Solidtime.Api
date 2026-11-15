using System.Net;
using System.Net.Http;

namespace Solidtime.Api.Test;

/// <summary>
/// An inner handler that answers from a delegate and counts the requests it receives
/// </summary>
internal sealed class StubHandler(Func<HttpRequestMessage, int, HttpResponseMessage> respond) : HttpMessageHandler
{
	/// <summary>
	/// The number of requests received so far
	/// </summary>
	public int Calls { get; private set; }

	/// <summary>
	/// The request bodies received, in order
	/// </summary>
	public List<string> Bodies { get; } = [];

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		Calls++;
		Bodies.Add(request.Content is null ? string.Empty : await request.Content.ReadAsStringAsync(cancellationToken));
		return respond(request, Calls);
	}
}

/// <summary>
/// A logger that keeps every formatted message
/// </summary>
internal sealed class ListLogger : ILogger
{
	/// <summary>
	/// The messages logged so far
	/// </summary>
	public List<string> Messages { get; } = [];

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

	public bool IsEnabled(LogLevel logLevel) => true;

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		=> Messages.Add(formatter(state, exception));
}

/// <summary>
/// Builds handlers wired to stubs
/// </summary>
internal static class Handlers
{
	internal static AuthenticatedBackingOffHttpClientHandler Create(SolidtimeClientOptions options, HttpMessageHandler inner)
		=> new(options) { InnerHandler = inner };

	internal static SolidtimeClientOptions Options(bool isReadOnly = false, ILogger? logger = null)
		=> new()
		{
			ApiToken = "secret-token-value",
			IsReadOnly = isReadOnly,
			Logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance
		};

	internal static HttpResponseMessage Status(HttpStatusCode code, string body = "")
		=> new(code) { Content = new StringContent(body) };

	internal static HttpResponseMessage TooManyRequests(Action<HttpResponseMessage> configure)
	{
		var response = Status(HttpStatusCode.TooManyRequests);
		configure(response);
		return response;
	}

	internal static Task<HttpResponseMessage> SendAsync(HttpMessageHandler handler, HttpRequestMessage request)
	{
		using var invoker = new HttpMessageInvoker(handler, disposeHandler: false);
		return invoker.SendAsync(request, TestContext.Current.CancellationToken);
	}
}
