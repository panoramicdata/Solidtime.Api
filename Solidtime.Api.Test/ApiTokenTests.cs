using AwesomeAssertions;
using Solidtime.Api.Models;
using System;
using System.Threading.Tasks;
using Xunit;

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
		createResult.Data.Token.Should().NotBeNullOrWhiteSpace();
		createResult.Data.Id.Should().BePositive();

		// Revoke the token
		await SolidtimeClient
			.ApiTokens
			.RevokeAsync(createResult.Data.Id, CancellationToken);

		// Verify it was deleted by getting all tokens
		var allTokens = await SolidtimeClient
			.ApiTokens
			.GetAsync(CancellationToken);

		allTokens.Data.Should().NotContain(t => t.Id == createResult.Data.Id);
	}

	/// <summary>
	/// Tests that creating an API token without expiration succeeds
	/// </summary>
	[Fact]
	public async Task ApiTokens_CreateWithoutExpiration_Succeeds()
	{
		// Create a token without expiration
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
		createResult.Data.ExpiresAt.Should().BeNull();

		// Clean up
		await SolidtimeClient
			.ApiTokens
			.RevokeAsync(createResult.Data.Id, CancellationToken);
	}
}
