using Solidtime.Api.Models;

namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Organizations endpoint
/// </summary>
public class OrganizationTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting an organization succeeds
	/// </summary>
	[Fact]
	public async Task Organizations_Get_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Organizations
			.GetAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Data.Id.Should().Be(organizationId);
		result.Data.Name.Should().NotBeNullOrWhiteSpace();
		result.Data.Currency.Should().NotBeNullOrWhiteSpace();
	}

	/// <summary>
	/// Tests that updating an organization succeeds
	/// </summary>
	[Fact]
	public async Task Organizations_Update_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		// Get current organization data
		var original = await SolidtimeClient
			.Organizations
			.GetAsync(organizationId, CancellationToken);

		// Update with the same name (idempotent operation)
		var updateRequest = new OrganizationUpdateRequest
		{
			Name = original.Data.Name
		};

		var result = await SolidtimeClient
			.Organizations
			.UpdateAsync(organizationId, updateRequest, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Data.Id.Should().Be(organizationId);
		result.Data.Name.Should().Be(updateRequest.Name);
	}

	/// <summary>
	/// Tests that organization has valid timestamps
	/// </summary>
	[Fact]
	public async Task Organizations_Get_HasValidTimestamps()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Organizations
			.GetAsync(organizationId, CancellationToken);

		result.Data.CreatedAt.Should().NotBeNull();
		result.Data.CreatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);
		
		result.Data.UpdatedAt.Should().NotBeNull();
		result.Data.UpdatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);
		result.Data.UpdatedAt.Value.Should().BeOnOrAfter(result.Data.CreatedAt.Value);
	}
}
