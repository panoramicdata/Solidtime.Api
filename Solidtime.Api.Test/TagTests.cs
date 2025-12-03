using AwesomeAssertions;
using Solidtime.Api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Tags endpoint
/// </summary>
public class TagTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting all tags succeeds
	/// </summary>
	[Fact]
	public async Task Tags_Get_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Tags
			.GetAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Links.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a tag succeeds
	/// </summary>
	[Fact]
	public async Task Tags_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		// Create
		var createRequest = new TagStoreRequest
		{
			Name = $"Test Tag {Guid.NewGuid()}"
		};

		var createResult = await SolidtimeClient
			.Tags
			.CreateAsync(organizationId, createRequest, CancellationToken);

		createResult.Should().NotBeNull();
		createResult.Data.Should().NotBeNull();
		createResult.Data.Name.Should().Be(createRequest.Name);
		createResult.Data.Id.Should().NotBeNullOrWhiteSpace();
		createResult.Data.OrganizationId.Should().Be(organizationId);

		var tagId = createResult.Data.Id;

		// Get by ID
		var getResult = await SolidtimeClient
			.Tags
			.GetByIdAsync(organizationId, tagId, CancellationToken);

		getResult.Should().NotBeNull();
		getResult.Data.Id.Should().Be(tagId);
		getResult.Data.Name.Should().Be(createRequest.Name);

		// Update
		var updateRequest = new TagUpdateRequest
		{
			Name = $"Updated Tag {Guid.NewGuid()}"
		};

		var updateResult = await SolidtimeClient
			.Tags
			.UpdateAsync(organizationId, tagId, updateRequest, CancellationToken);

		updateResult.Should().NotBeNull();
		updateResult.Data.Id.Should().Be(tagId);
		updateResult.Data.Name.Should().Be(updateRequest.Name);

		// Delete
		await SolidtimeClient
			.Tags
			.DeleteAsync(organizationId, tagId, CancellationToken);

		// Verify deletion by checking it doesn't appear in the list
		var allTags = await SolidtimeClient
			.Tags
			.GetAsync(organizationId, null, null, CancellationToken);

		allTags.Data.Should().NotContain(t => t.Id == tagId);
	}

	/// <summary>
	/// Tests that pagination works correctly
	/// </summary>
	[Fact]
	public async Task Tags_Pagination_Works()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Tags
			.GetAsync(organizationId, 1, 5, CancellationToken);

		result.Should().NotBeNull();
		result.Meta.CurrentPage.Should().Be(1);
		result.Meta.PerPage.Should().Be(5);
		result.Data.Count.Should().BeLessThanOrEqualTo(5);
	}

	/// <summary>
	/// Tests that tags have valid timestamps
	/// </summary>
	[Fact]
	public async Task Tags_Get_HasValidTimestamps()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Tags
			.GetAsync(organizationId, null, null, CancellationToken);

		if (result.Data.Count != 0)
		{
			var tag = result.Data.First();
			tag.CreatedAt.Should().BeBefore(DateTimeOffset.UtcNow);
			tag.UpdatedAt.Should().BeBefore(DateTimeOffset.UtcNow);
			tag.UpdatedAt.Should().BeOnOrAfter(tag.CreatedAt);
		}
	}
}
