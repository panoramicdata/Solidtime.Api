using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Solidtime.Api.Test;

/// <summary>
/// XUnit test fixture for dependency injection
/// </summary>
public class Fixture : TestBedFixture
{
	private IConfigurationRoot? _configuration;

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
	/// Disposes resources asynchronously
	/// </summary>
	/// <returns>A value task</returns>
	protected override ValueTask DisposeAsyncCore()
		=> default;

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
