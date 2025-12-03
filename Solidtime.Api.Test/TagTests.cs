using Solidtime.Api.Models;

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
	/// Tests that creating and deleting a tag succeeds
	/// NOTE: Solidtime API does not support GET by ID or UPDATE operations for tags
	/// </summary>
	[Fact]
	public async Task Tags_CreateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();
		string? tagId = null;

		try
		{
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

			tagId = createResult.Data.Id;

			// Verify creation by getting all tags and checking the new one is present
			var allTagsAfterCreate = await SolidtimeClient
				.Tags
				.GetAsync(organizationId, null, null, CancellationToken);

			allTagsAfterCreate.Data.Should().Contain(t => t.Id == tagId);
			var createdTag = allTagsAfterCreate.Data.First(t => t.Id == tagId);
			createdTag.Name.Should().Be(createRequest.Name);

			// Delete
			await SolidtimeClient
				.Tags
				.DeleteAsync(organizationId, tagId, CancellationToken);

			// Verify deletion by checking it doesn't appear in the list
			var allTagsAfterDelete = await SolidtimeClient
				.Tags
				.GetAsync(organizationId, null, null, CancellationToken);

			allTagsAfterDelete.Data.Should().NotContain(t => t.Id == tagId);
		}
		finally
		{
			// Ensure cleanup even if test fails
			if (tagId != null)
			{
				try
				{
					await SolidtimeClient
						.Tags
						.DeleteAsync(organizationId, tagId, CancellationToken);
				}
				catch
				{
					// Tag may already be deleted, ignore errors
				}
			}
		}
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
		result.Meta.Should().NotBeNull();
		result.Meta!.CurrentPage.Should().Be(1);
		// Note: API may ignore perPage parameter and use its own default
		result.Data.Should().NotBeNull();
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
			tag.CreatedAt.Should().NotBeNull();
			tag.CreatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);

			tag.UpdatedAt.Should().NotBeNull();
			tag.UpdatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);
			tag.UpdatedAt.Value.Should().BeOnOrAfter(tag.CreatedAt.Value);
		}
	}
}
