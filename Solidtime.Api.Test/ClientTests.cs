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
		var result = await GetClientsAsync();

		Verify.PaginatedEnvelope(result);
	}

	/// <summary>
	/// Tests that creating, updating, and deleting a client succeeds
	/// </summary>
	[Fact]
	public async Task Clients_CreateUpdateDelete_Succeeds()
	{
		var organizationId = await GetOrganizationIdAsync();

		var createRequest = new ClientStoreRequest { Name = $"{Configuration.CrudClientName} {Guid.NewGuid()}" };

		await CreateThenCleanupAsync(
			() => SolidtimeClient.Clients.CreateAsync(organizationId, createRequest, CancellationToken),
			created => SolidtimeClient.Clients.DeleteAsync(organizationId, created.Data.Id, CancellationToken),
			async createResult =>
			{
				createResult.Should().NotBeNull();
				createResult.Data.Name.Should().Be(createRequest.Name);
				createResult.Data.Id.Should().NotBeNullOrWhiteSpace();
				createResult.Data.IsArchived.Should().BeFalse();

				var clientId = createResult.Data.Id;

				// Update
				var updateRequest = new ClientUpdateRequest { Name = $"{Configuration.CrudClientName} Updated {Guid.NewGuid()}", IsArchived = false };
				var updateResult = await SolidtimeClient.Clients.UpdateAsync(organizationId, clientId, updateRequest, CancellationToken);

				updateResult.Should().NotBeNull();
				updateResult.Data.Id.Should().Be(clientId);
				updateResult.Data.Name.Should().Be(updateRequest.Name);

				// Archive
				var archiveRequest = new ClientUpdateRequest { Name = updateResult.Data.Name, IsArchived = true };
				var archiveResult = await SolidtimeClient.Clients.UpdateAsync(organizationId, clientId, archiveRequest, CancellationToken);
				archiveResult.Data.IsArchived.Should().BeTrue();

				// Delete and verify
				await SolidtimeClient.Clients.DeleteAsync(organizationId, clientId, CancellationToken);
				var allClients = await GetClientsAsync();
				allClients.Data.Should().NotContain(client => client.Id == clientId);
			});
	}

	/// <summary>
	/// Tests that archived filter works correctly
	/// </summary>
	[Fact]
	public async Task Clients_ArchivedFilter_Works()
	{
		await VerifyClientListingAsync(archived: null);
		await VerifyClientListingAsync(archived: "all");
	}

	/// <summary>
	/// Tests that clients have valid timestamps
	/// </summary>
	[Fact]
	public async Task Clients_Get_HasValidTimestamps()
	{
		var result = await GetClientsAsync();

		if (result.Data.Count != 0)
		{
			Verify.Timestamps(result.Data.First());
		}
	}

	private async Task VerifyClientListingAsync(string? archived = null)
	{
		var result = await GetClientsAsync(page: 1, archived: archived);

		Verify.PaginatedEnvelope(result);
	}

	private Task<PaginatedResponse<Client>> GetClientsAsync(int? page = null, string? archived = null)
		=> ForOrganizationAsync((organizationId, cancellationToken)
			=> SolidtimeClient.Clients.GetAsync(organizationId, page, archived, cancellationToken));
}