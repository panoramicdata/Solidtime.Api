using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Request object for updating an organization
/// </summary>
public class OrganizationUpdateRequest
{
	/// <summary>
	/// The organization's name
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	/// <summary>
	/// The organization's billing email
	/// </summary>
	[JsonPropertyName("billing_email")]
	public string? BillingEmail { get; set; }

	/// <summary>
	/// The currency code used by the organization
	/// </summary>
	[JsonPropertyName("currency")]
	public string? Currency { get; set; }
}
