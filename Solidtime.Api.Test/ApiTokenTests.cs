namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the API tokens endpoint
/// </summary>
public class ApiTokenTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting API tokens succeeds
	/// </summary>
	[Fact]
	public async Task ApiTokens_Get_Succeeds()
	{
		var result = await SolidtimeClient.ApiTokens.GetAsync(CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that creating and revoking an API token succeeds
	/// </summary>
	[Fact]
	public async Task ApiTokens_CreateAndRevoke_Succeeds()
	{
		var createRequest = new ApiTokenStoreRequest
		{
			Name = $"Test Token {Guid.NewGuid()}",
			ExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
		};

		await CreateTokenThenRevokeAsync(createRequest, async createResult =>
		{
			createResult.Data.Name.Should().Be(createRequest.Name);
			createResult.Data.Id.Should().NotBeNullOrWhiteSpace();

			// The token value should be in AccessToken (JWT format)
			createResult.Data.AccessToken.Should().NotBeNullOrWhiteSpace();

			// ExpiresAt should be set
			createResult.Data.ExpiresAt.Should().NotBeNull();

			var tokenId = createResult.Data.Id;
			await SolidtimeClient.ApiTokens.RevokeAsync(tokenId, CancellationToken);

			// Verify it was revoked by getting all tokens
			var allTokens = await SolidtimeClient.ApiTokens.GetAsync(CancellationToken);
			allTokens.Data.Should().NotContain(token => token.Id == tokenId);
		});
	}

	/// <summary>
	/// Tests that creating an API token without expiration succeeds
	/// </summary>
	[Fact]
	public async Task ApiTokens_CreateWithoutExpiration_Succeeds()
	{
		// No expiration is specified - the API provides a default
		var createRequest = new ApiTokenStoreRequest { Name = $"No Expiry Token {Guid.NewGuid()}" };

		await CreateTokenThenRevokeAsync(createRequest, createResult =>
		{
			createResult.Data.Name.Should().Be(createRequest.Name);

			// The API provides a default expiry even when none is specified
			createResult.Data.ExpiresAt.Should().NotBeNull();

			return Task.CompletedTask;
		});
	}

	/// <summary>
	/// Creates an API token, runs the given assertions against it, and revokes it afterwards
	/// however the assertions end. Revoking an already-revoked token is ignored.
	/// </summary>
	/// <param name="createRequest">The token to create</param>
	/// <param name="body">The assertions to run against the created token</param>
	private Task CreateTokenThenRevokeAsync(
		ApiTokenStoreRequest createRequest,
		Func<DataWrapper<ApiTokenCreated>, Task> body)
		=> CreateThenCleanupAsync(
			async () =>
			{
				var createResult = await SolidtimeClient.ApiTokens.CreateAsync(createRequest, CancellationToken);
				createResult.Should().NotBeNull();
				createResult.Data.Should().NotBeNull();
				return createResult;
			},
			created => SolidtimeClient.ApiTokens.RevokeAsync(created.Data.Id, CancellationToken),
			body);
}
