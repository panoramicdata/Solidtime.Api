#!/usr/bin/env dotnet-script
#r "nuget: Refit, 8.0.0"
#r "nuget: System.Text.Json, 10.0.0"

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

// Prompt for API token
Console.Write("Enter your Solidtime API token: ");
var apiToken = Console.ReadLine();

if (string.IsNullOrWhiteSpace(apiToken))
{
    Console.WriteLine("Error: API token is required");
    return;
}

// Create HTTP client
using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://app.solidtime.io/api");
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

try
{
    Console.WriteLine("\nFetching user information...\n");
    
    // Get current user
    var meResponse = await httpClient.GetAsync("/v1/users/me");
    meResponse.EnsureSuccessStatusCode();
    var meJson = await meResponse.Content.ReadAsStringAsync();
    
    Console.WriteLine("? Successfully authenticated!");
    Console.WriteLine($"\nUser Info Response:\n{meJson}\n");
    
    // Parse user ID
    using var meDoc = JsonDocument.Parse(meJson);
    var userId = meDoc.RootElement.GetProperty("data").GetProperty("id").GetString();
    var userName = meDoc.RootElement.GetProperty("data").GetProperty("name").GetString();
    
    Console.WriteLine($"User: {userName}");
    Console.WriteLine($"User ID: {userId}\n");
    
    // Try to get API tokens to find organization
    Console.WriteLine("Fetching API tokens to find organization ID...\n");
    var tokensResponse = await httpClient.GetAsync("/v1/users/me/api-tokens");
    tokensResponse.EnsureSuccessStatusCode();
    var tokensJson = await tokensResponse.Content.ReadAsStringAsync();
    
    Console.WriteLine($"API Tokens Response:\n{tokensJson}\n");
    
    // The Solidtime API doesn't directly expose organizations from /me
    // We need to try accessing an organization endpoint or checking members
    Console.WriteLine("?? Note: The /me endpoint doesn't directly return organization information.");
    Console.WriteLine("\nTo find your Organization ID:");
    Console.WriteLine("1. Log into Solidtime at https://app.solidtime.io");
    Console.WriteLine("2. Navigate to Settings");
    Console.WriteLine("3. Look at the URL - it will be: /organizations/{YOUR-ORG-ID}/settings");
    Console.WriteLine("4. Copy the UUID from the URL");
    
    Console.WriteLine("\nAlternatively, check your browser's developer tools (Network tab)");
    Console.WriteLine("when logged into Solidtime - API calls will include the organization ID.\n");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"? Error: {ex.Message}");
    if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
        Console.WriteLine("\nThe API token appears to be invalid or expired.");
        Console.WriteLine("Please create a new token at: https://app.solidtime.io/settings/api-tokens");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"? Unexpected error: {ex.Message}");
}
