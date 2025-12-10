# Solidtime.Api NuGet Package

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/3201cac7238d4aeaa062e1fc6092c715)](https://app.codacy.com/gh/panoramicdata/Solidtime.Api/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
[![Nuget](https://img.shields.io/nuget/v/Solidtime.Api)](https://www.nuget.org/packages/Solidtime.Api/)
[![Nuget](https://img.shields.io/nuget/dt/Solidtime.Api)](https://www.nuget.org/packages/Solidtime.Api/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Description

A .NET 10.0 client library for the [Solidtime API](https://docs.solidtime.io/api-reference).

Solidtime is a modern, open-source time tracking solution with comprehensive project management, time tracking, reporting, and team collaboration features.

This library provides strongly-typed access to the Solidtime API v1 with full support for:
- **API Tokens** - Personal access token management
- **Users** - User profile and current user information
- **Organizations** - Organization management and settings
- **Clients** - Client management with pagination
- **Projects** - Project management with archiving support
- **Project Members** - Project member and billing rate management
- **Tags** - Tag management with pagination
- **Tasks** - Task management
- **Time Entries** - Full time tracking with start/stop timer support
- **Members** - Organization member management
- **Reports** - Report generation and management
- **Charts** - Data visualization endpoints
- **Imports** - Data import from other time tracking tools

## Installation

```bash
dotnet add package Solidtime.Api
```

Or via Package Manager:
```powershell
Install-Package Solidtime.Api
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

// Get an organization (you need to know your organization ID)
var organizationId = "your-organization-uuid";
var organization = await client.Organizations.GetAsync(organizationId, CancellationToken.None);
Console.WriteLine($"Organization: {organization.Data.Name}");

// Get projects for an organization
var projects = await client.Projects.GetAsync(organizationId, null, null, CancellationToken.None);
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
    archived: null,
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
var newClient = await client.Clients.CreateAsync(
    organizationId,
    new ClientStoreRequest
    {
        Name = "Acme Corporation"
    },
    CancellationToken.None);

// List all clients
var clients = await client.Clients.GetAsync(
    organizationId,
    page: null,
    archived: null,
    CancellationToken.None);
```

## Error Handling

The library throws `SolidtimeApiException` for API errors:

```csharp
try
{
    var project = await client.Projects.GetByIdAsync(orgId, projectId, CancellationToken.None);
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

### Running Tests

The test project requires configuration via user secrets. Set up your test environment:

**Step 1: Find Your Organization ID**

We've included a helper script to guide you:

```powershell
# Run the helper script (it will prompt for your API token)
.\GetOrganizationId.ps1

# Or provide the token directly
.\GetOrganizationId.ps1 -ApiToken "your-api-token"
```

**Alternatively, find it manually:**
1. Log into Solidtime at https://app.solidtime.io
2. Look at the URL in your browser
3. The URL will be: `https://app.solidtime.io/teams/{YOUR-ORGANIZATION-ID}/...`
4. Copy the UUID from the URL (the part after `/teams/`)
5. Note: The Solidtime UI uses "teams" in the URL path, but the API uses "organizations"

**Step 2: Configure User Secrets**

```bash
cd Solidtime.Api.Test

# Set your API token (required)
dotnet user-secrets set "Configuration:ApiToken" "your-api-token"

# Set your organization ID (required for most tests)
dotnet user-secrets set "Configuration:SampleOrganizationId" "your-organization-uuid"

# Optional: Set sample IDs to speed up tests
dotnet user-secrets set "Configuration:SampleProjectId" "project-uuid"
dotnet user-secrets set "Configuration:SampleClientId" "client-uuid"
```

**Step 3: Run Tests**

```bash
dotnet test
```

### Publishing to NuGet (Maintainers Only)

This repository includes a PowerShell script for publishing packages to NuGet.org:

1. Create a file named `nuget-key.txt` in the solution root containing your NuGet API key
   - Get your API key from: https://www.nuget.org/account/apikeys
   - This file is .gitignored and will not be committed
2. Run the publish script:
   ```powershell
   # Run all tests and publish
   .\Publish.ps1
   
   # Skip tests and publish (not recommended)
   .\Publish.ps1 -SkipTests
   
   # Publish a Debug build (not recommended for production)
   .\Publish.ps1 -Configuration Debug
   ```

The script will:
- ? Clean and restore the project
- ? Build in Release configuration
- ? Run all unit tests (unless -SkipTests is specified)
- ? Pack the NuGet package with symbols
- ? Publish to NuGet.org automatically

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built by [Panoramic Data Limited](https://github.com/panoramicdata)
- Solidtime API documentation: https://docs.solidtime.io/api-reference
