namespace Xunit.Microsoft.DependencyInjection.Logging;

/// <summary>
/// An <see cref="ILogger"/> implementation that writes log messages to the xUnit <see cref="ITestOutputHelper"/>.
/// </summary>
/// <remarks>
/// Loggers created by <see cref="Abstracts.TestBedFixture"/> write to the output of the test that is
/// running when a message is logged (<see cref="TestContext.Current"/>), so a logger resolved from a
/// container built during an earlier test still reports to the current one. Loggers created through
/// the public constructors always write to the helper they were given.
/// </remarks>
public class OutputLogger : ILogger
{
	private readonly ITestOutputHelper _testOutputHelper;
	private readonly string _categoryName;
	private readonly bool _followCurrentTest;

	/// <summary>
	/// Creates a logger that writes to <paramref name="testOutputHelper"/> under the given category name.
	/// </summary>
	public OutputLogger(string categoryName, ITestOutputHelper testOutputHelper)
		: this(categoryName, testOutputHelper, followCurrentTest: false)
	{
	}

	/// <summary>
	/// When <paramref name="followCurrentTest"/> is true, messages go to the output of the test running
	/// when they are logged, and <paramref name="testOutputHelper"/> is used only when no test is running.
	/// </summary>
	internal OutputLogger(string categoryName, ITestOutputHelper testOutputHelper, bool followCurrentTest)
	{
		_categoryName = categoryName;
		_testOutputHelper = testOutputHelper;
		_followCurrentTest = followCurrentTest;
	}

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
		var testOutputHelper = _followCurrentTest
			? TestContext.Current.TestOutputHelper ?? _testOutputHelper
			: _testOutputHelper;
		try
		{
			if (exception is not null)
			{
				testOutputHelper.WriteLine($"{logLevel} - Category: {_categoryName} : {formatter(state, exception)} :: {DateTime.Now}");
			}
			else
			{
				testOutputHelper.WriteLine($"{logLevel} - Category: {_categoryName} : {state} :: {DateTime.Now}");
			}
		}
		catch
		{
			//Ignore
		}
	}
}
