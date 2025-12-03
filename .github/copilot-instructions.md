# GitHub Copilot Instructions for Solidtime.Api

## 🚨 CRITICAL RULES - READ FIRST 🚨

### NEVER SKIP TESTS
- **NEVER** suggest skipping, disabling, or commenting out failing tests
- **NEVER** use `Skip = "reason"` on tests unless explicitly requested by the user
- **ALWAYS** fix the root cause of test failures - tests exist to catch issues
- If a test fails, investigate and fix the underlying problem
- If you don't know how to fix it, ask for clarification - don't bypass it
- The only acceptable reason to skip a test is missing test data (404 from API), and even then, document it clearly

### Fix Issues, Don't Hide Them
- Unmapped JSON properties? Add them to the model
- Type mismatch? Fix the type in the model
- Missing property? Add it with proper JSON attribute
- 404 errors? Check if test data exists, document the requirement
- Use `JsonExtensionData` during development to discover missing properties, then add them properly

---

## Project Status

**Current Phase**: ✅ Setup Complete - Ready for API Implementation

**Completed**:
- ✅ Solution and project structure
- ✅ Core infrastructure (SolidtimeClient, SolidtimeClientOptions, AuthenticatedHttpClientHandler)
- ✅ Test framework (Fixture, Configuration, SolidtimeTest base class)
- ✅ Build verification successful
- ✅ XUnit v3 compatibility confirmed

**Next Phase**: Implement API interfaces starting with IApiTokens

**Important Files**:
- `MASTER_PLAN.md` - Complete implementation roadmap with phases
- `solidtime-openapi.json` - OpenAPI specification for all endpoints
- `../Toggl.Api/` - Reference implementation to follow

---

## Project Overview

