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
			.GetAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		// Links may be null when the result set is empty
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
			.GetAsync(organizationId, CancellationToken);

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
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Members
			.GetAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		
		// Note: The Solidtime API returns paginated response structure but 
		// does not accept pagination query parameters
		if (result.Data.Count > 0)
		{
			result.Meta!.Total.Should().BeGreaterThanOrEqualTo(result.Data.Count);
		}
		
		result.Data.Should().NotBeNull();
	}
}
