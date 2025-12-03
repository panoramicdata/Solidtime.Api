using System;
using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Represents an organization in the Solidtime system
/// </summary>
public class Organization
{
	/// <summary>
	/// The unique identifier for the organization
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	/// <summary>
	/// The organization's name
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The organization's billing email
	/// </summary>
	[JsonPropertyName("billing_email")]
	public string? BillingEmail { get; set; }

	/// <summary>
	/// The currency code used by the organization
	/// </summary>
	[JsonPropertyName("currency")]
	public required string Currency { get; set; }

	/// <summary>
	/// Gets or sets whether the organization is personal (single-user)
	/// </summary>
	[JsonPropertyName("is_personal")]
	public required bool IsPersonal { get; set; }

	/// <summary>
	/// The date and time when the organization was created
	/// </summary>
	[JsonPropertyName("created_at")]
	public required DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// The date and time when the organization was last updated
	/// </summary>
	[JsonPropertyName("updated_at")]
	public required DateTimeOffset UpdatedAt { get; set; }
}
