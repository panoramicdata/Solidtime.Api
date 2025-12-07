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
			.GetAsync(organizationId, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a tag succeeds
	/// </summary>
	[Fact]
	public async Task Tags_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();
		string? tagId = null;

		try
		{
			// Create
			var createRequest = new TagStoreRequest { Name = $"Test Tag {Guid.NewGuid()}" };
			var createResult = await SolidtimeClient.Tags.CreateAsync(organizationId, createRequest, CancellationToken);

			createResult.Should().NotBeNull();
			createResult.Data.Name.Should().Be(createRequest.Name);
			createResult.Data.Id.Should().NotBeNullOrWhiteSpace();
			tagId = createResult.Data.Id;

			// Verify creation
			var allTags = await SolidtimeClient.Tags.GetAsync(organizationId, CancellationToken);
			allTags.Data.Should().Contain(t => t.Id == tagId);
			allTags.Data.First(t => t.Id == tagId).Name.Should().Be(createRequest.Name);

			// Update
			var updateRequest = new TagUpdateRequest { Name = $"Updated Tag {Guid.NewGuid()}" };
			var updateResult = await SolidtimeClient.Tags.UpdateAsync(organizationId, tagId, updateRequest, CancellationToken);

			updateResult.Should().NotBeNull();
			updateResult.Data.Name.Should().Be(updateRequest.Name);

			// Delete and verify
			await SolidtimeClient.Tags.DeleteAsync(organizationId, tagId, CancellationToken);
			var afterDelete = await SolidtimeClient.Tags.GetAsync(organizationId, CancellationToken);
			afterDelete.Data.Should().NotContain(t => t.Id == tagId);
		}
		finally
		{
			if (tagId != null)
				await SafeDeleteAsync(() => SolidtimeClient.Tags.DeleteAsync(organizationId, tagId, CancellationToken));
		}
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
			.GetAsync(organizationId, CancellationToken);

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
