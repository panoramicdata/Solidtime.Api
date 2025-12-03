# Solidtime.Api - Master Plan

## Progress Summary

**Last Updated**: December 3, 2025

**Current Phase**: Phase 4 - OpenAPI Specification Verification Audit 🔍

**Completed**:
- ✅ Phase 1: Project Setup (100%)
- ✅ Phase 2: Core Infrastructure (100%)
- ✅ Phase 3: Core API Implementation (100%)
  - ✅ All 10 core endpoints implemented
  - ✅ All unit tests passing
  - ✅ Integration tests verified against live Solidtime API
- ✅ Build verification successful
- ✅ All projects compile without errors or warnings

**Next Steps**: 
- 🔍 Phase 4: Verify all Refit interfaces match OpenAPI specification exactly (CRITICAL - discovered ITasks had incorrect parameters)
- 🚀 Phase 5: Implement Advanced Features (Reports, Charts, Imports)

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

### Phase 4: OpenAPI Specification Verification Audit 🔍 IN PROGRESS

**Rationale**: During testing, we discovered that the `ITasks` interface had incorrect `page` and `perPage` parameters that don't exist in the OpenAPI spec. The tasks endpoint only supports `project_id` and `done` filter parameters. This suggests other interfaces may have similar discrepancies from copying templates without proper validation.

**Goal**: Systematically verify that ALL Refit interface definitions accurately match the OpenAPI specification in `solidtime-openapi.json` and correct any discrepancies.

**Estimated Time**: 5.5-7.5 hours

#### 4.1 Extract OpenAPI Endpoint Specifications
- [ ] Parse `solidtime-openapi.json` systematically for all endpoints
- [ ] Document each endpoint's HTTP method, path, parameters (path, query, body), and response schema
- [ ] Create reference document `ENDPOINT_VERIFICATION.md` tracking:
  - Expected parameters per endpoint from OpenAPI
  - Current implementation status
  - Discrepancies found

#### 4.2 Verify Each Interface Against OpenAPI Spec

**Priority Order** (based on likelihood of issues):

1. **IApiTokens** - Verify all endpoints
   - [ ] `/v1/users/me/api-tokens` GET - check for unexpected pagination params
   - [ ] `/v1/users/me/api-tokens` POST
   - [ ] `/v1/users/me/api-tokens/{apiToken}/revoke` POST
   - [ ] `/v1/users/me/api-tokens/{apiToken}` DELETE

2. **IMe** - Verify all endpoints
   - [ ] `/v1/users/me` GET
   - [ ] `/v1/users/me/memberships` GET - check for pagination params
   - [ ] `/v1/users/me/time-entries/active` GET

3. **IOrganizations** - Verify all endpoints
   - [ ] `/v1/organizations/{organization}` GET
   - [ ] `/v1/organizations/{organization}` PUT

4. **IClients** - Verify all endpoints
   - [ ] `/v1/organizations/{organization}/clients` GET
     - Check for `page` param (should exist per OpenAPI line 728)
     - Check for `archived` param (exists per OpenAPI line 736)
     - Verify NO `per_page` param (not in spec)
   - [ ] `/v1/organizations/{organization}/clients` POST
   - [ ] `/v1/organizations/{organization}/clients/{client}` PUT
   - [ ] `/v1/organizations/{organization}/clients/{client}` DELETE

5. **IProjects** - Verify all endpoints
   - [ ] `/v1/organizations/{organization}/projects` GET
     - Confirm `page` param exists (OpenAPI line 2171)
     - Check for `archived` param (exists per OpenAPI line 2180)
     - Verify NO `per_page` param (not in spec)
   - [ ] `/v1/organizations/{organization}/projects` POST
   - [ ] `/v1/organizations/{organization}/projects/{project}` GET
   - [ ] `/v1/organizations/{organization}/projects/{project}` PUT
   - [ ] `/v1/organizations/{organization}/projects/{project}` DELETE

6. **ITags** - Verify all endpoints
   - [ ] `/v1/organizations/{organization}/tags` GET - verify NO pagination params (OpenAPI line 3344 shows none)
   - [ ] `/v1/organizations/{organization}/tags` POST
   - [ ] `/v1/organizations/{organization}/tags/{tag}` PUT
   - [ ] `/v1/organizations/{organization}/tags/{tag}` DELETE

7. **ITasks** - ✅ VERIFIED & FIXED
   - [x] Corrected to use `project_id` and `done` filters instead of `page`/`perPage`
   - [x] Updated tests to match corrected interface

8. **ITimeEntries** - HIGH PRIORITY (complex endpoint)
   - [ ] `/v1/organizations/{organization}/time-entries` GET
     - **CRITICAL**: Uses `limit` and `offset` (OpenAPI lines 4004-4020), NOT page/perPage
     - Check for filter params: `member_id`, `start`, `end`, `active`, `billable`, etc.
     - Verify array params: `member_ids`, `client_ids`, `project_ids`, `tag_ids`, `task_ids`
     - Check for `only_full_dates`, `rounding_type`, `rounding_minutes` params
   - [ ] `/v1/organizations/{organization}/time-entries` POST
   - [ ] `/v1/organizations/{organization}/time-entries` PATCH (update multiple)
   - [ ] `/v1/organizations/{organization}/time-entries` DELETE (delete multiple with `ids` query param)
   - [ ] `/v1/organizations/{organization}/time-entries/{timeEntry}` PUT
   - [ ] `/v1/organizations/{organization}/time-entries/{timeEntry}` DELETE
   - [ ] `/v1/organizations/{organization}/time-entries/export` GET
   - [ ] `/v1/organizations/{organization}/time-entries/aggregate` GET
   - [ ] `/v1/organizations/{organization}/time-entries/aggregate/export` GET

