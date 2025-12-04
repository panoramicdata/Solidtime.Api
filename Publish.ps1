#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Publishes the Solidtime.Api NuGet package to NuGet.org

.DESCRIPTION
    This script builds the project, runs unit tests (unless -SkipTests is specified),
    and publishes the NuGet package and symbols to NuGet.org.
    
    The NuGet API key must be stored in a file named 'nuget-key.txt' in the solution root.

.PARAMETER SkipTests
    Skip running unit tests before publishing

.PARAMETER Configuration
    Build configuration (Debug or Release). Default is Release.

.EXAMPLE
    .\Publish.ps1
    Runs tests and publishes the package

.EXAMPLE
    .\Publish.ps1 -SkipTests
    Publishes the package without running tests

.EXAMPLE
    .\Publish.ps1 -Configuration Debug
    Publishes a Debug build (not recommended for production)
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [switch]$SkipTests,

    [Parameter(Mandatory = $false)]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

# Script variables
$scriptPath = $PSScriptRoot
$nugetKeyFile = Join-Path $scriptPath 'nuget-key.txt'
$projectPath = Join-Path $scriptPath 'Solidtime.Api\Solidtime.Api.csproj'
$testProjectPath = Join-Path $scriptPath 'Solidtime.Api.Test\Solidtime.Api.Test.csproj'
$nugetSource = 'https://api.nuget.org/v3/index.json'

# Helper functions
function Write-Info {
    param([string]$Message)
    Write-Information "⏳  $Message" -InformationAction Continue
}

function Write-Success {
    param([string]$Message)
    Write-Information "✓ $Message" -InformationAction Continue
}

function Write-ErrorMessage {
    param([string]$Message)
    Write-Error "✗ $Message"
}

function Write-WarningMessage {
    param([string]$Message)
    Write-Warning "⚠️  $Message"
}

# Banner
Write-Information "" -InformationAction Continue
Write-Information "═════════════════════════════════════════════" -InformationAction Continue
Write-Information "█       Solidtime.Api NuGet Package Publisher           █" -InformationAction Continue
Write-Information "═════════════════════════════════════════════" -InformationAction Continue
Write-Information "" -InformationAction Continue

# Validate NuGet key file exists
Write-Info "Checking for NuGet API key..."
if (-not (Test-Path $nugetKeyFile)) {
    Write-ErrorMessage "NuGet API key file not found: $nugetKeyFile"
    Write-Warning ""
    Write-Warning "Please create a file named 'nuget-key.txt' in the solution root containing your NuGet API key."
    Write-Warning "You can get your API key from: https://www.nuget.org/account/apikeys"
    Write-Warning ""
    exit 1
}

# Read NuGet API key
$nugetApiKey = (Get-Content $nugetKeyFile -Raw).Trim()
if ([string]::IsNullOrWhiteSpace($nugetApiKey)) {
    Write-ErrorMessage "NuGet API key file is empty: $nugetKeyFile"
    exit 1
}
Write-Success "NuGet API key loaded"

# Validate project files exist
Write-Info "Validating project files..."
if (-not (Test-Path $projectPath)) {
    Write-ErrorMessage "Project file not found: $projectPath"
    exit 1
}
if (-not (Test-Path $testProjectPath)) {
    Write-ErrorMessage "Test project file not found: $testProjectPath"
    exit 1
}
Write-Success "Project files validated"

# Clean previous builds
Write-Info "Cleaning previous builds..."
try {
    dotnet clean $projectPath --configuration $Configuration --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMessage "Clean failed"
        exit 1
    }
    Write-Success "Clean completed"
}
catch {
    Write-ErrorMessage "Clean failed: $_"
    exit 1
}

# Restore dependencies
Write-Info "Restoring NuGet packages..."
try {
    dotnet restore $projectPath --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMessage "Restore failed"
        exit 1
    }
    Write-Success "Packages restored"
}
catch {
    Write-ErrorMessage "Restore failed: $_"
    exit 1
}

# Build the project
Write-Info "Building project ($Configuration configuration)..."
try {
    dotnet build $projectPath --configuration $Configuration --no-restore --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMessage "Build failed"
        exit 1
    }
    Write-Success "Build completed"
}
catch {
    Write-ErrorMessage "Build failed: $_"
    exit 1
}

