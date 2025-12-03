# Solidtime.Api - Master Plan

## Progress Summary

**Last Updated**: December 3, 2025

**Current Phase**: Phase 3 Complete ✅ - Ready for Phase 4 (Advanced Features)

**Completed**:
- ✅ Phase 1: Project Setup (100%)
- ✅ Phase 2: Core Infrastructure (100%)
- ✅ Phase 3: Core API Implementation (100%)
- ✅ Build verification successful
- ✅ All projects compile without errors or warnings
- ✅ All 10 core API endpoints implemented with comprehensive tests

**Next Steps**: Test Phase 3 implementations against live Solidtime API, then begin Phase 4 (Advanced Features)

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

2. **Me** (needed for getting current user/org info) ✅
   - [x] IMe interface
   - [x] Models: User
   - [x] Tests: MeTests.cs

3. **Organizations** (context for other resources) ✅
   - [x] IOrganizations interface
   - [x] Models: Organization, OrganizationUpdateRequest
   - [x] Tests: OrganizationTests.cs

4. **Clients** ✅
   - [x] IClients interface
   - [x] Models: Client, ClientStoreRequest, ClientUpdateRequest
   - [x] Tests: ClientTests.cs

5. **Projects** ✅
   - [x] IProjects interface
   - [x] Models: Project, ProjectStoreRequest, ProjectUpdateRequest
   - [x] Tests: ProjectTests.cs

6. **Tags** ✅
   - [x] ITags interface
   - [x] Models: Tag, TagStoreRequest, TagUpdateRequest
   - [x] Tests: TagTests.cs

7. **Tasks** ✅
   - [x] ITasks interface
   - [x] Models: TaskModel, TaskStoreRequest, TaskUpdateRequest
   - [x] Tests: TaskTests.cs

8. **TimeEntries** ✅
   - [x] ITimeEntries interface
   - [x] Models: TimeEntry, TimeEntryStoreRequest, TimeEntryUpdateRequest
   - [x] Tests: TimeEntryTests.cs

9. **ProjectMembers** ✅
   - [x] IProjectMembers interface
   - [x] Models: ProjectMember, ProjectMemberStoreRequest, ProjectMemberUpdateRequest
   - [x] Tests: ProjectMemberTests.cs

10. **Members** (Organization members) ✅
    - [x] IMembers interface
    - [x] Models: Member, MemberUpdateRequest
    - [x] Tests: MemberTests.cs

### Phase 4: Advanced Features (Week 3-4)
- [ ] Reports API
  - [ ] IReports interface
  - [ ] Report models
  - [ ] Tests: ReportTests.cs
- [ ] Charts API
  - [ ] ICharts interface
  - [ ] Chart models
  - [ ] Tests: ChartTests.cs
- [ ] Imports API
  - [ ] IImports interface
  - [ ] Import models
  - [ ] Tests: ImportTests.cs

### Phase 5: Documentation & Polish (Week 4-5)
- [ ] XML documentation for all public APIs
- [ ] Update README.md with:
  - [ ] Installation instructions
  - [ ] Quick start guide
  - [ ] Code examples
  - [ ] API coverage matrix
  - [ ] Contributing guidelines
- [ ] Create CHANGELOG.md
- [ ] Add code samples in documentation
- [ ] Review and update SECURITY.md
- [ ] Create GitHub release notes template

### Phase 6: Testing & Quality (Week 5)
- [ ] Achieve >80% code coverage
- [ ] Integration tests with real API (via UserSecrets)
- [ ] Test all CRUD operations
- [ ] Test error handling scenarios
- [ ] Test pagination
- [ ] Performance testing
- [ ] Security review

### Phase 7: NuGet Package Preparation (Week 6)
- [ ] Validate package metadata
- [ ] Replace temporary logo with Solidtime logo
- [ ] Test package installation locally
- [ ] Create release notes
- [ ] Tag version 1.0.0
- [ ] Publish to NuGet.org

## Configuration & Secrets

### Test Configuration (UserSecrets)
```json
{
  "Configuration": {
    "ApiToken": "your-api-token-here",
    "SampleOrganizationId": "organization-uuid",
    "SampleProjectId": "project-uuid",
    "SampleClientId": "client-uuid",
    "CrudClientName": "Test Client for CRUD"
  }
}
```

