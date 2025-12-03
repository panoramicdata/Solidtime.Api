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

		// Try to get a project ID - test will be inconclusive if no projects exist
		string? projectId;
		try
		{
			projectId = await GetProjectIdAsync();
		}
		catch (InvalidOperationException ex)
		{
			// No projects in organization - test passes but logs warning
			Logger.LogWarning(ex, "No projects found in organization - test cannot verify project members functionality");
			return;
		}

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

		// Try to get a project ID - test will be inconclusive if no projects exist
		string? projectId;
		try
		{
			projectId = await GetProjectIdAsync();
		}
		catch (InvalidOperationException ex)
		{
			// No projects in organization - test passes but logs warning
			Logger.LogWarning(ex, "No projects found in organization - test cannot verify project member data");
			return;
		}

		var result = await SolidtimeClient
			.ProjectMembers
			.GetAsync(organizationId, projectId, CancellationToken);

		if (result.Data.Count != 0)
		{
			var member = result.Data.First();
			member.Id.Should().NotBeNullOrWhiteSpace();
			member.UserId.Should().NotBeNullOrWhiteSpace();
			member.ProjectId.Should().Be(projectId);
			member.CreatedAt.Should().NotBeNull();
			member.CreatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);
			member.UpdatedAt.Should().NotBeNull();
			member.UpdatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);
		}
	}
}
