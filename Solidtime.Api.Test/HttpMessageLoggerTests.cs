using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Solidtime.Api.Test;

/// <summary>
/// Offline tests for the debug logging of requests and responses
/// </summary>
public class HttpMessageLoggerTests
{
	/// <summary>
	/// The token is redacted and the bodies are logged and still readable afterwards
	/// </summary>
	[Fact]
	public async Task Debug_LogsBodiesAndRedactsToken()
	{
		var log = new ListLogger();
		var stub = new StubHandler((_, _) =>
		{
			var response = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("{\"resp\":2}", Encoding.UTF8, "application/json")
			};
			response.Headers.TryAddWithoutValidation("X-Trace", "abc");
			return response;
		});
		using var handler = Handlers.Create(Handlers.Options(logger: log), stub);
		using var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/v1/x")
		{
			Content = new StringContent("{\"req\":1}", Encoding.UTF8, "application/json")
		};
		request.Headers.TryAddWithoutValidation("X-Custom", "custom-value");

		using var response = await Handlers.SendAsync(handler, request);
		var responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

		var all = string.Join('\n', log.Messages);
		all.Should().Contain("REDACTED").And.Contain("{\"req\":1}").And.Contain("{\"resp\":2}");
		all.Should().Contain("X-Custom: custom-value").And.Contain("X-Trace: abc").And.Contain("Status: 200");
		all.Should().NotContain("secret-token-value");
		responseBody.Should().Be("{\"resp\":2}");
	}

	/// <summary>
	/// A retried request is logged with its attempt number
	/// </summary>
	[Fact]
	public async Task Debug_LogsRetryAttempt()
	{
		var log = new ListLogger();
		var stub = new StubHandler((_, call) => call == 1
			? Handlers.TooManyRequests(r => r.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.Zero))
			: Handlers.Status(HttpStatusCode.OK));
		using var handler = Handlers.Create(Handlers.Options(logger: log), stub);

		using var response = await Handlers.SendAsync(handler, new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/v1/x"));

		log.Messages.Should().Contain(message => message.Contains("Retry attempt 1 of 3", StringComparison.Ordinal));
		log.Messages.Should().Contain(message => message.Contains("Rate limit exceeded", StringComparison.Ordinal));
	}
}
