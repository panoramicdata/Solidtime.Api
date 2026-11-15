using Solidtime.Api.Interfaces;
using System.Net.Http;

namespace Solidtime.Api;

/// <summary>
/// A Solidtime API client
/// </summary>
public class SolidtimeClient : IDisposable
{
	private readonly List<IDisposable> _owned = [];
	private bool _disposed;

	/// <summary>
	/// Initializes a new instance of the <see cref="SolidtimeClient"/> class
	/// </summary>
	/// <param name="options">The client options</param>
	public SolidtimeClient(SolidtimeClientOptions options)
	{
		ArgumentNullException.ThrowIfNull(options);

		// Validate that all of the necessary configuration has been provided
		options.Validate();

		var httpClient = new HttpClient(new AuthenticatedBackingOffHttpClientHandler(options))
		{
			BaseAddress = new Uri(options.BaseUrl),
			Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
		};
		_owned.Add(httpClient);

		var refitSettings = new RefitSettings
		{
			ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
				WriteIndented = true,
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				UnmappedMemberHandling = options.JsonUnmappedMemberHandling,
				Converters =
				{
					new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)
				}
			})
		};

		// Initialize Refit interfaces
		ApiTokens = RestService.For<IApiTokens>(httpClient, refitSettings);
		Me = RestService.For<IMe>(httpClient, refitSettings);
		Organizations = RestService.For<IOrganizations>(httpClient, refitSettings);
		Clients = RestService.For<IClients>(httpClient, refitSettings);
		Projects = RestService.For<IProjects>(httpClient, refitSettings);
		Tags = RestService.For<ITags>(httpClient, refitSettings);
		Tasks = RestService.For<ITasks>(httpClient, refitSettings);
		TimeEntries = RestService.For<ITimeEntries>(httpClient, refitSettings);
		ProjectMembers = RestService.For<IProjectMembers>(httpClient, refitSettings);
		Members = RestService.For<IMembers>(httpClient, refitSettings);
		Reports = RestService.For<IReports>(httpClient, refitSettings);
		Charts = RestService.For<ICharts>(httpClient, refitSettings);
		Imports = RestService.For<IImports>(httpClient, refitSettings);
	}

	/// <summary>
	/// Gets the API for managing API tokens (Personal Access Tokens)
	/// </summary>
	public IApiTokens ApiTokens { get; }

	/// <summary>
	/// Gets the API for accessing information about the currently authenticated user
	/// </summary>
	public IMe Me { get; }

	/// <summary>
	/// Gets the API for managing organizations
	/// </summary>
	public IOrganizations Organizations { get; }

	/// <summary>
	/// Gets the API for managing clients
	/// </summary>
	public IClients Clients { get; }

	/// <summary>
	/// Gets the API for managing projects
	/// </summary>
	public IProjects Projects { get; }

	/// <summary>
	/// Gets the API for managing tags
	/// </summary>
	public ITags Tags { get; }

	/// <summary>
	/// Gets the API for managing tasks
	/// </summary>
	public ITasks Tasks { get; }

	/// <summary>
	/// Gets the API for managing time entries
	/// </summary>
	public ITimeEntries TimeEntries { get; }

	/// <summary>
	/// Gets the API for managing project members
	/// </summary>
	public IProjectMembers ProjectMembers { get; }

	/// <summary>
	/// Gets the API for managing organization members
	/// </summary>
	public IMembers Members { get; }

	/// <summary>
	/// Gets the API for managing reports
	/// </summary>
	public IReports Reports { get; }

	/// <summary>
	/// Gets the API for accessing chart data
	/// </summary>
	public ICharts Charts { get; }

	/// <summary>
	/// Gets the API for managing imports from other time tracking systems
	/// </summary>
	public IImports Imports { get; }

	/// <summary>
	/// Disposes the client and releases resources
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Disposes the client and releases resources
	/// </summary>
	/// <param name="disposing">True if disposing managed resources</param>
	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}

		if (disposing)
		{
			foreach (var owned in _owned)
			{
				owned.Dispose();
			}
		}

		_disposed = true;
	}
}
