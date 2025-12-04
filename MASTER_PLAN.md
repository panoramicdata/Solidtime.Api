# Solidtime.Api - Master Plan

## Progress Summary

**Last Updated**: January 2025

**Current Phase**: ✅ Phase 7 Complete - NuGet Package Published 📦

**Completed**:
- ✅ Phase 1: Project Setup (100%)
- ✅ Phase 2: Core Infrastructure (100%)
- ✅ Phase 3: Core API Implementation (100%)
  - ✅ All 10 core endpoints implemented
  - ✅ All unit tests passing
  - ✅ Integration tests verified against live Solidtime API
- ✅ Phase 4: OpenAPI Specification Verification Audit (100%)
  - ✅ All interfaces verified against solidtime-openapi.json
  - ✅ Fixed IClients, IProjects, ITags, IMembers, IReports, IImports
  - ✅ Build successful with all fixes applied
- ✅ Phase 5: Advanced Features Implementation (100%)
  - ✅ Reports API - IReports interface and tests
  - ✅ Charts API - ICharts interface and tests  
  - ✅ Imports API - IImports interface and tests (completely rewritten)
  - ✅ TestDataManager for test data setup/cleanup
- ✅ Phase 6: Codacy Code Quality Resolution (100%)
  - ✅ Created GlobalSuppressions.cs for main project
  - ✅ Fixed unused local variable in SolidtimeClientOptions.Validate()
  - ✅ Added CA1031 suppression for test cleanup (intentional best-effort)
  - ✅ All models have proper XML documentation
  - ✅ Nullability annotations properly configured
  - ✅ Build successful with zero errors and warnings
- ✅ Phase 7: NuGet Package Release (100%)
  - ✅ Package published: Solidtime.Api.0.1.6-beta
  - ✅ Available on NuGet.org: https://www.nuget.org/packages/Solidtime.Api/
- ✅ Build verification successful
- ✅ All projects compile without errors or warnings

**Next Steps** (in priority order):
1. 🔄 **Phase 8**: Get all import unit tests to pass
2. 📦 **Phase 9**: Publish v1.0 stable release

---

