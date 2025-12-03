using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Solidtime.Api.Models;
using Xunit;

namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the ProjectMembers endpoint
/// </summary>
public class ProjectMemberTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting all project members succeeds
	/// </summary>
	[Fact]
	public async Task ProjectMembers_Get_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();
		var projectId = await GetProjectIdAsync();

		var result = await SolidtimeClient
			.ProjectMembers
			.GetAsync(organizationId, projectId, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that project members have valid data
	/// </summary>
	[Fact]
	public async Task ProjectMembers_Get_HasValidData()
	{
		var organizationId = await GetOrganizationIdAsync();
		var projectId = await GetProjectIdAsync();

		var result = await SolidtimeClient
			.ProjectMembers
			.GetAsync(organizationId, projectId, CancellationToken);

		if (result.Data.Count != 0)
		{
			var member = result.Data.First();
			member.Id.Should().NotBeNullOrWhiteSpace();
			member.UserId.Should().NotBeNullOrWhiteSpace();
			member.ProjectId.Should().Be(projectId);
			member.CreatedAt.Should().BeBefore(System.DateTimeOffset.UtcNow);
			member.UpdatedAt.Should().BeBefore(System.DateTimeOffset.UtcNow);
		}
	}
}
