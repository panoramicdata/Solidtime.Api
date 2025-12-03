using System;
using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Represents a user in the Solidtime system
/// </summary>
public class User
{
	/// <summary>
	/// The unique identifier for the user
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	/// <summary>
	/// The user's name
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The user's email address
	/// </summary>
	[JsonPropertyName("email")]
	public required string Email { get; set; }

	/// <summary>
	/// The URL to the user's profile photo
	/// </summary>
	[JsonPropertyName("profile_photo_url")]
	public string? ProfilePhotoUrl { get; set; }

	/// <summary>
	/// The user's timezone
	/// </summary>
	[JsonPropertyName("timezone")]
	public required string Timezone { get; set; }

	/// <summary>
	/// The week start day (0=Sunday, 1=Monday, etc.)
	/// </summary>
	[JsonPropertyName("week_start")]
	public required int WeekStart { get; set; }

	/// <summary>
	/// Gets or sets whether the user's email has been verified
	/// </summary>
	[JsonPropertyName("email_verified_at")]
	public DateTimeOffset? EmailVerifiedAt { get; set; }

	/// <summary>
	/// The date and time when the user was created
	/// </summary>
	[JsonPropertyName("created_at")]
	public required DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// The date and time when the user was last updated
	/// </summary>
	[JsonPropertyName("updated_at")]
	public required DateTimeOffset UpdatedAt { get; set; }
}
