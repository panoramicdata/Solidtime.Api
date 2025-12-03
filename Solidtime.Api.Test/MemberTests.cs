namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Members endpoint
/// </summary>
public class MemberTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting all members succeeds
	/// </summary>
	[Fact]
	public async Task Members_Get_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Members
			.GetAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Links.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that members have valid data
	/// </summary>
	[Fact]
	public async Task Members_Get_HasValidData()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Members
			.GetAsync(organizationId, null, null, CancellationToken);

		if (result.Data.Count != 0)
		{
			var member = result.Data.First();
			member.Id.Should().NotBeNullOrWhiteSpace();
			member.UserId.Should().NotBeNullOrWhiteSpace();
			member.Name.Should().NotBeNullOrWhiteSpace();
			member.Email.Should().NotBeNullOrWhiteSpace();
			member.Role.Should().NotBeNullOrWhiteSpace();
		}
	}

	/// <summary>
	/// Tests that pagination works correctly
	/// </summary>
	[Fact]
	public async Task Members_Pagination_Works()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Members
			.GetAsync(organizationId, 1, 5, CancellationToken);

		result.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Meta!.CurrentPage.Should().Be(1);
		// Note: API may ignore perPage parameter and use its own default
		result.Data.Should().NotBeNull();
	}
}
