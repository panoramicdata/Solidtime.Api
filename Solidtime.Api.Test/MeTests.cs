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
		var result = await SolidtimeClient.Me.GetAsync(CancellationToken);

		result.Should().NotBeNull();
		result.Data.Id.Should().NotBeNullOrWhiteSpace();
		result.Data.Name.Should().NotBeNullOrWhiteSpace();
		result.Data.Email.Should().NotBeNullOrWhiteSpace();
		result.Data.Timezone.Should().NotBeNullOrWhiteSpace();
		result.Data.WeekStart.Should().NotBeNullOrWhiteSpace();
	}

	/// <summary>
	/// Tests that the current user has valid timestamps if present.
	/// The /v1/users/me endpoint does not return created_at/updated_at, so they are only
	/// checked when the API does supply them.
	/// </summary>
	[Fact]
	public async Task Me_Get_TimestampsAreOptional()
	{
		var result = await SolidtimeClient.Me.GetAsync(CancellationToken);

		Verify.OptionalTimestamps(result.Data);
	}
}
