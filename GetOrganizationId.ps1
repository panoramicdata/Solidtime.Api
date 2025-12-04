#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Finds your Solidtime Organization ID using your API token.

.DESCRIPTION
    This script uses your Solidtime API token to query the API and help you
    find your Organization ID needed for running tests.

.EXAMPLE
    .\GetOrganizationId.ps1

.NOTES
    This script uses Write-Output and Write-Warning for key information
    to support output redirection. Colored decorative output uses Write-Host.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$false)]
    [string]$ApiToken
)

# Helper function to write colored output (decorative only)
function Write-ColoredOutput {
    param(
        [string]$Message,
        [System.ConsoleColor]$Color = 'White'
    )
    Write-Host $Message -ForegroundColor $Color
}

# Get API token from user secrets if not provided
if ([string]::IsNullOrWhiteSpace($ApiToken)) {
    try {
        Push-Location "Solidtime.Api.Test"
        $secretsList = dotnet user-secrets list 2>$null
        Pop-Location
        
        $tokenLine = $secretsList | Select-String "Configuration:ApiToken"
        if ($tokenLine) {
            $ApiToken = ($tokenLine -replace '.*= ', '').Trim()
            Write-ColoredOutput "? Found API token in user secrets" -Color Green
        }
    } catch {
        Pop-Location
    }
}

# Prompt for API token if still not found
if ([string]::IsNullOrWhiteSpace($ApiToken)) {
    $ApiToken = Read-Host "Enter your Solidtime API token"
}

if ([string]::IsNullOrWhiteSpace($ApiToken)) {
    Write-Error "API token is required"
    exit 1
}

$baseUrl = "https://app.solidtime.io/api"
$headers = @{
    "Authorization" = "Bearer $ApiToken"
    "Accept" = "application/json"
}

Write-ColoredOutput "`n?? Fetching user information...`n" -Color Cyan

try {
    # Get current user
    $meResponse = Invoke-RestMethod -Uri "$baseUrl/v1/users/me" -Headers $headers -Method Get
    
    Write-ColoredOutput "? Successfully authenticated!" -Color Green
    Write-Output ""
    Write-Output "User Information:"
    Write-Output "  Name: $($meResponse.data.name)"
    Write-Output "  Email: $($meResponse.data.email)"
    Write-Output "  User ID: $($meResponse.data.id)"
    
    Write-ColoredOutput "`n?? Attempting to discover your organization ID...`n" -Color Cyan
    
    # Strategy: Try to infer organization from API token endpoint
    # API tokens are created within an organization context
    try {
        $tokensResponse = Invoke-RestMethod -Uri "$baseUrl/v1/users/me/api-tokens" -Headers $headers -Method Get
        
        # Check if there's any organization hint in the response
        if ($tokensResponse.data -and $tokensResponse.data.Count -gt 0) {
            Write-ColoredOutput "?? Found $($tokensResponse.data.Count) API token(s)" -Color Cyan
        }
    } catch {
        Write-Warning "Could not fetch API tokens"
    }
    
    Write-ColoredOutput "`n?? Organization ID Discovery Methods:`n" -Color Cyan
    
    Write-ColoredOutput "METHOD 1: Via Solidtime Web UI (Recommended)" -Color Green
    Write-Output "  1. Go to https://app.solidtime.io"
    Write-Output "  2. Log in with your account"
    Write-Output "  3. Look at the URL in your browser's address bar"
    Write-Output "  4. The URL format is: /teams/{YOUR-ORG-ID}/..."
    Write-Output "  5. Copy the UUID (format: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)"
    Write-Output "  6. Note: The UI uses 'teams' in URLs, but the API uses 'organizations'"
    Write-Output ""
    
    Write-ColoredOutput "EXAMPLE:" -Color Yellow
    Write-Output "  If your URL is: https://app.solidtime.io/teams/8ea33e20-bceb-4b6b-b5bb-49fadc00677a"
    Write-Output "  Your Organization ID is: 8ea33e20-bceb-4b6b-b5bb-49fadc00677a"
    Write-Output ""
    
    Write-ColoredOutput "METHOD 2: Via Browser Developer Tools (Advanced)" -Color Green
    Write-Output "  1. Open https://app.solidtime.io in your browser"
    Write-Output "  2. Press F12 to open Developer Tools"
    Write-Output "  3. Go to the 'Network' tab"
    Write-Output "  4. Click on 'Projects', 'Time Entries', or any other menu item"
    Write-Output "  5. Look at the API request URLs in the Network tab"
    Write-Output "  6. You'll see URLs like: /v1/organizations/{YOUR-ORG-ID}/projects"
    Write-Output "  7. Copy the organization ID from the URL"
    Write-Output ""
    
    Write-ColoredOutput "METHOD 3: Check Your Browser's Local Storage (Advanced)" -Color Green
    Write-Output "  1. Open https://app.solidtime.io in your browser"
    Write-Output "  2. Press F12 to open Developer Tools"
    Write-Output "  3. Go to 'Application' tab (Chrome) or 'Storage' tab (Firefox)"
    Write-Output "  4. Look under 'Local Storage' ? 'https://app.solidtime.io'"
    Write-Output "  5. Look for keys related to 'organization' or 'current_organization'"
    Write-Output ""
    
    Write-ColoredOutput "NEXT STEPS:" -Color Cyan
    Write-Output "Once you have your Organization ID, configure it with:"
    Write-Output ""
    Write-Output "  cd Solidtime.Api.Test"
    Write-Output "  dotnet user-secrets set `"Configuration:SampleOrganizationId`" `"YOUR-ORG-ID`" ""
    Write-Output ""
    
    Write-ColoredOutput "Then run the tests:" -Color Cyan
    Write-Output "  dotnet test"
    Write-Output ""
    
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    
    if ($statusCode -eq 401) {
        Write-Error "Authentication failed! The API token appears to be invalid or expired."
        Write-Output "Please create a new token at: https://app.solidtime.io/settings/api-tokens"
    } else {
        Write-Error $_.Exception.Message
        if ($_.ErrorDetails.Message) {
            Write-Warning "Details: $($_.ErrorDetails.Message)"
        }
    }
    exit 1
}
