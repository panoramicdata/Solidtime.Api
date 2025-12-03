#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Finds your Solidtime Organization ID using your API token.

.DESCRIPTION
    This script uses your Solidtime API token to query the API and help you
    find your Organization ID needed for running tests.

.EXAMPLE
    .\GetOrganizationId.ps1
#>

param(
    [Parameter(Mandatory=$false)]
    [string]$ApiToken
)

# Get API token from user secrets if not provided
if ([string]::IsNullOrWhiteSpace($ApiToken)) {
    try {
        Push-Location "Solidtime.Api.Test"
        $secretsList = dotnet user-secrets list 2>$null
        Pop-Location
        
        $tokenLine = $secretsList | Select-String "Configuration:ApiToken"
        if ($tokenLine) {
            $ApiToken = ($tokenLine -replace '.*= ', '').Trim()
            Write-Host "? Found API token in user secrets" -ForegroundColor Green
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
    Write-Host "? Error: API token is required" -ForegroundColor Red
    exit 1
}

$baseUrl = "https://app.solidtime.io/api"
$headers = @{
    "Authorization" = "Bearer $ApiToken"
    "Accept" = "application/json"
}

Write-Host "`n?? Fetching user information...`n" -ForegroundColor Cyan

try {
    # Get current user
    $meResponse = Invoke-RestMethod -Uri "$baseUrl/v1/users/me" -Headers $headers -Method Get
    
    Write-Host "? Successfully authenticated!" -ForegroundColor Green
    Write-Host "`nUser Information:" -ForegroundColor Yellow
    Write-Host "  Name: $($meResponse.data.name)"
    Write-Host "  Email: $($meResponse.data.email)"
    Write-Host "  User ID: $($meResponse.data.id)"
    
    Write-Host "`n?? Attempting to discover your organization ID...`n" -ForegroundColor Cyan
    
    # Strategy: Try to infer organization from API token endpoint
    # API tokens are created within an organization context
    try {
        $tokensResponse = Invoke-RestMethod -Uri "$baseUrl/v1/users/me/api-tokens" -Headers $headers -Method Get
        
        # Check if there's any organization hint in the response
        if ($tokensResponse.data -and $tokensResponse.data.Count -gt 0) {
            Write-Host "?? Found $($tokensResponse.data.Count) API token(s)" -ForegroundColor Cyan
        }
    } catch {
        Write-Host "??  Could not fetch API tokens" -ForegroundColor Yellow
    }
    
    Write-Host "`n?? Organization ID Discovery Methods:`n" -ForegroundColor Cyan
    
    Write-Host "METHOD 1: Via Solidtime Web UI (Recommended)" -ForegroundColor Green
    Write-Host "  1. Go to https://app.solidtime.io"
    Write-Host "  2. Log in with your account"
    Write-Host "  3. Look at the URL in your browser's address bar"
    Write-Host "  4. The URL format is: /teams/{YOUR-ORG-ID}/..."
    Write-Host "  5. Copy the UUID (format: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)"
    Write-Host "  6. Note: The UI uses 'teams' in URLs, but the API uses 'organizations'`n"
    
    Write-Host "EXAMPLE:" -ForegroundColor Yellow
    Write-Host "  If your URL is: https://app.solidtime.io/teams/8ea33e20-bceb-4b6b-b5bb-49fadc00677a"
    Write-Host "  Your Organization ID is: 8ea33e20-bceb-4b6b-b5bb-49fadc00677a`n"
    
    Write-Host "METHOD 2: Via Browser Developer Tools (Advanced)" -ForegroundColor Green
    Write-Host "  1. Open https://app.solidtime.io in your browser"
    Write-Host "  2. Press F12 to open Developer Tools"
    Write-Host "  3. Go to the 'Network' tab"
    Write-Host "  4. Click on 'Projects', 'Time Entries', or any other menu item"
    Write-Host "  5. Look at the API request URLs in the Network tab"
    Write-Host "  6. You'll see URLs like: /v1/organizations/{YOUR-ORG-ID}/projects"
    Write-Host "  7. Copy the organization ID from the URL`n"
    
    Write-Host "METHOD 3: Check Your Browser's Local Storage (Advanced)" -ForegroundColor Green
    Write-Host "  1. Open https://app.solidtime.io in your browser"
    Write-Host "  2. Press F12 to open Developer Tools"
    Write-Host "  3. Go to 'Application' tab (Chrome) or 'Storage' tab (Firefox)"
    Write-Host "  4. Look under 'Local Storage' ? 'https://app.solidtime.io'"
    Write-Host "  5. Look for keys related to 'organization' or 'current_organization'`n"
    
    Write-Host "NEXT STEPS:" -ForegroundColor Cyan
    Write-Host "Once you have your Organization ID, configure it with:`n"
    Write-Host "  cd Solidtime.Api.Test" -ForegroundColor White
    Write-Host "  dotnet user-secrets set `"Configuration:SampleOrganizationId`" `"YOUR-ORG-ID`"`n" -ForegroundColor White
    
    Write-Host "Then run the tests:" -ForegroundColor Cyan
    Write-Host "  dotnet test`n" -ForegroundColor White
    
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    
    if ($statusCode -eq 401) {
        Write-Host "? Authentication failed!" -ForegroundColor Red
        Write-Host "`nThe API token appears to be invalid or expired." -ForegroundColor Yellow
        Write-Host "Please create a new token at: https://app.solidtime.io/settings/api-tokens`n"
    } else {
        Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ErrorDetails.Message) {
            Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
        }
    }
    exit 1
}
