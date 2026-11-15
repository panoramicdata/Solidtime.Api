using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Solidtime.Api.Test;

/// <summary>
/// Offline tests for the authentication, read-only and rate-limit handling of the HTTP handler
/// </summary>
public class HandlerTests
{
	private static Uri Url => new("http://localhost/api/v1/ping");

	private static HttpResponseMessage RetryNowThenOk(int call)
		=> call < 3
			? Handlers.TooManyRequests(r => r.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.Zero))
			: Handlers.Status(HttpStatusCode.OK, "ok");

	private static async Task<(HttpStatusCode Status, int Calls)> SendGetAsync(Func<HttpRequestMessage, int, HttpResponseMessage> respond, bool isReadOnly = false, string method = "GET")
	{
		var stub = new StubHandler(respond);
		using var handler = Handlers.Create(Handlers.Options(isReadOnly), stub);

		using var response = await Handlers.SendAsync(handler, new HttpRequestMessage(new HttpMethod(method), Url));

		return (response.StatusCode, stub.Calls);
	}

	/// <summary>
	/// Every request carries the bearer token
	/// </summary>
	[Fact]
	public async Task Send_AddsBearerAuthorization()
	{
		string? seen = null;

		var (status, _) = await SendGetAsync((request, _) =>
		{
			seen = request.Headers.Authorization?.ToString();
			return Handlers.Status(HttpStatusCode.OK);
		});

		seen.Should().Be("Bearer secret-token-value");
		status.Should().Be(HttpStatusCode.OK);
	}

	/// <summary>
	/// A null options object is rejected
	/// </summary>
	[Fact]
	public void Constructor_NullOptions_Throws()
		=> new Action(() => _ = new AuthenticatedBackingOffHttpClientHandler(null!))
			.Should().Throw<ArgumentNullException>();

	/// <summary>
	/// Read-only mode refuses writes before they reach the inner handler
	/// </summary>
	[Theory]
	[InlineData("POST")]
	[InlineData("PUT")]
	[InlineData("DELETE")]
	[InlineData("PATCH")]
	public async Task ReadOnly_WriteMethods_AreRefused(string method)
	{
		var reached = false;

		var act = () => SendGetAsync((_, _) =>
		{
			reached = true;
			return Handlers.Status(HttpStatusCode.OK);
		}, isReadOnly: true, method: method);

		var thrown = await act.Should().ThrowAsync<InvalidOperationException>();
		thrown.Which.Message.Should().Contain("read-only").And.Contain(method);
		reached.Should().BeFalse();
	}

	/// <summary>
	/// Read-only mode lets the safe methods through
	/// </summary>
	[Theory]
	[InlineData("GET")]
	[InlineData("HEAD")]
	[InlineData("OPTIONS")]
	public async Task ReadOnly_SafeMethods_AreSent(string method)
	{
		var (status, calls) = await SendGetAsync((_, _) => Handlers.Status(HttpStatusCode.OK), isReadOnly: true, method: method);

		(status, calls).Should().Be((HttpStatusCode.OK, 1));
	}

	/// <summary>
	/// A 429 is retried and the retried request keeps its body
	/// </summary>
	[Fact]
	public async Task RateLimited_IsRetriedWithSameBody()
	{
		var stub = new StubHandler((_, call) => RetryNowThenOk(call));
		using var handler = Handlers.Create(Handlers.Options(), stub);
		using var request = new HttpRequestMessage(HttpMethod.Post, Url)
		{
			Content = new StringContent("{\"a\":1}", Encoding.UTF8, "application/json")
		};
		request.Headers.TryAddWithoutValidation("X-Custom", "v");

		using var response = await Handlers.SendAsync(handler, request);

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		stub.Calls.Should().Be(3);
		stub.Bodies.Should().OnlyContain(body => body == "{\"a\":1}");
	}

	/// <summary>
	/// After the retries are used up the last 429 is returned
	/// </summary>
	[Fact]
	public async Task RateLimited_GivesUpAfterMaxRetries()
	{
		var result = await SendGetAsync((_, _) => Handlers.TooManyRequests(r => r.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.Zero)));

		result.Should().Be((HttpStatusCode.TooManyRequests, 4));
	}

	/// <summary>
	/// Each way of hinting at a delay, or not hinting, ends in a successful retry
	/// </summary>
	[Theory]
	[InlineData("retry-after-date")]
	[InlineData("reset-header")]
	[InlineData("bad-reset-header")]
	public async Task RateLimited_DelayHints_AreHonoured(string hint)
	{
		var soon = DateTimeOffset.UtcNow.AddSeconds(2);
		var result = await SendGetAsync((_, call) => call > 1
			? Handlers.Status(HttpStatusCode.OK)
			: Handlers.TooManyRequests(r => ApplyHint(r, hint, soon)));

		result.Should().Be((HttpStatusCode.OK, 2));
	}

	/// <summary>
	/// Other error statuses are returned without a retry
	/// </summary>
	[Fact]
	public async Task ServerError_IsNotRetried()
	{
		var result = await SendGetAsync((_, _) => Handlers.Status(HttpStatusCode.InternalServerError));

		result.Should().Be((HttpStatusCode.InternalServerError, 1));
	}

	private static void ApplyHint(HttpResponseMessage response, string hint, DateTimeOffset soon)
	{
		switch (hint)
		{
			case "retry-after-date":
				response.Headers.RetryAfter = new RetryConditionHeaderValue(soon);
				break;
			case "reset-header":
				response.Headers.TryAddWithoutValidation("X-RateLimit-Reset", soon.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture));
				break;
			default:
				response.Headers.TryAddWithoutValidation("X-RateLimit-Reset", "not-a-number");
				break;
		}
	}
}
