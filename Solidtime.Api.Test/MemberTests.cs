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
		var result = await ForOrganizationAsync(SolidtimeClient.Members.GetAsync);

		Verify.PaginatedEnvelopeWithMeta(result);
	}

	/// <summary>
	/// Tests that members have valid data
	/// </summary>
	[Fact]
	public async Task Members_Get_HasValidData()
	{
		var result = await ForOrganizationAsync(SolidtimeClient.Members.GetAsync);

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
	/// Tests that the paginated response contains proper metadata
	/// </summary>
	[Fact]
	public async Task Members_Get_HasPaginationMetadata()
	{
		var result = await ForOrganizationAsync(SolidtimeClient.Members.GetAsync);

		Verify.PaginatedEnvelopeWithMeta(result);

		// Note: The Solidtime API returns paginated response structure but
		// does not accept pagination query parameters
		if (result.Data.Count > 0)
		{
			result.Meta!.Total.Should().BeGreaterThanOrEqualTo(result.Data.Count);
		}
	}
}
