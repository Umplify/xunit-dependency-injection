namespace Xunit.Microsoft.DependencyInjection.Logging;

public class OutputLogger(string categoryName, ITestOutputHelper testOutputHelper) : ILogger
{
	private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
	private readonly string _categoryName = categoryName;

	public OutputLogger(ITestOutputHelper testOutputHelper)
		: this("Tests", testOutputHelper)
	{
	}

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		=> new NoOpDisposable();

	public bool IsEnabled(LogLevel logLevel)
		=> true;

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
