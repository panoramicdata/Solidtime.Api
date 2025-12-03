using System.Text.Json.Serialization;

namespace Solidtime.Api.Models;

/// <summary>
/// Request object for updating a project member
/// </summary>
public class ProjectMemberUpdateRequest
{
	/// <summary>
	/// Gets or sets the billable rate for this project member (in cents)
	/// </summary>
	[JsonPropertyName("billable_rate")]
	public int? BillableRate { get; set; }
}
