namespace Solidtime.Api.Models;

/// <summary>
/// Represents an organization in the Solidtime system
/// </summary>
public class Organization : SolidtimeEntityBase
{
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
}
