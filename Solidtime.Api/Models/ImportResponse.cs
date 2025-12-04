namespace Solidtime.Api.Models;

/// <summary>
/// Response from an import operation
/// </summary>
public class ImportResponse
{
	/// <summary>
	/// The import report with counts of created entities
	/// </summary>
	[JsonPropertyName("report")]
	public required ImportReport Report { get; set; }
}

/// <summary>
/// Report detailing what was imported
/// </summary>
public class ImportReport
{
	/// <summary>
	/// Client import statistics
	/// </summary>
	[JsonPropertyName("clients")]
	public required ImportEntityCount Clients { get; set; }

	/// <summary>
	/// Project import statistics
	/// </summary>
	[JsonPropertyName("projects")]
	public required ImportEntityCount Projects { get; set; }

	/// <summary>
	/// Task import statistics
	/// </summary>
	[JsonPropertyName("tasks")]
	public required ImportEntityCount Tasks { get; set; }

	/// <summary>
	/// Time entry import statistics
	/// </summary>
	[JsonPropertyName("time_entries")]
	public required ImportEntityCount TimeEntries { get; set; }

	/// <summary>
	/// Tag import statistics
	/// </summary>
	[JsonPropertyName("tags")]
	public required ImportEntityCount Tags { get; set; }

	/// <summary>
	/// User import statistics
	/// </summary>
	[JsonPropertyName("users")]
	public required ImportEntityCount Users { get; set; }
}

/// <summary>
/// Count of entities created during import
/// </summary>
public class ImportEntityCount
{
	/// <summary>
	/// Number of entities created
	/// </summary>
	[JsonPropertyName("created")]
	public required int Created { get; set; }
}
