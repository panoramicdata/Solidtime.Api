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
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a client succeeds
	/// </summary>
	[Fact]
	public async Task Clients_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();
		string? clientId = null;

		try
		{
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

			clientId = createResult.Data.Id;

			// Update name
			var updateRequest = new ClientUpdateRequest
			{
				Name = $"{Configuration.CrudClientName} Updated {Guid.NewGuid()}",
				IsArchived = false
			};

			var updateResult = await SolidtimeClient
				.Clients
				.UpdateAsync(organizationId, clientId, updateRequest, CancellationToken);

			updateResult.Should().NotBeNull();
			updateResult.Data.Id.Should().Be(clientId);
			updateResult.Data.Name.Should().Be(updateRequest.Name);

			// Archive (PUT requires name field)
			var archiveRequest = new ClientUpdateRequest
			{
				Name = updateResult.Data.Name,
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
		finally
		{
			// Ensure cleanup even if test fails
			if (clientId != null)
			{
				try
				{
					await SolidtimeClient
						.Clients
						.DeleteAsync(organizationId, clientId, CancellationToken);
				}
				catch
				{
					// Client may already be deleted, ignore errors
				}
			}
		}
	}

	/// <summary>
	/// Tests that archived filter works correctly
	/// </summary>
	[Fact]
	public async Task Clients_ArchivedFilter_Works()
	{
		var organizationId = await GetOrganizationIdAsync();

		// Get all clients (default, non-archived)
		var result = await SolidtimeClient
			.Clients
			.GetAsync(organizationId, 1, null, CancellationToken);

		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();

		// Get all clients including archived
		var allResult = await SolidtimeClient
			.Clients
			.GetAsync(organizationId, 1, "all", CancellationToken);

		allResult.Should().NotBeNull();
		allResult.Data.Should().NotBeNull();
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
