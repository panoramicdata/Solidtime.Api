using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Solidtime.Api.Test;

/// <summary>
/// XUnit test fixture for dependency injection
/// Handles setup and cleanup of test data
/// </summary>
public class Fixture : TestBedFixture
{
	private IConfigurationRoot? _configuration;
	private readonly SemaphoreSlim _setupLock = new(1, 1);
	private bool _testDataSetup;

	/// <summary>
	/// Adds services to the dependency injection container
	/// </summary>
	/// <param name="services">The service collection</param>
	/// <param name="configuration">The configuration</param>
	protected override void AddServices(
		IServiceCollection services,
		IConfiguration? configuration)
	{
		if (_configuration is null)
		{
			throw new InvalidOperationException("Configuration is null");
		}

		services
			.AddScoped<CancellationTokenSource>()
			.Configure<Configuration>(_configuration.GetSection("Configuration"))
			.AddLogging(builder =>
			{
				builder.SetMinimumLevel(LogLevel.Debug);
			});
	}

	/// <summary>
	/// Sets up test data before running tests (called once)
	/// </summary>
	public async Task EnsureTestDataSetupAsync(ITestOutputHelper testOutputHelper)
	{
		await _setupLock.WaitAsync();
		try
		{
			if (_testDataSetup)
			{
				testOutputHelper.WriteLine("Test data already set up, skipping");
				return;
			}

			testOutputHelper.WriteLine("Setting up test data for the first time");
			await SetupTestDataAsync(testOutputHelper);
			_testDataSetup = true;
		}
		finally
		{
			_setupLock.Release();
		}
	}

	/// <summary>
	/// Sets up test data before running tests
	/// </summary>
	private async Task SetupTestDataAsync(ITestOutputHelper testOutputHelper)
	{
		try
		{
			// Get configuration
			var configOptions = GetService<IOptions<Configuration>>(testOutputHelper)
				?? throw new InvalidOperationException("Configuration not found.");
			var config = configOptions.Value;

			if (string.IsNullOrWhiteSpace(config.ApiToken))
			{
				throw new InvalidOperationException("ApiToken is required in user secrets.");
			}

			if (string.IsNullOrWhiteSpace(config.SampleOrganizationId))
			{
				throw new InvalidOperationException("SampleOrganizationId is required in user secrets.");
			}

			// Create logger
			var loggerFactory = LoggerFactory.Create(builder =>
			{
				builder.SetMinimumLevel(LogLevel.Debug);
				builder.AddProvider(new XunitLoggerProvider(testOutputHelper));
			});
			var logger = loggerFactory.CreateLogger<Fixture>();

			// Create Solidtime client
			var client = new SolidtimeClient(new SolidtimeClientOptions
			{
				ApiToken = config.ApiToken,
				Logger = logger
			});

			// Create test data manager
			TestDataManager = new TestDataManager(client, config.SampleOrganizationId, logger);

			// Setup new test data (skip cleanup to save API calls - cleanup will happen at end)
			await TestDataManager.SetupTestDataAsync(System.Threading.CancellationToken.None);

			logger.LogInformation("Test data setup completed successfully");
		}
		catch (Exception ex)
		{
			testOutputHelper.WriteLine($"Failed to setup test data: {ex.Message}");
			throw;
		}
	}

	/// <summary>
	/// Gets the test data manager
	/// </summary>
	public TestDataManager? TestDataManager { get; private set; }

	/// <summary>
	/// Disposes resources asynchronously
	/// </summary>
	/// <returns>A value task</returns>
	protected override async ValueTask DisposeAsyncCore()
	{
		// Cleanup test data when all tests are done
		if (TestDataManager != null)
		{
			await TestDataManager.CleanupAllTestDataAsync(System.Threading.CancellationToken.None);
		}

		_setupLock?.Dispose();
	}

	/// <summary>
	/// Gets test application settings
	/// </summary>
	/// <returns>Test application settings</returns>
	protected override IEnumerable<TestAppSettings> GetTestAppSettings()
	{
		var directoryInfo = new DirectoryInfo(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../..")));
		_configuration = new ConfigurationBuilder()
			.SetBasePath(directoryInfo.FullName)
			.AddJsonFile("appsettings.json", true)
			.AddUserSecrets<Configuration>()
			.AddEnvironmentVariables()
			.Build();

		// This is not used
		return [
			new TestAppSettings
			{
				IsOptional = true,
				Filename = null,
			}
		];
	}
}
