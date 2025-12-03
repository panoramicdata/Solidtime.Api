namespace Solidtime.Api.Test;

/// <summary>
/// Logger implementation that writes to XUnit's ITestOutputHelper
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="XunitLogger"/> class
/// </remarks>
/// <param name="categoryName">The category name for the logger</param>
/// <param name="testOutputHelper">The test output helper</param>
public class XunitLogger(string categoryName, ITestOutputHelper testOutputHelper) : ILogger
{

	/// <inheritdoc/>
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		=> null;

	/// <inheritdoc/>
	public bool IsEnabled(LogLevel logLevel)
		=> true;

	/// <inheritdoc/>
	public void Log<TState>(
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}

		var message = formatter(state, exception);
		var logLevelString = logLevel switch
		{
			LogLevel.Trace => "TRACE",
			LogLevel.Debug => "DEBUG",
			LogLevel.Information => "INFO",
			LogLevel.Warning => "WARN",
			LogLevel.Error => "ERROR",
			LogLevel.Critical => "CRIT",
			_ => logLevel.ToString().ToUpperInvariant()
		};

		try
		{
			testOutputHelper.WriteLine($"[{logLevelString}] {categoryName}: {message}");

			if (exception != null)
			{
				testOutputHelper.WriteLine($"Exception: {exception}");
			}
		}
		catch (InvalidOperationException)
		{
			// XUnit throws this if writing to output after test completes
			// Ignore it to prevent test failures
		}
	}
}

/// <summary>
/// Logger provider that creates XunitLogger instances
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="XunitLoggerProvider"/> class
/// </remarks>
/// <param name="testOutputHelper">The test output helper</param>
public class XunitLoggerProvider(ITestOutputHelper testOutputHelper) : ILoggerProvider
{

	/// <inheritdoc/>
	public ILogger CreateLogger(string categoryName)
		=> new XunitLogger(categoryName, testOutputHelper);

	/// <inheritdoc/>
	public void Dispose() =>
		// Nothing to dispose
		GC.SuppressFinalize(this);
}
