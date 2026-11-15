namespace Solidtime.Api.Test;

/// <summary>
/// A logger that is always enabled and hands each formatted message to <see cref="Write"/>
/// </summary>
public abstract class TestLoggerBase : ILogger
{
	/// <inheritdoc/>
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

	/// <inheritdoc/>
	public bool IsEnabled(LogLevel logLevel) => true;

	/// <inheritdoc/>
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		=> Write(logLevel, formatter(state, exception), exception);

	/// <summary>
	/// Records one formatted message
	/// </summary>
	protected abstract void Write(LogLevel logLevel, string message, Exception? exception);
}