**Solidtime.Api** is a .NET 10 NuGet package providing a strongly-typed client library for the [Solidtime API](https://docs.solidtime.io/api-reference). This project follows the exact same architecture, patterns, and quality standards as the sibling **Toggl.Api** project.

## Project Context

### What is Solidtime?
Solidtime is a modern, open-source time tracking solution with self-hosting capabilities. It provides comprehensive project management, time tracking, reporting, and team collaboration features.

### Project Goals
- Create a production-ready .NET client library for the Solidtime API
- Follow identical patterns and standards as Toggl.Api for consistency
- Provide a clean, intuitive, strongly-typed API surface
- Support .NET 10 with latest C# features
- Include comprehensive test coverage with XUnit 3 and AwesomeAssertions

## Technology Stack

### Core Technologies
- **Framework**: .NET 10.0
- **Language**: C# (latest version)
- **REST Client**: Refit 8.0.0+ (for declarative HTTP client interfaces)
- **JSON**: System.Text.Json (snake_case naming policy)
- **HTTP Authentication**: Bearer token via custom AuthenticatedHttpClientHandler

### Testing Stack
- **Framework**: XUnit 3.2.0+
- **Assertions**: AwesomeAssertions 9.3.0+ (fluent assertion library)
- **DI Integration**: Xunit.Microsoft.DependencyInjection 10.0.0+
- **Configuration**: Microsoft.Extensions.Configuration with UserSecrets
- **Code Coverage**: coverlet.collector 6.0.2

### Tooling
- **Versioning**: Nerdbank.GitVersioning 3.9.50+ (version.json based)
- **Logging**: Microsoft.Extensions.Logging.Abstractions 10.0.0
- **CI/CD**: GitHub Actions (CodeQL security analysis, build/test)

## Architecture Patterns

### 1. Client Class Pattern
The main `SolidtimeClient` class:
- Implements `IDisposable`
- Accepts `SolidtimeClientOptions` in constructor
- Creates single HttpClient with custom authentication handler
- Initializes all Refit interfaces with consistent settings
- Exposes resource-specific interfaces as properties (e.g., `client.Projects`, `client.TimeEntries`)

### 2. Refit Interface Pattern
All API interfaces use Refit attributes:
```csharp
public interface IProjects
{
    [Get("/v1/organizations/{organization}/projects")]
    Task<DataWrapper<List<Project>>> GetAsync(
        [AliasAs("organization")] string organizationId,
        [Query] int? page,
        [Query] int? perPage,
        CancellationToken cancellationToken);
}
```

Key conventions:
- All methods are async (return `Task<T>`)
- All methods accept `CancellationToken` as final parameter
- Use `[AliasAs]` for path and query parameters
- Use `[Body]` for request payloads
- Use proper HTTP method attributes (`[Get]`, `[Post]`, `[Put]`, `[Delete]`)

### 3. Model Pattern
All models use System.Text.Json attributes:
```csharp
public class Project
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("is_archived")]
    public required bool IsArchived { get; set; }
}
```

Key conventions:
- Use `[JsonPropertyName]` with snake_case naming
- Use `required` keyword for non-nullable required properties
- Use nullable types (`?`) for optional properties
- Match OpenAPI schema exactly
- Include XML documentation comments

### 4. Response Wrapper Pattern
Solidtime API wraps responses in `{ "data": ... }`:
```csharp
public class DataWrapper<T>
{
    [JsonPropertyName("data")]
    public required T Data { get; set; }
}
```

Paginated responses include links and meta:
```csharp
public class PaginatedResponse<T>
{
    [JsonPropertyName("data")]
    public required List<T> Data { get; set; }
    
    [JsonPropertyName("links")]
    public required PaginationLinks Links { get; set; }
    
    [JsonPropertyName("meta")]
    public required PaginationMeta Meta { get; set; }
}
```

### 5. Test Pattern
All tests inherit from `SolidtimeTest` base class:
```csharp
public class ProjectTests(ITestOutputHelper testOutputHelper, Fixture fixture) 
    : SolidtimeTest(testOutputHelper, fixture)
{
    [Fact]
    public async Task Projects_Get_Succeeds()
    {
        var organizationId = await GetOrganizationIdAsync();
        var projects = await SolidtimeClient
            .Projects
            .GetAsync(organizationId, null, null, CancellationToken);
        
        projects.Should().NotBeNull();
        projects.Data.Should().NotBeNullOrEmpty();
    }
}
```

Test naming: `{Resource}_{Method}_{Scenario}`

## API Specifics

### Base URL
- Production: `https://app.solidtime.io/api`
- Staging: `https://app.staging.solidtime.io/api`
- Local: `https://solidtime.test/api`

### Authentication
- Uses OAuth2 Bearer tokens (Personal Access Tokens)
- Token added via `Authorization: Bearer {token}` header
- Implemented in `AuthenticatedHttpClientHandler`

### URL Structure
- Pattern: `/v1/organizations/{organization}/{resource}`
- Organization ID is a UUID string
- Most resources are scoped to an organization

### JSON Conventions
- **Naming**: snake_case (e.g., `is_archived`, `created_at`)
- **Dates**: ISO 8601 format, UTC timezone (e.g., `2024-02-26T17:17:17Z`)
- **IDs**: UUIDs as strings (except API token IDs which are non-UUID strings)

### Common Response Patterns
- Single resource: `{ "data": { ... } }`
- Collection: `{ "data": [ ... ] }`
- Paginated: `{ "data": [...], "links": {...}, "meta": {...} }`
- Success (no content): HTTP 204
- Errors: HTTP 4xx/5xx with error details in response body

## Code Quality Standards

### Project Settings
```xml
<Nullable>enable</Nullable>
<AnalysisMode>Recommended</AnalysisMode>
<AnalysisLevel>latest-recommended</AnalysisLevel>
<LangVersion>latest</LangVersion>
<WarningLevel>8</WarningLevel>
```

### Naming Conventions
- **Classes/Interfaces**: `PascalCase` (e.g., `SolidtimeClient`, `IProjects`)
- **Methods/Properties**: `PascalCase`
- **Parameters/Variables**: `camelCase`
- **Private fields**: `_camelCase` (with underscore prefix)
- **Constants**: `PascalCase`

### Code Patterns
- ✅ Use `required` keyword for required properties
- ✅ Use nullable reference types (`?`) for optional values
- ✅ Use primary constructors for dependency injection (C# 12+)
- ✅ Use `record` types for simple DTOs
- ✅ Always pass `CancellationToken` to async methods
- ✅ Use `await` consistently (no `.Result` or `.Wait()`)
- ✅ Validate arguments with `ArgumentNullException.ThrowIfNull()`
- ❌ Avoid `async void` (except event handlers)
- ❌ Avoid catching generic `Exception` without rethrowing

### Documentation
- All public APIs **must** have XML documentation
- Include `<summary>`, `<param>`, `<returns>`, and `<exception>` tags
- Reference official API docs: `https://engineering.solidtime.com/docs/...` (update URL as needed)
- Add `<example>` tags for complex usage

## Testing Guidelines

### Test Structure
- Inherit from `SolidtimeTest` base class
- Use primary constructors: `(ITestOutputHelper testOutputHelper, Fixture fixture)`
- Use `[Fact]` for single test cases
- Use `[Theory]` with `[InlineData]` for parameterized tests
- Use `CancellationToken` from base class property

### Assertions
Use AwesomeAssertions (fluent syntax):
```csharp
result.Should().NotBeNull();
result.Data.Should().NotBeNullOrEmpty();
result.Data.Count.Should().BeGreaterThan(0);
project.Name.Should().Be("Expected Name");
```

### Test Organization
- One test class per interface (e.g., `ProjectTests` for `IProjects`)
- Test all CRUD operations
- Test error scenarios (404, 401, 403, etc.)
- Test pagination where applicable
- Clean up test data in integration tests

### Configuration
Use UserSecrets for test configuration:
```json
{
  "Configuration": {
    "ApiToken": "your-api-token",
    "SampleOrganizationId": "uuid-here",
    "SampleProjectId": "uuid-here"
  }
}
```

**Finding Your Organization ID:**
The Solidtime web UI uses `/teams/{organization-id}` in URLs (note: "teams" not "organizations"):
1. Log into https://app.solidtime.io
2. Look at the URL - it will be: `https://app.solidtime.io/teams/{YOUR-ORG-ID}`
3. Copy the UUID from the URL
4. Set it in user secrets: `dotnet user-secrets set "Configuration:SampleOrganizationId" "your-uuid"`

## Common Gotchas & Tips

### Solidtime vs Toggl Differences
| Aspect | Toggl | Solidtime |
|--------|-------|-----------|
| JSON naming | camelCase | snake_case |
| ID type | `long` (numeric) | `string` (UUID) |
| URL structure | `/api/v9/workspaces/{id}` | `/v1/organizations/{id}` |
| Response wrapping | Direct objects | `{ "data": ... }` wrapper |
| Authentication | API key | Bearer token (OAuth2) |

### When Creating New Interfaces
1. Check `solidtime-openapi.json` for the exact endpoint structure
2. Create models matching the OpenAPI schema exactly
3. Use `[AliasAs]` to map C# names to API parameter names
4. Return appropriate wrapper types (`DataWrapper<T>` or `PaginatedResponse<T>`)
5. Include `CancellationToken` parameter
6. Add XML documentation with API reference links

### When Creating Models
1. Check OpenAPI schemas in `solidtime-openapi.json`
2. Use `[JsonPropertyName("snake_case_name")]` for all properties
3. Use `required` for non-nullable required fields
4. Use `string?` for optional string fields
5. Use `DateTimeOffset` for timestamps (not `DateTime`)
6. Include description from OpenAPI as XML documentation

### When Writing Tests
1. Use helper methods from `SolidtimeTest`: `GetOrganizationIdAsync()`, etc.
2. Don't hardcode IDs - use configuration or fetch dynamically
3. Use AwesomeAssertions, not built-in `Assert` class
4. Test the happy path first, then edge cases
5. Consider adding `[Trait("Category", "Integration")]` for tests requiring API access

## File Organization

### Main Project Structure
```
Solidtime.Api/
├── Exceptions/           # Custom exceptions
├── Extensions/           # Extension methods (dates, strings, etc.)
├── Interfaces/           # Refit API interfaces
├── Models/              # DTOs and domain models
│   └── Enums/           # Enumerations
├── QueryObjects/        # Query parameter objects
└── Properties/          # Assembly info, etc.
```

### Test Project Structure
```
Solidtime.Api.Test/
├── {Resource}Tests.cs   # One per interface
├── Configuration.cs     # Test configuration model
├── Fixture.cs          # XUnit DI fixture
├── SolidtimeTest.cs    # Base test class
└── appsettings.json    # Optional test settings
```

## References & Resources

### Primary References
- **Toggl.Api Project**: `../Toggl.Api/` (sibling project - **the main reference for patterns**)
- **OpenAPI Spec**: `solidtime-openapi.json` (in project root)
- **API Documentation**: https://docs.solidtime.io/api-reference
- **Master Plan**: `MASTER_PLAN.md` (in project root)

### Library Documentation
- **Refit**: https://github.com/reactiveui/refit
- **System.Text.Json**: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview
- **XUnit**: https://xunit.net/
- **AwesomeAssertions**: https://github.com/csMACnz/AwesomeAssertions
- **Nerdbank.GitVersioning**: https://github.com/dotnet/Nerdbank.GitVersioning

## Current Implementation Status

### ✅ Completed Infrastructure

**Main Project** (`Solidtime.Api/`):
- `SolidtimeClient.cs` - Main client class (skeleton, ready for interface properties)
- `SolidtimeClientOptions.cs` - Configuration with validation
- `AuthenticatedHttpClientHandler.cs` - Bearer token authentication
- `Exceptions/SolidtimeApiException.cs` - Custom exception type
- Project compiles successfully with all dependencies

**Test Project** (`Solidtime.Api.Test/`):
- `Fixture.cs` - XUnit DI fixture with UserSecrets support
- `Configuration.cs` - Test configuration model
- `SolidtimeTest.cs` - Base test class with helper methods
- UserSecrets ID: `deca5666-5906-4b93-9a14-2a998cd4252b`
- Project compiles successfully

### 🚧 Ready to Implement

The project is ready for Phase 3: Core API Implementation. Follow this priority order:

1. **IApiTokens** (authentication management)
2. **IMe** (current user info)
3. **IOrganizations** (organization context)
4. **IClients**, **IProjects**, **ITags**, **ITasks** (core resources)
5. **ITimeEntries** (time tracking)
6. **IProjectMembers**, **IMembers** (team management)

For each interface:
1. Create models in `Models/` based on OpenAPI spec
2. Create interface in `Interfaces/` with Refit attributes
3. Add property to `SolidtimeClient.cs`
4. Create test class in test project
5. Write tests and verify

### XUnit v3 Specific Notes

**Important**: XUnit v3 has namespace changes:
- `ITestOutputHelper` is in `Xunit` namespace (NOT `Xunit.Abstractions`)
- Use `using Xunit;` for test output helper
- Use `using Xunit.Microsoft.DependencyInjection.Abstracts;` for TestBed
- No `[CollectionDefinition]` attribute needed on test base class

## Development Workflow

### When Starting a New Feature
1. Review the corresponding section in `solidtime-openapi.json`
2. Check how similar feature is implemented in Toggl.Api
3. Create models first (in `Models/` folder)
4. Create interface second (in `Interfaces/` folder)
5. Add property to `SolidtimeClient`
6. Create test class (in test project)
7. Write tests and implement
8. Add XML documentation
9. Update README.md if needed

### Before Committing
- [ ] Build succeeds with zero warnings
- [ ] All tests pass
- [ ] New code has XML documentation
- [ ] Code follows naming conventions
- [ ] No hardcoded secrets/tokens
- [ ] Updated relevant documentation

### Pull Request Checklist
- [ ] Descriptive PR title and description
- [ ] All tests pass in CI
- [ ] CodeQL security scan passes
- [ ] Code coverage maintained or improved
- [ ] Follows project patterns from Toggl.Api
- [ ] Documentation updated

## Common Tasks

### Adding a New Endpoint
```csharp
// 1. In Interfaces/IProjects.cs
[Post("/v1/organizations/{organization}/projects")]
Task<DataWrapper<Project>> CreateAsync(
    [AliasAs("organization")] string organizationId,
    [Body] ProjectStoreRequest request,
    CancellationToken cancellationToken);

// 2. In SolidtimeClient.cs (constructor)
Projects = RestService.For<IProjects>(_httpClient, refitSettings);

// 3. In ProjectTests.cs
[Fact]
public async Task Projects_Create_Succeeds()
{
    var organizationId = await GetOrganizationIdAsync();
    var request = new ProjectStoreRequest { /* ... */ };
    
    var result = await SolidtimeClient.Projects.CreateAsync(
        organizationId, request, CancellationToken);
    
    result.Should().NotBeNull();
    result.Data.Should().NotBeNull();
    result.Data.Name.Should().Be(request.Name);
}
```

### Adding a New Model
```csharp
/// <summary>
/// Represents a project in Solidtime
/// </summary>
public class Project
{
    /// <summary>
    /// Unique identifier for the project
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    /// <summary>
    /// Project name
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    /// <summary>
    /// Whether the project is archived
    /// </summary>
    [JsonPropertyName("is_archived")]
    public required bool IsArchived { get; set; }
    
    /// <summary>
    /// When the project was created (ISO 8601 format, UTC)
    /// </summary>
    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }
}
```

## Key Principles

1. **Consistency**: Follow Toggl.Api patterns exactly - they're proven and well-tested
2. **Testability**: Every public API should be testable and have tests
3. **Documentation**: Code should be self-documenting with clear XML docs
4. **Type Safety**: Leverage C#'s type system - use strong types, not primitives
5. **Async All the Way**: All I/O operations are async
6. **Fail Fast**: Validate inputs early, throw meaningful exceptions
7. **DRY**: Don't repeat yourself - use base classes, helpers, extensions
8. **Security**: Never hardcode secrets, always use configuration

## Quick Reference

### Import Statements
```csharp
// Refit
using Refit;

// JSON
using System.Text.Json.Serialization;

// Testing
using Xunit;
using AwesomeAssertions;

// Configuration
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// Logging
using Microsoft.Extensions.Logging;
```

### Common Attributes
- `[Get("path")]`, `[Post("path")]`, `[Put("path")]`, `[Delete("path")]` - HTTP methods
- `[AliasAs("name")]` - Parameter/property name mapping
- `[Body]` - Request body parameter
- `[Query]` - Query string parameter
- `[JsonPropertyName("name")]` - JSON property name
- `[Fact]` - Single test
- `[Theory]` - Parameterized test
- `[InlineData(...)]` - Test parameter data

## Contact & Support

For questions about:
- **Project patterns**: Reference Toggl.Api implementation
- **API behavior**: Check OpenAPI spec and official docs
- **Test patterns**: Review existing test classes
- **Build issues**: Check GitHub Actions workflows

---

**Remember**: When in doubt, look at how it's done in Toggl.Api! This project aims to be a sibling, not a fork - same quality, same patterns, different API.

## Critical Reminders for Implementation

### JSON Naming Convention
- **ALWAYS use snake_case** for JSON property names: `[JsonPropertyName("is_archived")]`
- This is different from Toggl.Api which uses camelCase
- Already configured in SolidtimeClient JsonSerializerOptions

### ID Types
- **ALWAYS use `string`** for IDs (they are UUIDs)
- Exception: API token IDs may be non-UUID strings
- Different from Toggl.Api which uses `long` numeric IDs

### Response Wrappers
- Single resource: Return `DataWrapper<T>`
- Collection: Return `DataWrapper<List<T>>`
- Paginated: Return `PaginatedResponse<T>` with Links and Meta

### URL Structure
- Pattern: `/v1/organizations/{organization}/{resource}`
- Use `[AliasAs("organization")]` for organization parameter
- Organization ID is a UUID string

### XUnit v3 Namespaces
- `ITestOutputHelper` is in `Xunit` (not `Xunit.Abstractions`)
- Use primary constructors in test classes
- Inherit from `TestBed<Fixture>` (not `TestBedWithDI`)

---
