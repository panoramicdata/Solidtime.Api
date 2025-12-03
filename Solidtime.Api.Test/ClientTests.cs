using Solidtime.Api.Models;

namespace Solidtime.Api.Test;

/// <summary>
/// Tests for the Clients endpoint
/// </summary>
public class ClientTests(ITestOutputHelper testOutputHelper, Fixture fixture)
	: SolidtimeTest(testOutputHelper, fixture)
{
	/// <summary>
	/// Tests that getting all clients succeeds
	/// </summary>
	[Fact]
	public async Task Clients_Get_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Clients
			.GetAsync(organizationId, null, null, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Links.Should().NotBeNull();
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a client succeeds
	/// </summary>
	[Fact]
	public async Task Clients_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		// Create
		var createRequest = new ClientStoreRequest
		{
			Name = $"{Configuration.CrudClientName} {Guid.NewGuid()}"
		};

		var createResult = await SolidtimeClient
			.Clients
			.CreateAsync(organizationId, createRequest, CancellationToken);

		createResult.Should().NotBeNull();
		createResult.Data.Should().NotBeNull();
		createResult.Data.Name.Should().Be(createRequest.Name);
		createResult.Data.Id.Should().NotBeNullOrWhiteSpace();
		createResult.Data.IsArchived.Should().BeFalse();
		createResult.Data.OrganizationId.Should().Be(organizationId);

		var clientId = createResult.Data.Id;

		// Get by ID
		var getResult = await SolidtimeClient
			.Clients
			.GetByIdAsync(organizationId, clientId, CancellationToken);

		getResult.Should().NotBeNull();
		getResult.Data.Id.Should().Be(clientId);
		getResult.Data.Name.Should().Be(createRequest.Name);

		// Update
		var updateRequest = new ClientUpdateRequest
		{
			Name = $"{Configuration.CrudClientName} Updated {Guid.NewGuid()}"
		};

		var updateResult = await SolidtimeClient
			.Clients
			.UpdateAsync(organizationId, clientId, updateRequest, CancellationToken);

		updateResult.Should().NotBeNull();
		updateResult.Data.Id.Should().Be(clientId);
		updateResult.Data.Name.Should().Be(updateRequest.Name);

		// Archive
		var archiveRequest = new ClientUpdateRequest
		{
			IsArchived = true
		};

		var archiveResult = await SolidtimeClient
			.Clients
			.UpdateAsync(organizationId, clientId, archiveRequest, CancellationToken);

		archiveResult.Data.IsArchived.Should().BeTrue();

		// Delete
		await SolidtimeClient
			.Clients
			.DeleteAsync(organizationId, clientId, CancellationToken);

		// Verify deletion by checking it doesn't appear in the list
		var allClients = await SolidtimeClient
			.Clients
			.GetAsync(organizationId, null, null, CancellationToken);

		allClients.Data.Should().NotContain(c => c.Id == clientId);
	}

	/// <summary>
	/// Tests that pagination works correctly
	/// </summary>
	[Fact]
	public async Task Clients_Pagination_Works()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Clients
			.GetAsync(organizationId, 1, 5, CancellationToken);

		result.Should().NotBeNull();
		result.Meta.Should().NotBeNull();
		result.Meta!.CurrentPage.Should().Be(1);
		result.Meta.PerPage.Should().Be(5);
		result.Data.Count.Should().BeLessThanOrEqualTo(5);
	}

	/// <summary>
	/// Tests that clients have valid timestamps
	/// </summary>
	[Fact]
	public async Task Clients_Get_HasValidTimestamps()
	{
		var organizationId = await GetOrganizationIdAsync();

		var result = await SolidtimeClient
			.Clients
			.GetAsync(organizationId, null, null, CancellationToken);

		if (result.Data.Count != 0)
		{
			var client = result.Data.First();
			client.CreatedAt.Should().NotBeNull();
			client.CreatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);
			client.UpdatedAt.Should().NotBeNull();
			client.UpdatedAt!.Value.Should().BeBefore(DateTimeOffset.UtcNow);
			client.UpdatedAt.Value.Should().BeOnOrAfter(client.CreatedAt.Value);
		}
	}
}
