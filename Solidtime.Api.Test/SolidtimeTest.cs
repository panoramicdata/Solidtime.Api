using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
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

		// Logger
		var loggerFactory = fixture
			.GetService<ILoggerFactory>(testOutputHelper)
			?? throw new InvalidOperationException("LoggerFactory is null");
		Logger = loggerFactory.CreateLogger(GetType());

		// Configuration
		var configOptions = fixture
			.GetService<IOptions<Configuration>>(testOutputHelper)
			?? throw new InvalidOperationException("Configuration not found.");
		Configuration = configOptions.Value;

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
	protected async Task<string> GetOrganizationIdAsync()
	{
		if (_organizationId is null)
		{
			// Use configured organization ID
			_organizationId = Configuration.SampleOrganizationId;
			
			// If we add a Me.GetOrganizations() method in the future, we could fetch it dynamically:
			// var orgs = await SolidtimeClient.Me.GetOrganizationsAsync(CancellationToken);
			// _organizationId = orgs.Data.First().Id;
		}

		return _organizationId;
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

		// Otherwise, fetch from API
		// TODO: Implement once IProjects interface is created
		// var organizationId = await GetOrganizationIdAsync();
		// var projects = await SolidtimeClient.Projects.GetAsync(organizationId, null, null, CancellationToken);
		// return projects.Data.First().Id;
		
		throw new InvalidOperationException("SampleProjectId not configured in user secrets");
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

		// Otherwise, fetch from API
		// TODO: Implement once IClients interface is created
		// var organizationId = await GetOrganizationIdAsync();
		// var clients = await SolidtimeClient.Clients.GetAsync(organizationId, CancellationToken);
		// return clients.Data.First().Id;
		
		throw new InvalidOperationException("SampleClientId not configured in user secrets");
	}
}