# Run tests (unless skipped)
if (-not $SkipTests) {
    Write-Info "Running unit tests..."
    try {
        dotnet test $testProjectPath --configuration $Configuration --no-build --verbosity normal --logger "console;verbosity=normal"
        if ($LASTEXITCODE -ne 0) {
            Write-ErrorMessage "Tests failed. Publishing aborted."
            Write-Warning ""
            Write-Warning "Fix the failing tests or use -SkipTests to bypass test execution."
            exit 1
        }
        Write-Success "All tests passed"
    }
    catch {
        Write-ErrorMessage "Test execution failed: $_"
        exit 1
    }
}
else {
    Write-WarningMessage "Tests skipped (use without -SkipTests to run tests)"
}

# Pack the NuGet package
Write-Info "Packing NuGet package..."
try {
    dotnet pack $projectPath --configuration $Configuration --no-build --include-symbols --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMessage "Pack failed"
        exit 1
    }
    Write-Success "Package created"
}
catch {
    Write-ErrorMessage "Pack failed: $_"
    exit 1
}

# Find the generated package
$packagesPath = Join-Path $scriptPath "Solidtime.Api\bin\$Configuration"
$nupkgFiles = Get-ChildItem -Path $packagesPath -Filter "*.nupkg" -Recurse | Where-Object { $_.Name -notlike "*.symbols.nupkg" }
$snupkgFiles = Get-ChildItem -Path $packagesPath -Filter "*.snupkg" -Recurse

if ($nupkgFiles.Count -eq 0) {
    Write-ErrorMessage "No NuGet package found in $packagesPath"
    exit 1
}

# Get the latest package (in case multiple versions exist)
$packageFile = $nupkgFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1
$symbolsFile = $snupkgFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1

Write-Info "Package to publish: $($packageFile.Name)"
if ($symbolsFile) {
    Write-Info "Symbols package: $($symbolsFile.Name)"
}

# Confirm publication
Write-Information "" -InformationAction Continue
Write-Information "═══════════════════════════════════════" -InformationAction Continue
Write-Information "  Ready to publish to NuGet.org" -InformationAction Continue
Write-Information "═══════════════════════════════════════" -InformationAction Continue
Write-Information "  Package:       $($packageFile.Name)" -InformationAction Continue
Write-Information "  Configuration: $Configuration" -InformationAction Continue
Write-Information "  Tests:         $(if ($SkipTests) { 'Skipped' } else { 'Passed' })" -InformationAction Continue
Write-Information "═══════════════════════════════════════" -InformationAction Continue
Write-Information "" -InformationAction Continue

# Publish the main package
Write-Info "Publishing package to NuGet.org..."
try {
    dotnet nuget push $packageFile.FullName --api-key $nugetApiKey --source $nugetSource --skip-duplicate
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMessage "Package publish failed"
        exit 1
    }
    Write-Success "Package published successfully"
}
catch {
    Write-ErrorMessage "Package publish failed: $_"
    exit 1
}

# Publish symbols package if it exists
if ($symbolsFile) {
    Write-Info "Publishing symbols package to NuGet.org..."
    try {
        dotnet nuget push $symbolsFile.FullName --api-key $nugetApiKey --source $nugetSource --skip-duplicate
        if ($LASTEXITCODE -ne 0) {
            Write-WarningMessage "Symbols package publish failed (this is not critical)"
        }
        else {
            Write-Success "Symbols package published successfully"
        }
    }
    catch {
        Write-WarningMessage "Symbols package publish failed: $_"
    }
}

# Success!
Write-Information "" -InformationAction Continue
Write-Information "═════════════════════════════════════════════" -InformationAction Continue
Write-Information "█              ✨ Publication Successful! ✨            █" -InformationAction Continue
Write-Information "═════════════════════════════════════════════" -InformationAction Continue
Write-Information "" -InformationAction Continue
Write-Success "Package: $($packageFile.Name)"
Write-Success "NuGet.org: https://www.nuget.org/packages/Solidtime.Api/"
Write-Information "" -InformationAction Continue
Write-Info "Note: It may take a few minutes for the package to appear in NuGet search results."
Write-Information "" -InformationAction Continue