9. **IProjectMembers** - Verify all endpoints
   - [ ] `/v1/organizations/{organization}/projects/{project}/project-members` GET
   - [ ] `/v1/organizations/{organization}/projects/{project}/project-members` POST
   - [ ] `/v1/organizations/{organization}/project-members/{projectMember}` PUT
   - [ ] `/v1/organizations/{organization}/project-members/{projectMember}` DELETE

10. **IMembers** - Verify all endpoints
    - [ ] `/v1/organizations/{organization}/members` GET - verify pagination params
    - [ ] `/v1/organizations/{organization}/members/{member}` PUT
    - [ ] `/v1/organizations/{organization}/members/{member}` DELETE
      - Check for `delete_related` query param (OpenAPI line 1862)
    - [ ] `/v1/organizations/{organization}/members/{member}/invite-placeholder` POST
    - [ ] `/v1/organizations/{organization}/members/{member}/make-placeholder` POST
    - [ ] `/v1/organizations/{organization}/member/{member}/merge-into` POST

11. **IReports** - Verify all endpoints (if implemented)
    - [ ] `/v1/organizations/{organization}/reports` GET
    - [ ] `/v1/organizations/{organization}/reports` POST
    - [ ] `/v1/organizations/{organization}/reports/{report}` GET
    - [ ] `/v1/organizations/{organization}/reports/{report}` PUT
    - [ ] `/v1/organizations/{organization}/reports/{report}` DELETE
    - [ ] `/v1/public/reports` GET

12. **ICharts** - Verify all endpoints (if implemented)
    - [ ] `/v1/organizations/{organization}/charts/weekly-project-overview` GET
    - [ ] `/v1/organizations/{organization}/charts/latest-tasks` GET
    - [ ] `/v1/organizations/{organization}/charts/last-seven-days` GET
    - [ ] `/v1/organizations/{organization}/charts/latest-team-activity` GET
    - [ ] `/v1/organizations/{organization}/charts/daily-tracked-hours` GET
    - [ ] `/v1/organizations/{organization}/charts/total-weekly-time` GET
    - [ ] `/v1/organizations/{organization}/charts/total-weekly-billable-time` GET
    - [ ] `/v1/organizations/{organization}/charts/total-weekly-billable-amount` GET
    - [ ] `/v1/organizations/{organization}/charts/weekly-history` GET

13. **IImports** - Verify all endpoints (if implemented)
    - [ ] `/v1/organizations/{organization}/importers` GET
    - [ ] `/v1/organizations/{organization}/import` POST

#### 4.3 Common Issues to Identify

**Pagination Pattern Discrepancies**:
- [ ] Identify interfaces incorrectly using `page`/`perPage` when spec doesn't define them
- [ ] Verify correct use of `limit`/`offset` pattern (TimeEntries)
- [ ] Check for endpoints that return simple arrays vs paginated responses
- [ ] Document which endpoints support pagination and which don't

**Parameter Issues**:
- [ ] Verify all `[AliasAs]` attributes match OpenAPI parameter names (snake_case)
- [ ] Check for missing `[AliasAs]` on path parameters
- [ ] Verify query parameter names match exactly
- [ ] Confirm optional parameters use nullable types (`?`)
- [ ] Ensure required parameters are non-nullable

**Array/Collection Parameters**:
- [ ] Verify array parameters are defined correctly (e.g., `member_ids[]`, `project_ids[]`)
- [ ] Check Refit attribute usage for array parameters
- [ ] Test array parameter serialization

**Return Type Mismatches**:
- [ ] Verify correct use of `DataWrapper<T>` vs `PaginatedResponse<T>` vs plain types
- [ ] Check for endpoints returning plain arrays (e.g., Charts endpoints)
- [ ] Ensure proper `Task` vs `Task<T>` return types

#### 4.4 Fix All Identified Issues

For each discrepancy found:
- [ ] Update interface method signature to match OpenAPI spec exactly
- [ ] Add/update `[AliasAs]` attributes for parameter name mapping
- [ ] Correct return types (DataWrapper vs PaginatedResponse vs plain)
- [ ] Update XML documentation to note any API quirks or limitations
- [ ] Update corresponding test files to match new signatures
- [ ] Remove tests for unsupported features
- [ ] Add tests for newly discovered parameters

#### 4.5 Validation & Documentation

- [ ] Build solution - verify zero compilation errors
- [ ] Run all unit tests - verify all passing
- [ ] Run integration tests against live API
- [ ] Create `ENDPOINT_VERIFICATION.md` summary document with:
  - All interfaces verified
  - All discrepancies found and fixed
  - API limitations and quirks discovered
  - Recommendations for future development
- [ ] Update `MASTER_PLAN.md` with completion status
- [ ] Update interface XML documentation
- [ ] Update README.md if needed

#### 4.6 Known Findings (To Be Documented)

**Already Discovered**:
- ✅ **ITasks**: Incorrectly had `page`/`perPage` parameters - spec only supports `project_id` and `done` filters
- Tasks endpoint returns paginated response structure but doesn't accept pagination control parameters
- API uses default page size of 500 for tasks

**To Investigate**:
- ITimeEntries likely uses `limit`/`offset` instead of `page`/`perPage` (different pagination pattern)
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

### Phase 5: Advanced Features (PENDING Phase 4 Completion)

**NOTE**: This phase should NOT begin until Phase 4 (OpenAPI Verification) is complete to ensure all new features are implemented correctly from the start.

Ready to implement after Phase 4:
- **Reports API** - Create and manage reports, export functionality
- **Charts API** - Weekly project overview, weekly hours chart  
- **Imports API** - Toggl import functionality

These features will build on the verified foundation from Phase 4.

### Setup Phase Complete (December 3, 2025)
