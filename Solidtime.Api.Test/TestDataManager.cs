using System.Net;
using System.Threading;

namespace Solidtime.Api.Test;

/// <summary>
/// Manages test data setup and cleanup for integration tests
/// </summary>
public class TestDataManager(SolidtimeClient client, string organizationId, ILogger logger)
{
	private readonly SolidtimeClient _client = client;

	/// <summary>
	/// Sets up test data before test run
	/// Creates a sample client, tag, project, and time entries for tests to use
	/// </summary>
	public async Task SetupTestDataAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("Setting up test data for organization {OrganizationId}", organizationId);

		try
		{
			// Use timestamp to ensure unique names
			var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMdd-HHmmss", System.Globalization.CultureInfo.InvariantCulture);

			// Create a sample client for tests
			var clientRequest = new ClientStoreRequest
			{
				Name = $"TestClient-{timestamp}"
			};

			var clientResult = await _client.Clients.CreateAsync(organizationId, clientRequest, cancellationToken);
			SampleClientId = clientResult.Data.Id;
			logger.LogInformation("Created sample client: {ClientId}", SampleClientId);

			// Create a sample project for tests
			// Note: Using lowercase Material Design color (from user's color palette)
			var projectRequest = new ProjectStoreRequest
			{
				Name = $"TestProject-{timestamp}",
				Color = "#ef5350",  // Material Red 400 - confirmed working by user
				ClientId = SampleClientId,
				IsBillable = false
			};

			var projectResult = await _client.Projects.CreateAsync(organizationId, projectRequest, cancellationToken);
			SampleProjectId = projectResult.Data.Id;
			logger.LogInformation("Created sample project: {ProjectId}", SampleProjectId);

			// Create a sample tag for tests
			var tagRequest = new TagStoreRequest
			{
				Name = $"TestTag-{timestamp}"
			};

			var tagResult = await _client.Tags.CreateAsync(organizationId, tagRequest, cancellationToken);
			SampleTagId = tagResult.Data.Id;
			logger.LogInformation("Created sample tag: {TagId}", SampleTagId);

			// Create sample time entries for chart tests
			// Create entries over the last 4 weeks to ensure charts have data
			await CreateSampleTimeEntriesAsync(cancellationToken);

			logger.LogInformation("Test data setup completed successfully");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to setup test data");
			throw;
		}
	}

	/// <summary>
	/// Creates sample time entries for testing charts and reports
	/// Creates entries spread over the last 4 weeks
	/// </summary>
	private async Task CreateSampleTimeEntriesAsync(CancellationToken cancellationToken)
	{
		try
		{
			// Get the current user's member ID
			var members = await _client.Members.GetAsync(organizationId, cancellationToken);
			if (members.Data.Count == 0)
			{
				logger.LogWarning("No members found in organization - cannot create sample time entries");
				return;
			}

			var memberId = members.Data.First().Id;

			var now = DateTimeOffset.UtcNow;

			// Create 4 time entries spread over the last 4 weeks
			for (var weekOffset = 0; weekOffset < 4; weekOffset++)
			{
				var entryDate = now.AddDays(-7 * weekOffset);
				var startTime = new DateTimeOffset(
					entryDate.Year,
					entryDate.Month,
					entryDate.Day,
					9, 0, 0,
					TimeSpan.Zero);
				var endTime = startTime.AddHours(2); // 2-hour time entry

				var timeEntryRequest = new TimeEntryStoreRequest
				{
					MemberId = memberId,
					Start = startTime.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture),
					End = endTime.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture),
					Description = $"Test time entry for week {weekOffset + 1}",
					ProjectId = SampleProjectId,
					Tags = SampleTagId != null ? [SampleTagId] : null,
					Billable = false
				};

				await _client.TimeEntries.CreateAsync(organizationId, timeEntryRequest, cancellationToken);
				logger.LogDebug("Created sample time entry for {Date}", startTime);
			}

			logger.LogInformation("Created {Count} sample time entries", 4);
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to create sample time entries - charts may not have data");
			// Don't fail the setup - charts are optional
		}
	}

	/// <summary>
	/// Cleans up ALL test data from the organization
	/// This includes any data created during tests as well as setup data
	/// </summary>
	public async Task CleanupAllTestDataAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("Cleaning up all test data for organization {OrganizationId}", organizationId);

		try
		{
			// Clean up all time entries
			await CleanupAllTimeEntriesAsync(cancellationToken);

			// Clean up all tasks
			await CleanupAllTasksAsync(cancellationToken);

			// Clean up all tags
			await CleanupAllTagsAsync(cancellationToken);

			// Clean up all projects
			await CleanupAllProjectsAsync(cancellationToken);

			// Clean up all clients
			await CleanupAllClientsAsync(cancellationToken);

			logger.LogInformation("Test data cleanup completed");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to cleanup test data");
			// Don't throw - cleanup should be best effort
		}
	}

	private async Task CleanupAllTimeEntriesAsync(CancellationToken cancellationToken)
	{
		try
		{
			var timeEntries = await _client.TimeEntries.GetAsync(organizationId, null, null, cancellationToken);
			foreach (var entry in timeEntries.Data)
			{
				try
				{
					await _client.TimeEntries.DeleteAsync(organizationId, entry.Id, cancellationToken);
					logger.LogDebug("Deleted time entry: {TimeEntryId}", entry.Id);
				}
				catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
				{
					// Already deleted, ignore
					logger.LogDebug("Time entry {TimeEntryId} already deleted", entry.Id);
				}
				catch (Exception ex)
				{
					logger.LogWarning(ex, "Failed to delete time entry {TimeEntryId}", entry.Id);
				}
			}
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to retrieve time entries for cleanup");
		}
	}

	private async Task CleanupAllTasksAsync(CancellationToken cancellationToken)
	{
		try
		{
			var tasks = await _client.Tasks.GetAsync(organizationId, null, null, cancellationToken);
			foreach (var task in tasks.Data)
			{
				try
				{
					await _client.Tasks.DeleteAsync(organizationId, task.Id, cancellationToken);
					logger.LogDebug("Deleted task: {TaskId}", task.Id);
				}
				catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
				{
					// Already deleted, ignore
					logger.LogDebug("Task {TaskId} already deleted", task.Id);
				}
				catch (Exception ex)
				{
					logger.LogWarning(ex, "Failed to delete task {TaskId}", task.Id);
				}
			}
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to retrieve tasks for cleanup");
		}
	}

	private async Task CleanupAllTagsAsync(CancellationToken cancellationToken)
	{
		try
		{
			var tags = await _client.Tags.GetAsync(organizationId, cancellationToken);
			foreach (var tag in tags.Data)
			{
				try
				{
					await _client.Tags.DeleteAsync(organizationId, tag.Id, cancellationToken);
					logger.LogDebug("Deleted tag: {TagId}", tag.Id);
				}
				catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
				{
					// Already deleted, ignore
					logger.LogDebug("Tag {TagId} already deleted", tag.Id);
				}
				catch (Exception ex)
				{
					logger.LogWarning(ex, "Failed to delete tag {TagId}", tag.Id);
				}
			}
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to retrieve tags for cleanup");
		}
	}

	private async Task CleanupAllProjectsAsync(CancellationToken cancellationToken)
	{
		try
		{
			var projects = await _client.Projects.GetAsync(organizationId, null, null, cancellationToken);
			foreach (var project in projects.Data)
			{
				try
				{
					await _client.Projects.DeleteAsync(organizationId, project.Id, cancellationToken);
					logger.LogDebug("Deleted project: {ProjectId}", project.Id);
				}
				catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
				{
					// Already deleted, ignore
					logger.LogDebug("Project {ProjectId} already deleted", project.Id);
				}
				catch (Exception ex)
				{
					logger.LogWarning(ex, "Failed to delete project {ProjectId}", project.Id);
				}
			}
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to retrieve projects for cleanup");
		}
	}

	private async Task CleanupAllClientsAsync(CancellationToken cancellationToken)
	{
		try
		{
			var clients = await _client.Clients.GetAsync(organizationId, null, null, cancellationToken);
			foreach (var clientEntity in clients.Data)
			{
				try
				{
					await _client.Clients.DeleteAsync(organizationId, clientEntity.Id, cancellationToken);
					logger.LogDebug("Deleted client: {ClientId}", clientEntity.Id);
				}
				catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
				{
					// Already deleted, ignore
					logger.LogDebug("Client {ClientId} already deleted", clientEntity.Id);
				}
				catch (Exception ex)
				{
					logger.LogWarning(ex, "Failed to delete client {ClientId}", clientEntity.Id);
				}
			}
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to retrieve clients for cleanup");
		}
	}

	/// <summary>
	/// Sample client ID created during setup
	/// </summary>
	public string? SampleClientId { get; private set; }

	/// <summary>
	/// Sample project ID created during setup
	/// </summary>
	public string? SampleProjectId { get; private set; }

	/// <summary>
	/// Sample tag ID created during setup
	/// </summary>
	public string? SampleTagId { get; private set; }
}
