using System.Net;

namespace Solidtime.Api.Exceptions;

/// <summary>
/// Exception thrown when the Solidtime API returns an error
/// </summary>
public class SolidtimeApiException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SolidtimeApiException"/> class
	/// </summary>
	public SolidtimeApiException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SolidtimeApiException"/> class with a specified error message
	/// </summary>
	/// <param name="message">The error message</param>
	public SolidtimeApiException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SolidtimeApiException"/> class with a specified error message and inner exception
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="innerException">The inner exception</param>
	public SolidtimeApiException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SolidtimeApiException"/> class with a specified error message and status code
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="statusCode">The HTTP status code</param>
	public SolidtimeApiException(string message, HttpStatusCode statusCode)
		: base(message)
	{
		StatusCode = statusCode;
	}

	/// <summary>
	/// Gets the HTTP status code associated with the error
	/// </summary>
	public HttpStatusCode? StatusCode { get; }
}
