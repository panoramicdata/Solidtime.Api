namespace Solidtime.Api.Models;

/// <summary>
/// Represents a user in the Solidtime system
/// </summary>
public class User : SolidtimeEntityBase
{
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
	/// The week start day (e.g., "monday", "sunday")
	/// </summary>
	[JsonPropertyName("week_start")]
	public required string WeekStart { get; set; }

	/// <summary>
	/// Gets or sets whether the user's email has been verified
	/// </summary>
	[JsonPropertyName("email_verified_at")]
	public DateTimeOffset? EmailVerifiedAt { get; set; }
}
