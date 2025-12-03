# Solidtime.Api NuGet Package

[![Nuget](https://img.shields.io/nuget/v/Solidtime.Api)](https://www.nuget.org/packages/Solidtime.Api/)
[![Nuget](https://img.shields.io/nuget/dt/Solidtime.Api)](https://www.nuget.org/packages/Solidtime.Api/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Description

This is a .NET 10.0 library for the Solidtime API.

Solidtime is a modern, open-source time tracking solution with comprehensive project management, time tracking, reporting, and team collaboration features.

This library provides strongly-typed access to the Solidtime API v1.

## Installation

```bash
dotnet add package Solidtime.Api
```

## Quick Start

```csharp
using Solidtime.Api;

// Create a client with your API token
var client = new SolidtimeClient(new SolidtimeClientOptions
{
    ApiToken = "your-api-token-here"
});

// Get current user information
var me = await client.Me.GetAsync(CancellationToken.None);
Console.WriteLine($"Hello, {me.Data.Name}!");

// Get organizations
var organizations = await client.Organizations.GetAsync(CancellationToken.None);
var orgId = organizations.Data.First().Id;

// Get projects for an organization
var projects = await client.Projects.GetAsync(orgId, null, null, CancellationToken.None);
foreach (var project in projects.Data)
{
    Console.WriteLine($"Project: {project.Name}");
}
```

## Authentication

Solidtime API uses Bearer token authentication (Personal Access Tokens). You can create an API token in your Solidtime account settings.

```csharp
var options = new SolidtimeClientOptions
{
    ApiToken = "your-api-token",
    TimeoutSeconds = 30 // Optional, default is 30
};

var client = new SolidtimeClient(options);
```

## API Coverage

This library provides access to all Solidtime API v1 endpoints:

- **API Tokens** - Manage personal access tokens
- **Users** - User management and current user information
- **Organizations** - Organization management and settings
- **Clients** - Client management
- **Projects** - Project management and archiving
- **Project Members** - Project member management
- **Tags** - Tag management
- **Tasks** - Task management
- **Time Entries** - Time entry management and tracking
- **Members** - Organization member management
- **Reports** - Report generation and management
- **Charts** - Data visualization
- **Imports** - Import data from other time tracking tools

## Usage Examples

### Managing Projects

```csharp
// Create a project
var newProject = await client.Projects.CreateAsync(
    organizationId,
    new ProjectStoreRequest
    {
        Name = "My New Project",
        Color = "#3498db",
        IsBillable = true,
        ClientId = clientId // optional
    },
    CancellationToken.None);

// Update a project
var updatedProject = await client.Projects.UpdateAsync(
    organizationId,
    projectId,
    new ProjectUpdateRequest
    {
        Name = "Updated Project Name",
        IsArchived = false
    },
    CancellationToken.None);

// Get all projects
var projects = await client.Projects.GetAsync(
    organizationId,
    page: 1,
    perPage: 50,
    CancellationToken.None);
```

### Time Tracking

```csharp
// Start a time entry
var timeEntry = await client.TimeEntries.CreateAsync(
    organizationId,
    new TimeEntryStoreRequest
    {
        ProjectId = projectId,
        TaskId = taskId, // optional
        Description = "Working on feature X",
        Start = DateTimeOffset.UtcNow,
        Tags = new[] { tagId } // optional
    },
    CancellationToken.None);

// Stop a time entry
var stoppedEntry = await client.TimeEntries.UpdateAsync(
    organizationId,
    timeEntry.Data.Id,
    new TimeEntryUpdateRequest
    {
        End = DateTimeOffset.UtcNow
    },
    CancellationToken.None);
```

### Managing Clients

```csharp
// Create a client
var client = await client.Clients.CreateAsync(
    organizationId,
    new ClientStoreRequest
    {
        Name = "Acme Corporation"
    },
    CancellationToken.None);

// List all clients
var clients = await client.Clients.GetAsync(
    organizationId,
    CancellationToken.None);
```

## Error Handling

The library throws `SolidtimeApiException` for API errors:

```csharp
try
{
    var project = await client.Projects.GetAsync(orgId, projectId, CancellationToken.None);
}
catch (SolidtimeApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
}
```

## Configuration

### Base URL

By default, the client uses `https://app.solidtime.io/api`. You can override this:

```csharp
var options = new SolidtimeClientOptions
{
    ApiToken = "your-token",
    BaseUrl = "https://your-instance.solidtime.io/api"
};
```

### Logging

The client supports Microsoft.Extensions.Logging:

```csharp
var logger = loggerFactory.CreateLogger<SolidtimeClient>();
var options = new SolidtimeClientOptions
{
    ApiToken = "your-token",
    Logger = logger
};
```

## Contributing

This project is developed using:
- **Refit** for declarative HTTP client interfaces
- **System.Text.Json** for JSON serialization
- **XUnit 3** and **AwesomeAssertions** for testing

Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Add tests for your changes
4. Ensure all tests pass
5. Submit a pull request

Refer to the Solidtime API documentation for endpoint details: https://docs.solidtime.io/api-reference

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built by [Panoramic Data Limited](https://github.com/panoramicdata)
- Solidtime API documentation: https://docs.solidtime.io/api-reference
