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
		var result = await GetTagsAsync();

		Verify.PaginatedEnvelope(result);
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a tag succeeds
	/// </summary>
	[Fact]
	public async Task Tags_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var createRequest = new TagStoreRequest { Name = $"Test Tag {Guid.NewGuid()}" };

		await CreateThenCleanupAsync(
			() => SolidtimeClient.Tags.CreateAsync(organizationId, createRequest, CancellationToken),
			created => SolidtimeClient.Tags.DeleteAsync(organizationId, created.Data.Id, CancellationToken),
			async createResult =>
			{
				createResult.Should().NotBeNull();
				createResult.Data.Name.Should().Be(createRequest.Name);
				createResult.Data.Id.Should().NotBeNullOrWhiteSpace();

				var tagId = createResult.Data.Id;

				// Verify creation
				await AssertTagExistsAsync(tagId, createRequest.Name);

				// Update
				var updateRequest = new TagUpdateRequest { Name = $"Updated Tag {Guid.NewGuid()}" };
				var updateResult = await SolidtimeClient.Tags.UpdateAsync(organizationId, tagId, updateRequest, CancellationToken);

				updateResult.Should().NotBeNull();
				updateResult.Data.Name.Should().Be(updateRequest.Name);

				// Delete and verify
				await SolidtimeClient.Tags.DeleteAsync(organizationId, tagId, CancellationToken);
				await AssertTagMissingAsync(tagId);
			});
	}

	/// <summary>
	/// Tests that tags have valid timestamps
	/// </summary>
	[Fact]
	public async Task Tags_Get_HasValidTimestamps()
	{
		var result = await GetTagsAsync();

		if (result.Data.Count != 0)
		{
			Verify.Timestamps(result.Data.First());
		}
	}

	private Task<PaginatedResponse<Tag>> GetTagsAsync()
		=> ForOrganizationAsync(SolidtimeClient.Tags.GetAsync);

	/// <summary>
	/// Asserts that the tag with the given id is present with the given name
	/// </summary>
	private async Task AssertTagExistsAsync(string tagId, string expectedName)
	{
		var allTags = await GetTagsAsync();
		allTags.Data.Should().Contain(tag => tag.Id == tagId);
		allTags.Data.First(tag => tag.Id == tagId).Name.Should().Be(expectedName);
	}

	/// <summary>
	/// Asserts that the tag with the given id is no longer present
	/// </summary>
	private async Task AssertTagMissingAsync(string tagId)
	{
		var afterDelete = await GetTagsAsync();
		afterDelete.Data.Should().NotContain(tag => tag.Id == tagId);
	}
}