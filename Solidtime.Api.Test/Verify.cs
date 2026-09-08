namespace Solidtime.Api.Test;

/// <summary>
/// Assertions shared by the endpoint test classes.
/// These live here rather than being repeated per test class so that a change to what
/// "a well-formed response" means is made in one place.
/// </summary>
internal static class Verify
{
	/// <summary>
	/// Asserts that a paginated envelope carries data.
	/// Links are deliberately not asserted: the API omits them when the result set is empty.
	/// </summary>
	/// <typeparam name="T">The type of the items in the envelope</typeparam>
	/// <param name="result">The paginated response to check</param>
	internal static void PaginatedEnvelope<T>(PaginatedResponse<T> result)
	{
		result.Should().NotBeNull();
		result.Data.Should().NotBeNull();
	}

	/// <summary>
	/// Asserts that a paginated envelope carries data and pagination metadata.
	/// Only for the endpoints that populate meta; several endpoints leave it out entirely.
	/// </summary>
	/// <typeparam name="T">The type of the items in the envelope</typeparam>
	/// <param name="result">The paginated response to check</param>
	internal static void PaginatedEnvelopeWithMeta<T>(PaginatedResponse<T> result)
	{
		PaginatedEnvelope(result);
		result.Meta.Should().NotBeNull();
	}

	/// <summary>
	/// Asserts that an entity's timestamps are present, in the past, and correctly ordered.
	/// </summary>
	/// <param name="entity">The entity to check</param>
	internal static void Timestamps(SolidtimeEntityBase entity)
	{
		entity.CreatedAt.Should().NotBeNull();
		entity.UpdatedAt.Should().NotBeNull();
		AssertTimestampsInPastAndOrdered(entity);
	}

	/// <summary>
	/// Asserts that whichever timestamps the endpoint returned are in the past and correctly
	/// ordered, without requiring either to be present. Some endpoints - /v1/users/me and
	/// organizations among them - do not return timestamps at all.
	/// </summary>
	/// <param name="entity">The entity to check</param>
	internal static void OptionalTimestamps(SolidtimeEntityBase entity)
		=> AssertTimestampsInPastAndOrdered(entity);

	/// <summary>
	/// Asserts that every chart data point carries a date and a non-negative duration.
	/// </summary>
	/// <typeparam name="T">The chart data point type</typeparam>
	/// <param name="dataPoints">The data points to check</param>
	/// <param name="getDate">Reads the date from a data point</param>
	/// <param name="getDuration">Reads the duration, in seconds, from a data point</param>
	internal static void DateDurationPoints<T>(
		List<T> dataPoints,
		Func<T, string> getDate,
		Func<T, int> getDuration)
	{
		dataPoints.Should().NotBeNull();

		foreach (var dataPoint in dataPoints)
		{
			getDate(dataPoint).Should().NotBeNullOrWhiteSpace();
			getDuration(dataPoint).Should().BeGreaterThanOrEqualTo(0);
		}
	}

	private static void AssertTimestampsInPastAndOrdered(SolidtimeEntityBase entity)
	{
		if (entity.CreatedAt.HasValue)
		{
			entity.CreatedAt.Value.Should().BeBefore(DateTimeOffset.UtcNow);
		}

		if (entity.UpdatedAt.HasValue)
		{
			entity.UpdatedAt.Value.Should().BeBefore(DateTimeOffset.UtcNow);
		}

		if (entity.CreatedAt.HasValue && entity.UpdatedAt.HasValue)
		{
			entity.UpdatedAt.Value.Should().BeOnOrAfter(entity.CreatedAt.Value);
		}
	}
}
