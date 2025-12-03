using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Solidtime.Api.Models;
using Xunit;

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
			member.OrganizationId.Should().Be(organizationId);
			member.Role.Should().NotBeNullOrWhiteSpace();
			member.CreatedAt.Should().BeBefore(System.DateTimeOffset.UtcNow);
			member.UpdatedAt.Should().BeBefore(System.DateTimeOffset.UtcNow);
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
		result.Meta.CurrentPage.Should().Be(1);
		result.Meta.PerPage.Should().Be(5);
		result.Data.Count.Should().BeLessThanOrEqualTo(5);
	}
}
