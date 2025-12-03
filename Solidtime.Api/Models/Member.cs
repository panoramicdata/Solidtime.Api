using System;
using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Represents an organization member in the Solidtime system
/// </summary>
public class Member
{
	/// <summary>
	/// Gets or sets the unique identifier for the member
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	/// <summary>
	/// Gets or sets the user ID of the member
	/// </summary>
	[JsonPropertyName("user_id")]
	public required string UserId { get; set; }

	/// <summary>
	/// Gets or sets the organization ID
	/// </summary>
	[JsonPropertyName("organization_id")]
	public required string OrganizationId { get; set; }

	/// <summary>
	/// Gets or sets the member's role in the organization
	/// </summary>
	[JsonPropertyName("role")]
	public required string Role { get; set; }

	/// <summary>
	/// Gets or sets the billable rate for this member (in cents)
	/// </summary>
	[JsonPropertyName("billable_rate")]
	public int? BillableRate { get; set; }

	/// <summary>
	/// Gets or sets the date and time when the member was created
	/// </summary>
	[JsonPropertyName("created_at")]
	public required DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the date and time when the member was last updated
	/// </summary>
	[JsonPropertyName("updated_at")]
	public required DateTimeOffset UpdatedAt { get; set; }
}
