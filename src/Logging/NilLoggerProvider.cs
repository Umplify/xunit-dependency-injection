namespace Xunit.Microsoft.DependencyInjection.Logging;

public sealed class NilLoggerProvider : ILoggerProvider
{
	public ILogger CreateLogger(string categoryName)
		=> new NilLogger();

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
