namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Me endpoint
/// </summary>
public class MeTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting the current user information succeeds
	/// </summary>
	[Fact]
	public async Task Me_Get_Succeeds()
	{
		var result = await SolidtimeClient
			.Me
			.GetAsync(CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Data.Id.Should().NotBeNullOrWhiteSpace();
		result.Data.Name.Should().NotBeNullOrWhiteSpace();
		result.Data.Email.Should().NotBeNullOrWhiteSpace();
		result.Data.Timezone.Should().NotBeNullOrWhiteSpace();
		result.Data.WeekStart.Should().NotBeNullOrWhiteSpace();
	}

	/// <summary>
	/// Tests that the current user has valid timestamps if present
	/// Note: /v1/users/me endpoint does not return created_at/updated_at
	/// </summary>
	[Fact]
	public async Task Me_Get_TimestampsAreOptional()
	{
		var result = await SolidtimeClient
			.Me
			.GetAsync(CancellationToken);

		// The /me endpoint doesn't return timestamps, so they should be null
		// If they are present in other endpoints, they should be valid
		if (result.Data.CreatedAt.HasValue)
		{
			result.Data.CreatedAt.Value.Should().BeBefore(DateTimeOffset.UtcNow);
		}
		
		if (result.Data.UpdatedAt.HasValue)
		{
			result.Data.UpdatedAt.Value.Should().BeBefore(DateTimeOffset.UtcNow);
		}
		
		if (result.Data.CreatedAt.HasValue && result.Data.UpdatedAt.HasValue)
		{
			result.Data.UpdatedAt.Value.Should().BeOnOrAfter(result.Data.CreatedAt.Value);
		}
	}
}
