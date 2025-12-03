using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;

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
	}

	/// <summary>
	/// Tests that the current user has valid timestamps
	/// </summary>
	[Fact]
	public async Task Me_Get_HasValidTimestamps()
	{
		var result = await SolidtimeClient
			.Me
			.GetAsync(CancellationToken);

		result.Data.CreatedAt.Should().BeBefore(System.DateTimeOffset.UtcNow);
		result.Data.UpdatedAt.Should().BeBefore(System.DateTimeOffset.UtcNow);
		result.Data.UpdatedAt.Should().BeOnOrAfter(result.Data.CreatedAt);
	}
}
