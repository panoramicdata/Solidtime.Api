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
		var projectMembers = await GetProjectMembersAsync("project members functionality");
		if (projectMembers is null)
		{
			return;
		}

		Verify.PaginatedEnvelope(projectMembers.Value.Members);
	}

	/// <summary>
	/// Tests that project members have valid data
	/// </summary>
	[Fact]
	public async Task ProjectMembers_Get_HasValidData()
	{
		var projectMembers = await GetProjectMembersAsync("project member data");
		if (projectMembers is null)
		{
			return;
		}

		var (projectId, result) = projectMembers.Value;
		if (result.Data.Count != 0)
		{
			var member = result.Data.First();
			member.Id.Should().NotBeNullOrWhiteSpace();
			member.UserId.Should().NotBeNullOrWhiteSpace();
			member.ProjectId.Should().Be(projectId);
			Verify.Timestamps(member);
		}
	}

	/// <summary>
	/// Gets the sample project's ID and its members, or null when the organization has no projects.
	/// </summary>
	/// <param name="whatCannotBeVerified">What the caller is unable to verify without a project</param>
	/// <returns>The project ID and its members, or null when no project is available</returns>
	private async Task<(string ProjectId, PaginatedResponse<ProjectMember> Members)?> GetProjectMembersAsync(
		string whatCannotBeVerified)
	{
		var projectId = await TryGetProjectIdAsync(whatCannotBeVerified);
		if (projectId is null)
		{
			return null;
		}

		var members = await ForOrganizationAsync((organizationId, cancellationToken)
			=> SolidtimeClient.ProjectMembers.GetAsync(organizationId, projectId, cancellationToken));

		return (projectId, members);
	}
}