### Environment Variables Support
- `SOLIDTIME_API_TOKEN` - API token for authentication
- `SOLIDTIME_BASE_URL` - Override base URL (default: https://app.solidtime.io/api)

## Testing Strategy

### Unit Tests
- Test each interface method
- Mock HTTP responses where appropriate
- Use AwesomeAssertions for fluent assertions

### Integration Tests
- Use real API with test organization
- Require UserSecrets configuration
- Clean up test data after runs
- Skip tests if credentials not configured

### Test Naming Convention
- `{Resource}_{Method}_{Scenario}` (e.g., `Projects_Get_Succeeds`)

## Documentation Standards

### XML Documentation
- All public classes, methods, and properties must have XML documentation
- Include parameter descriptions
- Include return value descriptions
- Include example code where helpful

### README.md Sections
1. Overview & Description
2. Installation
3. Quick Start
4. Authentication
5. Usage Examples (per resource type)
6. API Coverage
7. Error Handling
8. Contributing
9. License
10. Acknowledgments

## Code Style Guidelines

### Naming Conventions
- **Classes**: PascalCase
- **Interfaces**: IPascalCase
- **Methods**: PascalCase
- **Properties**: PascalCase
- **Parameters**: camelCase
- **Private fields**: _camelCase

### Pattern Preferences
- Use `required` keyword for required properties
- Use nullable reference types (`?`)
- Use `record` types for simple DTOs where appropriate
- Use primary constructors for dependency injection
- Prefer async/await over Task continuations
- Always pass CancellationToken

## Dependencies

### Main Project
```xml
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.0" />
<PackageReference Include="Nerdbank.GitVersioning" Version="3.9.50" />
<PackageReference Include="Refit" Version="8.0.0" />
```

### Test Project
```xml
<PackageReference Include="AwesomeAssertions" Version="9.3.0" />
<PackageReference Include="AwesomeAssertions.Analyzers" Version="9.0.8" />
<PackageReference Include="Microsoft.Extensions.Configuration.*" Version="10.0.0" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
<PackageReference Include="xunit.v3" Version="3.2.0" />
<PackageReference Include="Xunit.Microsoft.DependencyInjection" Version="10.0.0" />
<PackageReference Include="coverlet.collector" Version="6.0.2" />
```

## CI/CD Pipeline

### GitHub Actions Workflows
1. **CodeQL Analysis** - Security scanning on push/PR to develop branch
2. **Build & Test** - Run on all PRs and pushes
3. **NuGet Publish** - Publish on tagged releases

## Configuration Decisions ✅

1. **Organization Name**: Panoramic Data Limited ✅
   
2. **Repository Location**: https://github.com/panoramicdata/Solidtime.Api ✅
   
3. **License**: MIT License ✅

4. **API Testing**: Test Solidtime instance available ✅

5. **Logo**: Using Toggl logo temporarily (to be replaced later) ✅

6. **Versioning**: Starting at version 1.0.0 ✅

7. **API Coverage**: Implementing all endpoints from OpenAPI spec ✅

## Success Criteria

- [ ] Solution builds without warnings on .NET 10
- [ ] All tests pass
- [ ] Code coverage >80%
- [ ] NuGet package can be installed and used
- [ ] README provides clear usage examples
- [ ] API parity with Toggl.Api in terms of quality and patterns
- [ ] Security analysis passes (CodeQL)
- [ ] Published to NuGet.org

## References

- **Solidtime API Docs**: https://docs.solidtime.io/api-reference
- **OpenAPI Spec**: `solidtime-openapi.json` (in project root)
- **Toggl.Api Reference**: `..\Toggl.Api\` (sibling project)
- **Refit Documentation**: https://github.com/reactiveui/refit
- **XUnit Documentation**: https://xunit.net/
- **AwesomeAssertions**: https://github.com/csMACnz/AwesomeAssertions

## Timeline

- **Week 1**: Project setup + Core infrastructure
- **Week 2-3**: Core API implementation (ApiTokens through Members)
- **Week 3-4**: Advanced features (Reports, Charts, Imports)
- **Week 4-5**: Documentation & polish
- **Week 5**: Testing & quality assurance
- **Week 6**: NuGet package preparation & release

**Total Estimated Time**: 6 weeks for complete implementation

## Implementation Notes

### Phase 3 Complete (December 3, 2025)
- ✅ All 10 core API endpoints implemented with full CRUD operations
- ✅ Comprehensive XML documentation on all public APIs
- ✅ Test coverage for all endpoints with multiple test scenarios
- ✅ Snake_case JSON property naming implemented throughout
- ✅ Pagination support implemented where applicable
- ✅ All models follow required property pattern with nullable reference types
- ✅ Build successful with zero errors and zero warnings
- ✅ Code analysis level set to latest-recommended passing

### Implemented Endpoints Summary
1. **ApiTokens** - List, Create, Revoke
2. **Me** - Get current user info
3. **Organizations** - Get, Update
4. **Clients** - Get, GetById, Create, Update, Delete (with pagination)
5. **Projects** - Get, GetById, Create, Update, Delete (with pagination)
6. **Tags** - Get, GetById, Create, Update, Delete (with pagination)
7. **Tasks** - Get, GetById, Create, Update, Delete (with pagination)
8. **TimeEntries** - Get, GetById, Create, Update, Delete (with pagination, running timer support)
9. **ProjectMembers** - Get, Create, Update, Delete
10. **Members** - Get, GetById, Update, Delete (with pagination)

### Setup Phase Complete (December 3, 2025)
