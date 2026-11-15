namespace Solidtime.Api.Test;

/// <summary>
/// <see cref="SolidtimeClientOptions.IsReadOnly"/> needs no Solidtime account, token or configuration
/// </summary>
public class ReadOnlyOptionTests
{
	private static async Task<bool> IsRefusedAsync(bool isReadOnly, Func<SolidtimeClient, Task> call)
	{
		using var client = new SolidtimeClient(new SolidtimeClientOptions
		{
			ApiToken = "not-a-real-token",
			BaseUrl = "http://127.0.0.1:1/api",
			TimeoutSeconds = 5,
			IsReadOnly = isReadOnly
		});

		var thrown = await Assert.ThrowsAnyAsync<Exception>(() => call(client));
		return thrown.ToString().Contains("read-only", StringComparison.Ordinal);
	}

	/// <summary>
	/// The option is off unless asked for
	/// </summary>
	[Fact]
	public void IsReadOnly_DefaultsToFalse()
		=> new SolidtimeClientOptions { ApiToken = "t" }.IsReadOnly.Should().BeFalse();

	/// <summary>
	/// Only a write with the option on is refused by the policy; every other case fails on the missing server
	/// </summary>
	[Theory]
	[InlineData(true, true, true)]
	[InlineData(false, true, false)]
	[InlineData(true, false, false)]
	public async Task ReadOnly_RefusesOnlyWrites(bool isReadOnly, bool isWrite, bool expectedRefused)
	{
		var refused = await IsRefusedAsync(isReadOnly, isWrite ? client => client.Clients.CreateAsync("org", new ClientStoreRequest { Name = "x" }, TestContext.Current.CancellationToken) : client => client.Clients.GetAsync("org", null, null, TestContext.Current.CancellationToken));

		refused.Should().Be(expectedRefused);
	}
}
