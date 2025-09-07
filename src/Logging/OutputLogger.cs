namespace Xunit.Microsoft.DependencyInjection.Logging;

/// <summary>
/// An <see cref="ILogger"/> implementation that writes log messages to the xUnit <see cref="ITestOutputHelper"/>.
/// </summary>
public class OutputLogger(string categoryName, ITestOutputHelper testOutputHelper) : ILogger
{
	private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
	private readonly string _categoryName = categoryName;

	/// <summary>
	/// Creates a logger with the default category name "Tests".
	/// </summary>
	public OutputLogger(ITestOutputHelper testOutputHelper)
		: this("Tests", testOutputHelper)
	{
	}

	/// <summary>
	/// Begins a logical operation scope. Returns a disposable that performs no action.
	/// </summary>
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		=> new NoOpDisposable();

	/// <summary>
	/// Always returns true; all log levels are enabled for forwarding to test output.
	/// </summary>
	public bool IsEnabled(LogLevel logLevel)
		=> true;

	/// <inheritdoc />
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
	{
		try
		{
			if (exception is not null)
			{
				_testOutputHelper.WriteLine($"{logLevel} - Category: {_categoryName} : {formatter(state, exception)} :: {DateTime.Now}");
			}
			else
			{
				_testOutputHelper.WriteLine($"{logLevel} - Category: {_categoryName} : {state} :: {DateTime.Now}");
			}
		}
		catch
		{
			//Ignore
		}
	}
}