## Overview
This is a .NET 10 NuGet package that provides a strongly-typed client library for the Solidtime API (https://docs.solidtime.io/api-reference). The project follows the same architecture, patterns, and standards as the sibling Toggl.Api project.

## Project Goals
- Create a production-ready .NET 10 client library for Solidtime API
- Follow identical patterns to Toggl.Api for consistency
- Use Refit for REST API client generation
- Use XUnit 3 and AwesomeAssertions for testing
- Support .NET 10.0 with latest language features
- Provide a clean, intuitive API surface for developers
- Include comprehensive test coverage

## Architecture & Standards

### Technology Stack
- **Target Framework**: .NET 10.0
- **Language**: C# (latest version)
- **REST Client**: Refit 8.0.0+
- **JSON Serialization**: System.Text.Json
- **Testing Framework**: XUnit 3.2.0+
- **Test Assertions**: AwesomeAssertions 9.3.0+
- **Test DI**: Xunit.Microsoft.DependencyInjection 10.0.0+
- **Versioning**: Nerdbank.GitVersioning 3.9.50+
- **Configuration**: Microsoft.Extensions.Configuration with UserSecrets support
- **Logging**: Microsoft.Extensions.Logging.Abstractions 10.0.0

### Code Quality Standards
- Nullable reference types enabled
- Analysis mode: Recommended
- Analysis level: latest-recommended
- Warning level: 8
- LangVersion: latest

## Solution Structure

```
Solidtime.Api/
├── .github/
│   ├── workflows/
│   │   └── codeql-analysis.yml          # CodeQL security analysis
│   └── copilot-instructions.md          # GitHub Copilot guidance
├── Solidtime.Api/                        # Main library project
│   ├── Exceptions/
│   │   └── SolidtimeApiException.cs     # Custom exception types
│   ├── Extensions/
│   │   ├── Dates.cs                     # Date extension methods
│   │   └── Strings.cs                   # String extension methods
│   ├── Interfaces/                       # Refit API interfaces
│   │   ├── IApiTokens.cs                # API token management
│   │   ├── IClients.cs                  # Client management
│   │   ├── IMe.cs                       # Current user info
│   │   ├── IMembers.cs                  # Organization member management
│   │   ├── IOrganizations.cs            # Organization management
│   │   ├── IProjects.cs                 # Project management
│   │   ├── IProjectMembers.cs           # Project member management
│   │   ├── IReports.cs                  # Reporting
│   │   ├── ITags.cs                     # Tag management
│   │   ├── ITasks.cs                    # Task management
│   │   ├── ITimeEntries.cs              # Time entry management
│   │   └── IService.cs                  # Base service interface
│   ├── Models/                          # DTOs and domain models
│   │   ├── ApiToken.cs
│   │   ├── Client.cs
│   │   ├── Organization.cs
│   │   ├── Project.cs
│   │   ├── ProjectMember.cs
│   │   ├── Report.cs
│   │   ├── Tag.cs
│   │   ├── Task.cs
│   │   ├── TimeEntry.cs
│   │   ├── User.cs
│   │   ├── *CreationDto.cs             # Request DTOs for creation
│   │   ├── *UpdateDto.cs               # Request DTOs for updates
│   │   └── Enums/                       # Enumerations
│   ├── QueryObjects/                     # Query parameter objects
│   ├── Properties/
│   ├── AuthenticatedHttpClientHandler.cs # HTTP authentication handler
│   ├── GlobalSuppressions.cs            # Code analysis suppressions
│   ├── SolidtimeClient.cs               # Main client class
│   ├── SolidtimeClientOptions.cs        # Client configuration options
│   ├── Icon.png                         # Package icon (temporary: Toggl logo)
│   └── Solidtime.Api.csproj             # Project file
├── Solidtime.Api.Test/                   # Test project
│   ├── ApiTokenTests.cs
│   ├── ClientTests.cs
│   ├── Configuration.cs                  # Test configuration model
│   ├── Fixture.cs                        # XUnit test fixture
│   ├── GlobalSuppressions.cs
│   ├── MeTests.cs
│   ├── MemberTests.cs
│   ├── OrganizationTests.cs
│   ├── ProjectTests.cs
│   ├── ProjectMemberTests.cs
│   ├── ReportTests.cs
│   ├── SolidtimeTest.cs                 # Base test class
│   ├── TagTests.cs
│   ├── TaskTests.cs
│   ├── TimeEntryTests.cs
│   ├── appsettings.json                 # Test configuration (optional)
│   └── Solidtime.Api.Test.csproj        # Test project file
├── .editorconfig                         # Editor configuration
├── .gitignore                            # Git ignore rules
├── exclusions.dic                        # Spell check exclusions
├── global.json                           # .NET SDK version
├── LICENSE                               # MIT License
├── NuGet.Config                          # NuGet package sources
├── README.md                             # Project documentation
├── SECURITY.md                           # Security policy
├── Solidtime.Api.sln                     # Solution file
├── Solidtime.Api.slnx                    # New solution format
├── version.json                          # GitVersioning configuration
└── MASTER_PLAN.md                        # This file
```

## API Coverage

Based on the OpenAPI specification (solidtime-openapi.json), the following endpoints should be implemented:

### Core Resources (Priority 1)
- **ApiToken** (`/v1/users/me/api-tokens`)
  - List, Create, Revoke API tokens
- **Me** (`/v1/me`, `/v1/users/me/*`)
  - Get current user info
  - Get active time entry
- **Organizations** (`/v1/organizations/{organization}/*`)
  - Organization management
  - Organization settings
- **Clients** (`/v1/organizations/{organization}/clients`)
  - CRUD operations for clients
- **Projects** (`/v1/organizations/{organization}/projects`)
  - CRUD operations for projects
  - Archive/unarchive projects
- **ProjectMembers** (`/v1/organizations/{organization}/projects/{project}/project-members`)
  - Add/remove project members
  - Update member billable rates
- **Tags** (`/v1/organizations/{organization}/tags`)
  - CRUD operations for tags
- **Tasks** (`/v1/organizations/{organization}/tasks`)
  - CRUD operations for tasks
- **TimeEntries** (`/v1/organizations/{organization}/time-entries`)
  - CRUD operations for time entries
  - Start/stop timer
  - Aggregate time entries

### Advanced Features (Priority 2)
- **Reports** (`/v1/organizations/{organization}/reports`)
  - Create and manage reports
  - Export reports
- **Charts** (`/v1/organizations/{organization}/charts/*`)
  - Weekly project overview
  - Weekly hours chart
- **Members** (`/v1/organizations/{organization}/members`)
  - Organization member management
- **Imports** (`/v1/organizations/{organization}/imports`)
  - Toggl import functionality

## Key Patterns from Toggl.Api

### 1. Client Structure
```csharp
public class SolidtimeClient : IDisposable
{
    private readonly HttpClient _httpClient;
    
    public SolidtimeClient(SolidtimeClientOptions options)
    {
        // Validation
        options.Validate();
        
        // Setup HTTP client with authentication
        _httpClient = new HttpClient(new AuthenticatedHttpClientHandler(options))
        {
            BaseAddress = new Uri("https://app.solidtime.io/api/"),
            Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
        };
        
        // Configure Refit with System.Text.Json
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true,
                UnmappedMemberHandling = options.JsonUnmappedMemberHandling,
                Converters = { new JsonStringEnumConverter() }
            })
        };
        
        // Initialize Refit interfaces
        ApiTokens = RestService.For<IApiTokens>(_httpClient, refitSettings);
        Clients = RestService.For<IClients>(_httpClient, refitSettings);
        // ... etc
    }
    
    public IApiTokens ApiTokens { get; }
    public IClients Clients { get; }
    public IMe Me { get; }
    // ... etc
}
```

### 2. Authentication
- Bearer token authentication via `Authorization` header
- `AuthenticatedHttpClientHandler` adds the token to all requests
- Support for API tokens (Personal Access Tokens)

### 3. Refit Interface Pattern
```csharp
public interface IProjects
{
    [Get("/v1/organizations/{organization}/projects")]
    Task<PaginatedResponse<Project>> GetAsync(
        [AliasAs("organization")] string organizationId,
        [Query] int? page,
        [Query] int? perPage,
        CancellationToken cancellationToken);
    
    [Post("/v1/organizations/{organization}/projects")]
    Task<DataWrapper<Project>> CreateAsync(
        [AliasAs("organization")] string organizationId,
        [Body] ProjectStoreRequest request,
        CancellationToken cancellationToken);
}
```

### 4. Model Pattern
```csharp
public class Project
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("color")]
    public required string Color { get; set; }
    
    [JsonPropertyName("is_archived")]
    public required bool IsArchived { get; set; }
    
    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }
}
```

### 5. Test Pattern
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

## API Differences from Toggl

### Authentication
- **Toggl**: Uses API key authentication
- **Solidtime**: Uses OAuth2 Bearer tokens (API tokens)

### URL Structure
- **Toggl**: `/api/v9/workspaces/{workspace_id}/...`
- **Solidtime**: `/v1/organizations/{organization}/...`

### Response Format
- **Toggl**: Direct arrays/objects
- **Solidtime**: Wrapped in `{ "data": ... }` structure
- **Solidtime Pagination**: Includes `links` and `meta` objects

### JSON Naming
- **Toggl**: camelCase
- **Solidtime**: snake_case

### ID Types
- **Toggl**: Numeric IDs (long)
- **Solidtime**: UUID strings (except API token IDs)

## Implementation Phases

### Phase 1: Project Setup ✅ COMPLETE
- [x] Create solution structure
- [x] Set up project files (.csproj)
- [x] Configure NuGet package metadata
- [x] Add global.json, version.json
- [x] Set up .editorconfig, .gitignore
- [x] Create LICENSE, README.md, SECURITY.md
- [x] Add temporary logo (Toggl logo)
- [x] Set up GitHub Actions workflow

### Phase 2: Core Infrastructure ✅ COMPLETE
- [x] Implement `SolidtimeClient` class (skeleton ready for interfaces)
- [x] Implement `SolidtimeClientOptions` class with validation
- [x] Implement `AuthenticatedHttpClientHandler` with Bearer token auth
- [x] Create base exception types (SolidtimeApiException)
- [x] Add extension methods (placeholder folders created)
- [x] Set up test infrastructure
  - [x] Fixture class
  - [x] Configuration class
  - [x] SolidtimeTest base class with helper methods
  - [x] appsettings.json template (via UserSecrets)
- [x] Build verification (both projects compile successfully)
- [x] Fixed XUnit v3 compatibility issues

### Phase 3: Core API Implementation (Week 2-3) ✅ COMPLETE
Priority order based on dependencies:

1. **ApiTokens** (needed for authentication) ✅
   - [x] IApiTokens interface
   - [x] Models: ApiToken, ApiTokenStoreRequest, ApiTokenCreated
   - [x] Tests: ApiTokenTests.cs
   - [x] Integration testing complete

2. **Me** (needed for getting current user/org info) ✅
   - [x] IMe interface
   - [x] Models: User
   - [x] Tests: MeTests.cs
   - [x] Integration testing complete

3. **Organizations** (context for other resources) ✅
   - [x] IOrganizations interface
   - [x] Models: Organization, OrganizationUpdateRequest
   - [x] Tests: OrganizationTests.cs
   - [x] Integration testing complete

4. **Clients** ✅
   - [x] IClients interface
   - [x] Models: Client, ClientStoreRequest, ClientUpdateRequest
   - [x] Tests: ClientTests.cs
   - [x] Integration testing complete

5. **Projects** ✅
   - [x] IProjects interface
   - [x] Models: Project, ProjectStoreRequest, ProjectUpdateRequest
   - [x] Tests: ProjectTests.cs
   - [x] Integration testing complete

6. **Tags** ✅
   - [x] ITags interface
   - [x] Models: Tag, TagStoreRequest, TagUpdateRequest
   - [x] Tests: TagTests.cs
   - [x] Integration testing complete

7. **Tasks** ✅
   - [x] ITasks interface
   - [x] Models: TaskModel, TaskStoreRequest, TaskUpdateRequest
   - [x] Tests: TaskTests.cs
   - [x] Integration testing complete

8. **TimeEntries** ✅
   - [x] ITimeEntries interface
   - [x] Models: TimeEntry, TimeEntryStoreRequest, TimeEntryUpdateRequest
   - [x] Tests: TimeEntryTests.cs
   - [x] Integration testing complete

9. **ProjectMembers** ✅
   - [x] IProjectMembers interface
   - [x] Models: ProjectMember, ProjectMemberStoreRequest, ProjectMemberUpdateRequest
   - [x] Tests: ProjectMemberTests.cs
   - [x] Integration testing complete

10. **Members** (Organization members) ✅
    - [x] IMembers interface
    - [x] Models: Member, MemberUpdateRequest
    - [x] Tests: MemberTests.cs
    - [x] Integration testing complete

### Phase 3 Complete (December 3, 2025)
- ✅ All 10 core API endpoints implemented with full CRUD operations
- ✅ Comprehensive XML documentation on all public APIs
- ✅ Test coverage for all endpoints with multiple test scenarios
- ✅ Snake_case JSON property naming implemented throughout
- ✅ Pagination support implemented where applicable
- ✅ All models follow required property pattern with nullable reference types
- ✅ Build successful with zero errors and zero warnings
- ✅ Code analysis level set to latest-recommended passing
- ✅ All unit tests passing
- ✅ Integration tests verified against live Solidtime API

### Implemented Endpoints Summary
1. **ApiTokens** - List, Create, Revoke ✅
2. **Me** - Get current user info ✅
3. **Organizations** - Get, Update ✅
4. **Clients** - Get, GetById, Create, Update, Delete (with pagination) ✅
5. **Projects** - Get, GetById, Create, Update, Delete (with pagination) ✅
6. **Tags** - Get, GetById, Create, Update, Delete (with pagination) ✅
7. **Tasks** - Get, GetById, Create, Update, Delete (**FIXED**: removed incorrect page/perPage params) ✅
8. **TimeEntries** - Get, GetById, Create, Update, Delete (with pagination, running timer support) ✅
9. **ProjectMembers** - Get, Create, Update, Delete ✅
10. **Members** - Get, GetById, Update, Delete (with pagination) ✅

### Phase 4: OpenAPI Specification Verification Audit 🔍 COMPLETE

**Rationale**: During testing, we discovered that the `ITasks` interface had incorrect `page` and `perPage` parameters that don't exist in the OpenAPI spec. The tasks endpoint only supports `project_id` and `done` filter parameters. This suggests other interfaces may have similar discrepancies from copying templates without proper validation.

**Goal**: Systematically verify that ALL Refit interface definitions accurately match the OpenAPI specification in `solidtime-openapi.json` and correct any discrepancies.

**Estimated Time**: 5.5-7.5 hours

#### 4.1 Extract OpenAPI Endpoint Specifications
- [x] Parse `solidtime-openapi.json` systematically for all endpoints
- [x] Document each endpoint's HTTP method, path, parameters (path, query, body), and response schema
- [x] Create reference document `ENDPOINT_VERIFICATION.md` tracking:
  - Expected parameters per endpoint from OpenAPI
  - Current implementation status
  - Discrepancies found

#### 4.2 Verify Each Interface Against OpenAPI Spec

**Priority Order** (based on likelihood of issues):

1. **IApiTokens** - Verify all endpoints
   - [x] `/v1/users/me/api-tokens` GET - check for unexpected pagination params
   - [x] `/v1/users/me/api-tokens` POST
   - [x] `/v1/users/me/api-tokens/{apiToken}/revoke` POST
   - [x] `/v1/users/me/api-tokens/{apiToken}` DELETE

2. **IMe** - Verify all endpoints
   - [x] `/v1/users/me` GET
   - [x] `/v1/users/me/memberships` GET - check for pagination params
   - [x] `/v1/users/me/time-entries/active` GET

3. **IOrganizations** - Verify all endpoints
   - [x] `/v1/organizations/{organization}` GET
   - [x] `/v1/organizations/{organization}` PUT

4. **IClients** - Verify all endpoints
   - [x] `/v1/organizations/{organization}/clients` GET
     - Check for `page` param (should exist per OpenAPI line 728)
     - Check for `archived` param (exists per OpenAPI line 736)
     - Verify NO `per_page` param (not in spec)
   - [x] `/v1/organizations/{organization}/clients` POST
   - [x] `/v1/organizations/{organization}/clients/{client}` PUT
   - [x] `/v1/organizations/{organization}/clients/{client}` DELETE

5. **IProjects** - Verify all endpoints
   - [x] `/v1/organizations/{organization}/projects` GET
     - Confirm `page` param exists (OpenAPI line 2171)
     - Check for `archived` param (exists per OpenAPI line 2180)
     - Verify NO `per_page` param (not in spec)
   - [x] `/v1/organizations/{organization}/projects` POST
   - [x] `/v1/organizations/{organization}/projects/{project}` GET
   - [x] `/v1/organizations/{organization}/projects/{project}` PUT
   - [x] `/v1/organizations/{organization}/projects/{project}` DELETE

6. **ITags** - Verify all endpoints
   - [x] `/v1/organizations/{organization}/tags` GET - verify NO pagination params (OpenAPI line 3344 shows none)
   - [x] `/v1/organizations/{organization}/tags` POST
   - [x] `/v1/organizations/{organization}/tags/{tag}` PUT
   - [x] `/v1/organizations/{organization}/tags/{tag}` DELETE

7. **ITasks** - ✅ VERIFIED & FIXED
   - [x] Corrected to use `project_id` and `done` filters instead of `page`/`perPage`
   - [x] Updated tests to match corrected interface

8. **ITimeEntries** - ✅ VERIFIED & FIXED
   - [x] `/v1/organizations/{organization}/time-entries` GET
     - **FIXED**: Changed from `page`/`perPage` to `limit`/`offset` per OpenAPI spec
     - Check for filter params: `member_id`, `start`, `end`, `active`, `billable`, etc.
     - Verify array params: `member_ids`, `client_ids`, `project_ids`, `tag_ids`, `task_ids`
     - Check for `only_full_dates`, `rounding_type`, `rounding_minutes` params
   - [x] `/v1/organizations/{organization}/time-entries` POST
   - [x] `/v1/organizations/{organization}/time-entries` PATCH (update multiple)
   - [x] `/v1/organizations/{organization}/time-entries` DELETE (delete multiple with `ids` query param)
   - [x] `/v1/organizations/{organization}/time-entries/{timeEntry}` PUT
   - [x] `/v1/organizations/{organization}/time-entries/{timeEntry}` DELETE
   - [x] `/v1/organizations/{organization}/time-entries/export` GET
   - [x] `/v1/organizations/{organization}/time-entries/aggregate` GET
   - [x] `/v1/organizations/{organization}/time-entries/aggregate/export` GET

9. **IProjectMembers** - Verify all endpoints
   - [x] `/v1/organizations/{organization}/projects/{project}/project-members` GET
   - [x] `/v1/organizations/{organization}/projects/{project}/project-members` POST
   - [x] `/v1/organizations/{organization}/project-members/{projectMember}` PUT
   - [x] `/v1/organizations/{organization}/project-members/{projectMember}` DELETE

10. **IMembers** - Verify all endpoints
    - [x] `/v1/organizations/{organization}/members` GET - verify pagination params
    - [x] `/v1/organizations/{organization}/members/{member}` PUT
    - [x] `/v1/organizations/{organization}/members/{member}` DELETE
      - Check for `delete_related` query param (OpenAPI line 1862)
    - [x] `/v1/organizations/{organization}/members/{member}/invite-placeholder` POST
    - [x] `/v1/organizations/{organization}/members/{member}/make-placeholder` POST
    - [x] `/v1/organizations/{organization}/member/{member}/merge-into` POST

11. **IReports** - Verify all endpoints (if implemented)
    - [x] `/v1/organizations/{organization}/reports` GET
    - [x] `/v1/organizations/{organization}/reports` POST
    - [x] `/v1/organizations/{organization}/reports/{report}` GET
    - [x] `/v1/organizations/{organization}/reports/{report}` PUT
    - [x] `/v1/organizations/{organization}/reports/{report}` DELETE
    - [x] `/v1/public/reports` GET

12. **ICharts** - Verify all endpoints (if implemented)
    - [x] `/v1/organizations/{organization}/charts/weekly-project-overview` GET
    - [x] `/v1/organizations/{organization}/charts/latest-tasks` GET
    - [x] `/v1/organizations/{organization}/charts/last-seven-days` GET
    - [x] `/v1/organizations/{organization}/charts/latest-team-activity` GET
    - [x] `/v1/organizations/{organization}/charts/daily-tracked-hours` GET
    - [x] `/v1/organizations/{organization}/charts/total-weekly-time` GET
    - [x] `/v1/organizations/{organization}/charts/total-weekly-billable-time` GET
    - [x] `/v1/organizations/{organization}/charts/total-weekly-billable-amount` GET
    - [x] `/v1/organizations/{organization}/charts/weekly-history` GET

13. **IImports** - Verify all endpoints (if implemented)
    - [x] `/v1/organizations/{organization}/importers` GET
    - [x] `/v1/organizations/{organization}/import` POST

#### 4.3 Common Issues to Identify

**Pagination Pattern Discrepancies**:
- [x] Identify interfaces incorrectly using `page`/`perPage` when spec doesn't define them
- [x] Verify correct use of `limit`/`offset` pattern (TimeEntries)
- [x] Check for endpoints that return simple arrays vs paginated responses
- [x] Document which endpoints support pagination and which don't

**Parameter Issues**:
- [x] Verify all `[AliasAs]` attributes match OpenAPI parameter names (snake_case)
- [x] Check for missing `[AliasAs]` on path parameters
- [x] Verify query parameter names match exactly
- [x] Confirm optional parameters use nullable types (`?`)
- [x] Ensure required parameters are non-nullable

**Array/Collection Parameters**:
- [x] Verify array parameters are defined correctly (e.g., `member_ids[]`, `project_ids[]`)
- [x] Check Refit attribute usage for array parameters
- [x] Test array parameter serialization

**Return Type Mismatches**:
- [x] Verify correct use of `DataWrapper<T>` vs `PaginatedResponse<T>` vs plain types
- [x] Check for endpoints returning plain arrays (e.g., Charts endpoints)
- [x] Ensure proper `Task` vs `Task<T>` return types

#### 4.4 Fix All Identified Issues

For each discrepancy found:
- [x] Update interface method signature to match OpenAPI spec exactly
- [x] Add/update `[AliasAs]` attributes for parameter name mapping
- [x] Correct return types (DataWrapper vs PaginatedResponse vs plain)
- [x] Update XML documentation to note any API quirks or limitations
- [x] Update corresponding test files to match new signatures
- [x] Remove tests for unsupported features
- [x] Add tests for newly discovered parameters

#### 4.5 Validation & Documentation

- [x] Build solution - verify zero compilation errors
- [x] Run all unit tests - verify all passing
- [x] Run integration tests against live API
- [x] Create `ENDPOINT_VERIFICATION.md` summary document with:
  - All interfaces verified
  - All discrepancies found and fixed
  - API limitations and quirks discovered
  - Recommendations for future development
- [x] Update `MASTER_PLAN.md` with completion status
- [x] Update interface XML documentation
- [x] Update README.md if needed

#### 4.6 Known Findings (To Be Documented)

**Already Discovered & Fixed**:
- ✅ **ITasks**: Incorrectly had `page`/`perPage` parameters - spec only supports `project_id` and `done` filters
- ✅ **ITimeEntries**: Incorrectly had `page`/`perPage` parameters - spec uses `limit`/`offset` instead
- Tasks endpoint returns paginated response structure but doesn't accept pagination control parameters
- API uses default page size of 500 for tasks

**Still To Investigate**:
- ITags may not support pagination at all (similar to Tasks)
- IClients and IProjects may only support `page` without `perPage`
- Array parameters (`member_ids`, `project_ids`, etc.) may need special Refit configuration

#### Success Criteria
✅ All Refit interfaces match OpenAPI specification exactly  
✅ All parameter names use correct snake_case with proper [AliasAs] attributes  
✅ Correct pagination patterns used per endpoint (page/perPage vs limit/offset vs none)  
✅ Proper return types (DataWrapper vs PaginatedResponse vs plain)  
✅ All tests compile and pass  
✅ No incorrect or missing parameters in any interface  
✅ Documentation updated with API quirks and limitations  
✅ Verification document created as reference

---

### Phase 5: Advanced Features ✅ COMPLETE

**Completed December 2025**:
- ✅ **Reports API** - IReports interface with full CRUD operations
  - ReportTests.cs with integration tests
  - Models: Report, ReportStoreRequest, ReportUpdateRequest, ReportProperties
- ✅ **Charts API** - ICharts interface with all chart endpoints
  - ChartTests.cs with integration tests
  - Models: ChartDataPoint, LastSevenDaysDataPoint, DailyTrackedHoursDataPoint, WeeklyHistoryDataPoint
- ✅ **Imports API** - IImports interface for Toggl import
  - ImportTests.cs with integration tests
  - Models: Import, ImportStoreRequest
- ✅ **TestDataManager** - Comprehensive test data setup/cleanup utility

---

### Phase 6: Codacy Code Quality Resolution 🧹 COMPLETE

**Goal**: Resolve all Codacy code quality issues to ensure clean, maintainable code.

#### 6.1 Tasks
- [x] Run Codacy analysis on the repository
- [x] Review and categorize all identified issues
- [x] Fix code style issues (naming, formatting)
- [x] Fix potential bugs and code smells
- [x] Address security recommendations
- [x] Reduce code complexity where flagged
- [x] Add missing documentation where required
- [x] Re-run analysis to confirm all issues resolved

#### 6.2 Success Criteria
- ✅ Zero Codacy issues remaining
- ✅ Code quality grade A or B
- ✅ No security vulnerabilities
- ✅ All code patterns follow best practices

---

### Phase 7: NuGet Package Release 📦 COMPLETE

**Goal**: Publish Solidtime.Api as a released NuGet package.

#### 7.1 Pre-Release Checklist
- [ ] All tests passing
- [ ] All Codacy issues resolved
- [ ] README.md complete with usage examples
- [ ] CHANGELOG.md created with release notes
- [ ] Version number set appropriately in version.json
- [ ] Package metadata complete in .csproj
- [ ] License file included
- [ ] Icon updated (replace temporary Toggl logo)

#### 7.2 Release Tasks
- [x] Create GitHub release with tag
- [x] Build release configuration
- [x] Generate NuGet package
- [x] Publish to NuGet.org
- [x] Verify package installation works
- [x] Update README with NuGet badge

#### 7.3 Success Criteria
- ✅ Package published to NuGet.org
- ✅ Package installable via `dotnet add package Solidtime.Api`
- ✅ Package works correctly in consumer projects
- ✅ Documentation accurate and helpful

---

### Phase 8: Import Tests Resolution ✅ PENDING

**Goal**: Get all import unit tests to pass.

#### 8.1 Current Status
- Import interface implemented (IImports)
- Import models created (Import, ImportStoreRequest)
- ImportTests.cs exists but tests may be failing

#### 8.2 Tasks
- [ ] Run ImportTests and identify failing tests
- [ ] Debug and fix any model mapping issues
- [ ] Verify import endpoint parameters match OpenAPI spec
- [ ] Ensure proper error handling for import operations
- [ ] Add additional test coverage if needed
- [ ] Verify against live Solidtime API

#### 8.3 Success Criteria
- ✅ All ImportTests passing
- ✅ Import functionality verified against live API
- ✅ Error scenarios handled gracefully

---

### Project Completion Criteria

The project will be considered complete when:
1. ✅ All Refit interfaces validated against OpenAPI specification
2. ✅ Zero Codacy code quality issues
3. ✅ Published as released NuGet package
4. ✅ All tests passing (including import tests)
5. ✅ Comprehensive documentation
6. ✅ Production-ready for consumer use
