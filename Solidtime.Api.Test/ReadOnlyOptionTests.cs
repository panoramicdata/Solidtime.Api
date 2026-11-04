namespace Solidtime.Api.Test;

/// <summary>
/// <see cref="SolidtimeClientOptions.IsReadOnly"/> needs no Solidtime account, token or configuration
/// </summary>
public class ReadOnlyOptionTests
{
	private static SolidtimeClient ClientFor(bool isReadOnly)
		=> new(new SolidtimeClientOptions
		{
			ApiToken = "not-a-real-token",
			BaseUrl = "http://127.0.0.1:1/api",
			TimeoutSeconds = 5,
			IsReadOnly = isReadOnly
		});

	private static bool IsRefused(Exception e)
		=> (e is InvalidOperationException || e.InnerException is InvalidOperationException)
		&& e.ToString().Contains("read-only", StringComparison.Ordinal);

	/// <summary>
	/// The option is off unless asked for
	/// </summary>
	[Fact]
	public void IsReadOnly_DefaultsToFalse()
		=> new SolidtimeClientOptions { ApiToken = "t" }.IsReadOnly.Should().BeFalse();

	/// <summary>
	/// A write is refused before any connection is made (nothing listens on the address)
	/// </summary>
	[Fact]
	public async Task ReadOnly_Create_IsRefusedBeforeSending()
	{
		using var client = ClientFor(isReadOnly: true);

		var act = () => client.Clients.CreateAsync("org", new ClientStoreRequest { Name = "x" }, TestContext.Current.CancellationToken);

		var thrown = await act.Should().ThrowAsync<Exception>();
		IsRefused(thrown.Which).Should().BeTrue();
	}

	/// <summary>
	/// With the option off the same write is attempted and fails on the missing server, not on the policy
	/// </summary>
	[Fact]
	public async Task NotReadOnly_Create_IsAttempted()
	{
		using var client = ClientFor(isReadOnly: false);

		var act = () => client.Clients.CreateAsync("org", new ClientStoreRequest { Name = "x" }, TestContext.Current.CancellationToken);

		var thrown = await act.Should().ThrowAsync<Exception>();
		IsRefused(thrown.Which).Should().BeFalse();
	}

	/// <summary>
	/// A read is not refused: it fails on the missing server, not on the policy
	/// </summary>
	[Fact]
	public async Task ReadOnly_Get_IsNotRefused()
	{
		using var client = ClientFor(isReadOnly: true);

		var act = () => client.Clients.GetAsync("org", null, null, TestContext.Current.CancellationToken);

		var thrown = await act.Should().ThrowAsync<Exception>();
		IsRefused(thrown.Which).Should().BeFalse();
	}
}
