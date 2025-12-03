using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Threading;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Solidtime.Api.Test;

/// <summary>
/// Base class for Solidtime API tests
/// </summary>
public class SolidtimeTest : TestBed<Fixture>
{
	/// <summary>
	/// Gets the cancellation token for the current test
	/// </summary>
	protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

	private string? _organizationId;
	private new readonly Fixture _fixture;

	/// <summary>
	/// Gets the logger
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Gets the Solidtime client
	/// </summary>
	protected SolidtimeClient SolidtimeClient { get; }

	/// <summary>
	/// Gets the test configuration
	/// </summary>
	protected Configuration Configuration { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="SolidtimeTest"/> class
	/// </summary>
	/// <param name="testOutputHelper">The test output helper</param>
	/// <param name="fixture">The test fixture</param>
	protected SolidtimeTest(ITestOutputHelper testOutputHelper, Fixture fixture) : base(testOutputHelper, fixture)
	{
		ArgumentNullException.ThrowIfNull(testOutputHelper);
		ArgumentNullException.ThrowIfNull(fixture);

		_fixture = fixture;

		// Ensure test data is set up
		_fixture.EnsureTestDataSetupAsync(testOutputHelper).Wait();

		// Logger - create a logger factory with XUnit output
		var loggerFactory = LoggerFactory.Create(builder =>
		{
			builder.SetMinimumLevel(LogLevel.Debug);
			builder.AddProvider(new XunitLoggerProvider(testOutputHelper));
		});
		Logger = loggerFactory.CreateLogger(GetType());

		// Configuration
		var configOptions = fixture
			.GetService<IOptions<Configuration>>(testOutputHelper)
			?? throw new InvalidOperationException("Configuration not found.");
		Configuration = configOptions.Value;

		// Validate configuration
		if (string.IsNullOrWhiteSpace(Configuration.ApiToken))
		{
			throw new InvalidOperationException(
				"ApiToken is required. Please configure it in user secrets using: " +
				"dotnet user-secrets set \"Configuration:ApiToken\" \"your-token-here\" --project Solidtime.Api.Test");
		}

		// Solidtime Client
		SolidtimeClient = new SolidtimeClient(new SolidtimeClientOptions
		{
			ApiToken = Configuration.ApiToken,
			Logger = Logger,
			// Find missing properties during unit testing
			// Note that this is NOT the default behavior, so changes to the API won't cause backward compatibility issues
			JsonUnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
		});
	}

	/// <summary>
	/// Gets the organization ID for testing
	/// </summary>
	/// <returns>The organization ID</returns>
	/// <exception cref="InvalidOperationException">Thrown when SampleOrganizationId is not configured</exception>
	protected async Task<string> GetOrganizationIdAsync()
	{
		if (_organizationId != null)
		{
			return _organizationId;
		}

		// Try to use configured organization ID first
		if (!string.IsNullOrWhiteSpace(Configuration.SampleOrganizationId))
		{
			_organizationId = Configuration.SampleOrganizationId;
			return _organizationId;
		}

		// If not configured, we need to discover it
		// Unfortunately, there's no endpoint to list all organizations the user belongs to
		// The user must configure it in user secrets
		throw new InvalidOperationException(
			"SampleOrganizationId is not configured. Please set it in user secrets using: " +
			"dotnet user-secrets set \"Configuration:SampleOrganizationId\" \"your-org-id\" --project Solidtime.Api.Test\n\n" +
			"You can find your organization ID by:\n" +
			"1. Logging into Solidtime (https://app.solidtime.io)\n" +
			"2. Navigating to your organization settings\n" +
			"3. The organization ID is in the URL: /organizations/{organization-id}/settings");
	}

	/// <summary>
	/// Gets a sample project ID for testing
	/// </summary>
	/// <returns>The project ID</returns>
	protected async Task<string> GetProjectIdAsync()
	{
		// If configured, use the sample project ID
		if (!string.IsNullOrEmpty(Configuration.SampleProjectId))
		{
			return Configuration.SampleProjectId;
		}

		// Otherwise, use the project created during test setup
		if (_fixture.TestDataManager?.SampleProjectId != null)
		{
			return _fixture.TestDataManager.SampleProjectId;
		}

		// Otherwise, fetch from API
		var organizationId = await GetOrganizationIdAsync();
		var projects = await SolidtimeClient.Projects.GetAsync(organizationId, null, null, CancellationToken);
		
		if (projects.Data.Count == 0)
		{
			throw new InvalidOperationException(
				"No projects found in the organization. Please create a project first or configure SampleProjectId in user secrets.");
		}
		
		return projects.Data.First().Id;
	}

	/// <summary>
	/// Gets a sample client ID for testing
	/// </summary>
	/// <returns>The client ID</returns>
	protected async Task<string> GetClientIdAsync()
	{
		// If configured, use the sample client ID
		if (!string.IsNullOrEmpty(Configuration.SampleClientId))
		{
			return Configuration.SampleClientId;
		}

		// Otherwise, use the client created during test setup
		if (_fixture.TestDataManager?.SampleClientId != null)
		{
			return _fixture.TestDataManager.SampleClientId;
		}

		// Otherwise, fetch from API
		var organizationId = await GetOrganizationIdAsync();
		var clients = await SolidtimeClient.Clients.GetAsync(organizationId, null, null, CancellationToken);
		
		if (clients.Data.Count == 0)
		{
			throw new InvalidOperationException(
				"No clients found in the organization. Please create a client first or configure SampleClientId in user secrets.");
		}
		
		return clients.Data.First().Id;
	}

	/// <summary>
	/// Gets a sample tag ID for testing
	/// </summary>
	/// <returns>The tag ID</returns>
	protected async Task<string> GetTagIdAsync()
	{
		// Use the tag created during test setup
		if (_fixture.TestDataManager?.SampleTagId != null)
		{
			return _fixture.TestDataManager.SampleTagId;
		}

		// Otherwise, fetch from API
		var organizationId = await GetOrganizationIdAsync();
		var tags = await SolidtimeClient.Tags.GetAsync(organizationId, null, null, CancellationToken);
		
		if (tags.Data.Count == 0)
		{
			throw new InvalidOperationException(
				"No tags found in the organization. Please create a tag first.");
		}
		
		return tags.Data.First().Id;
	}
}
