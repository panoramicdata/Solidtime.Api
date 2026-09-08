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
		var result = await GetProjectMembersAsync("project members functionality");
		if (result is null)
		{
			return;
		}

		Verify.PaginatedEnvelope(result);
	}

	/// <summary>
	/// Tests that project members have valid data
	/// </summary>
	[Fact]
	public async Task ProjectMembers_Get_HasValidData()
	{
		var result = await GetProjectMembersAsync("project member data");
		if (result is null)
		{
			return;
		}

		if (result.Data.Count != 0)
		{
			var member = result.Data.First();
			member.Id.Should().NotBeNullOrWhiteSpace();
			member.UserId.Should().NotBeNullOrWhiteSpace();
			member.ProjectId.Should().NotBeNullOrWhiteSpace();
			Verify.Timestamps(member);
		}
	}

	/// <summary>
	/// Gets the members of the sample project, or null when the organization has no projects.
	/// </summary>
	/// <param name="whatCannotBeVerified">What the caller is unable to verify without a project</param>
	/// <returns>The project members, or null when no project is available</returns>
	private async Task<PaginatedResponse<ProjectMember>?> GetProjectMembersAsync(string whatCannotBeVerified)
	{
		var projectId = await TryGetProjectIdAsync(whatCannotBeVerified);
		if (projectId is null)
		{
			return null;
		}

		return await ForOrganizationAsync((organizationId, cancellationToken)
			=> SolidtimeClient.ProjectMembers.GetAsync(organizationId, projectId, cancellationToken));
	}
}
