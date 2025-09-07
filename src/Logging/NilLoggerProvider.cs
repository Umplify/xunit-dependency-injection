namespace Xunit.Microsoft.DependencyInjection.Logging;

/// <summary>
/// Logger provider that discards all log messages (Null Object pattern).
/// Useful for suppressing output when no <see cref="ITestOutputHelper"/> is available.
/// </summary>
public sealed class NilLoggerProvider : ILoggerProvider
{
	/// <inheritdoc />
	public ILogger CreateLogger(string categoryName)
		=> new NilLogger();

	/// <inheritdoc />
	public void Dispose()
	{
	}

	private class NilLogger : ILogger
	{
		public IDisposable? BeginScope<TState>(TState state) where TState : notnull
			=> new NoOpDisposable();

		public bool IsEnabled(LogLevel logLevel)
			=> false;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
		}
	}
}
