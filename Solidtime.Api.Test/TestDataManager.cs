using Microsoft.Extensions.Logging;
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
		if (logger.IsEnabled(LogLevel.Information))
		{
			logger.LogInformation("Setting up test data for organization {OrganizationId}", organizationId);
		}

		try
		{
			// Use timestamp plus a GUID suffix to ensure unique names across repeated runs.
			var uniqueSuffix = $"{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}"[..27];

			await CreateSampleClientAsync(uniqueSuffix, cancellationToken);
			await CreateSampleProjectAsync(uniqueSuffix, cancellationToken);
			await CreateSampleTagAsync(uniqueSuffix, cancellationToken);

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
	/// Creates the sample client used by tests
	/// </summary>
	private async Task CreateSampleClientAsync(string uniqueSuffix, CancellationToken cancellationToken)
	{
		var clientRequest = new ClientStoreRequest
		{
			Name = $"TestClient-{uniqueSuffix}"
		};

		var clientResult = await _client.Clients.CreateAsync(organizationId, clientRequest, cancellationToken);
		SampleClientId = clientResult.Data.Id;
		if (logger.IsEnabled(LogLevel.Information))
		{
			logger.LogInformation("Created sample client: {ClientId}", SampleClientId);
		}
	}

	/// <summary>
	/// Creates the sample project used by tests
	/// </summary>
	private async Task CreateSampleProjectAsync(string uniqueSuffix, CancellationToken cancellationToken)
	{
		// Note: Using lowercase Material Design color (from user's color palette)
		var projectRequest = new ProjectStoreRequest
		{
			Name = $"TestProject-{uniqueSuffix}",
			Color = "#ef5350",  // Material Red 400 - confirmed working by user
			ClientId = SampleClientId,
			IsBillable = false
		};

		var projectResult = await _client.Projects.CreateAsync(organizationId, projectRequest, cancellationToken);
		SampleProjectId = projectResult.Data.Id;
		if (logger.IsEnabled(LogLevel.Information))
		{
			logger.LogInformation("Created sample project: {ProjectId}", SampleProjectId);
		}
	}

	/// <summary>
	/// Creates the sample tag used by tests
	/// </summary>
	private async Task CreateSampleTagAsync(string uniqueSuffix, CancellationToken cancellationToken)
	{
		var tagRequest = new TagStoreRequest
		{
			Name = $"TestTag-{uniqueSuffix}"
		};

		var tagResult = await _client.Tags.CreateAsync(organizationId, tagRequest, cancellationToken);
		SampleTagId = tagResult.Data.Id;
		if (logger.IsEnabled(LogLevel.Information))
		{
			logger.LogInformation("Created sample tag: {TagId}", SampleTagId);
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
               var entryDate = now.AddDays(-(7 * weekOffset + 1));
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
             if (logger.IsEnabled(LogLevel.Debug))
				{
					logger.LogDebug("Created sample time entry for {Date}", startTime);
				}
			}

            if (logger.IsEnabled(LogLevel.Information))
			{
				logger.LogInformation("Created {Count} sample time entries", 4);
			}
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
		if (logger.IsEnabled(LogLevel.Information))
		{
			logger.LogInformation("Cleaning up all test data for organization {OrganizationId}", organizationId);
		}

		try
		{
			// Ordered so that dependants go before the things they depend on: time entries and
			// tasks reference projects, and projects reference clients.
			await CleanupAllAsync(
				"time entry",
				ct => _client.TimeEntries.GetAsync(organizationId, null, null, ct),
				(timeEntry, ct) => _client.TimeEntries.DeleteAsync(organizationId, timeEntry.Id, ct),
				timeEntry => timeEntry.Id,
				cancellationToken);

			await CleanupAllAsync(
				"task",
				ct => _client.Tasks.GetAsync(organizationId, null, null, ct),
				(task, ct) => _client.Tasks.DeleteAsync(organizationId, task.Id, ct),
				task => task.Id,
				cancellationToken);

			await CleanupAllAsync(
				"tag",
				ct => _client.Tags.GetAsync(organizationId, ct),
				(tag, ct) => _client.Tags.DeleteAsync(organizationId, tag.Id, ct),
				tag => tag.Id,
				cancellationToken);

			await CleanupAllAsync(
				"project",
				ct => _client.Projects.GetAsync(organizationId, null, null, ct),
				(project, ct) => _client.Projects.DeleteAsync(organizationId, project.Id, ct),
				project => project.Id,
				cancellationToken);

			await CleanupAllAsync(
				"client",
				ct => _client.Clients.GetAsync(organizationId, null, null, ct),
				(clientEntity, ct) => _client.Clients.DeleteAsync(organizationId, clientEntity.Id, ct),
				clientEntity => clientEntity.Id,
				cancellationToken);

			logger.LogInformation("Test data cleanup completed");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to cleanup test data");
			// Don't throw - cleanup should be best effort
		}
	}

	/// <summary>
	/// Deletes every entity of one kind, best effort. Failing to list them, or to delete any one
	/// of them, is logged and does not stop the rest of the cleanup.
	/// </summary>
	/// <typeparam name="T">The entity type being cleaned up</typeparam>
	/// <param name="entityType">The entity kind, for log messages</param>
	/// <param name="getAllAsync">Lists the entities to delete</param>
	/// <param name="deleteAsync">Deletes one entity</param>
	/// <param name="getId">Reads an entity's ID, for log messages</param>
	/// <param name="cancellationToken">Cancellation token</param>
	private async Task CleanupAllAsync<T>(
		string entityType,
		Func<CancellationToken, Task<PaginatedResponse<T>>> getAllAsync,
		Func<T, CancellationToken, Task> deleteAsync,
		Func<T, string> getId,
		CancellationToken cancellationToken)
	{
		PaginatedResponse<T> entities;
		try
		{
			entities = await getAllAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to retrieve {EntityType} entities for cleanup", entityType);
			return;
		}

		foreach (var entity in entities.Data)
		{
			await DeleteBestEffortAsync(entityType, entity, deleteAsync, getId, cancellationToken);
		}
	}

	/// <summary>
	/// Deletes one entity, treating an already-deleted entity as success and logging any other
	/// failure without rethrowing.
	/// </summary>
	private async Task DeleteBestEffortAsync<T>(
		string entityType,
		T entity,
		Func<T, CancellationToken, Task> deleteAsync,
		Func<T, string> getId,
		CancellationToken cancellationToken)
	{
		try
		{
			await deleteAsync(entity, cancellationToken);
			if (logger.IsEnabled(LogLevel.Debug))
			{
				logger.LogDebug("Deleted {EntityType}: {EntityId}", entityType, getId(entity));
			}
		}
		catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
		{
			// Already deleted, ignore
			if (logger.IsEnabled(LogLevel.Debug))
			{
				logger.LogDebug("{EntityType} {EntityId} already deleted", entityType, getId(entity));
			}
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to delete {EntityType} {EntityId}", entityType, getId(entity));
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
