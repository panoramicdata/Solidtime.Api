using Solidtime.Api.Models;

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
		var result = await SolidtimeClient
			.ApiTokens
			.GetAsync(CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that creating and revoking an API token succeeds
	/// </summary>
	[Fact]
	public async Task ApiTokens_CreateAndRevoke_Succeeds()
	{
		string? tokenId = null;
		try
		{
			// Create a test token
			var createRequest = new ApiTokenStoreRequest
			{
				Name = $"Test Token {Guid.NewGuid()}",
				ExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
			};

			var createResult = await SolidtimeClient
				.ApiTokens
				.CreateAsync(createRequest, CancellationToken);

			createResult.Should().NotBeNull();
			createResult.Data.Should().NotBeNull();
			createResult.Data.Name.Should().Be(createRequest.Name);
			createResult.Data.Id.Should().NotBeNullOrWhiteSpace();

			// The token value should be in AccessToken (JWT format)
			createResult.Data.AccessToken.Should().NotBeNullOrWhiteSpace();

			// ExpiresAt should be set
			createResult.Data.ExpiresAt.Should().NotBeNull();

			tokenId = createResult.Data.Id;

			// Revoke the token
			await SolidtimeClient
				.ApiTokens
				.RevokeAsync(tokenId, CancellationToken);

			// Verify it was deleted by getting all tokens
			var allTokens = await SolidtimeClient
				.ApiTokens
				.GetAsync(CancellationToken);

			allTokens.Data.Should().NotContain(t => t.Id == tokenId);
		}
		finally
		{
			// Ensure cleanup even if test fails
			if (tokenId != null)
			{
				try
				{
					await SolidtimeClient
						.ApiTokens
						.RevokeAsync(tokenId, CancellationToken);
				}
				catch
				{
					// Token may already be revoked, ignore errors
				}
			}
		}
	}

	/// <summary>
	/// Tests that creating an API token without expiration succeeds
	/// </summary>
	[Fact]
	public async Task ApiTokens_CreateWithoutExpiration_Succeeds()
	{
		string? tokenId = null;
		try
		{
			// Create a token without expiration (API will provide a default)
			var createRequest = new ApiTokenStoreRequest
			{
				Name = $"No Expiry Token {Guid.NewGuid()}"
			};

			var createResult = await SolidtimeClient
				.ApiTokens
				.CreateAsync(createRequest, CancellationToken);

			createResult.Should().NotBeNull();
			createResult.Data.Should().NotBeNull();
			createResult.Data.Name.Should().Be(createRequest.Name);
			// API provides a default expiry even when none is specified
			createResult.Data.ExpiresAt.Should().NotBeNull();

			tokenId = createResult.Data.Id;

			// Clean up
			await SolidtimeClient
				.ApiTokens
				.RevokeAsync(tokenId, CancellationToken);
		}
		finally
		{
			// Ensure cleanup even if test fails
			if (tokenId != null)
			{
				try
				{
					await SolidtimeClient
						.ApiTokens
						.RevokeAsync(tokenId, CancellationToken);
				}
				catch
				{
					// Token may already be revoked, ignore errors
				}
			}
		}
	}
}
