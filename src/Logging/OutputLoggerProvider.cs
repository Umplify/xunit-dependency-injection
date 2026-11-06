namespace Xunit.Microsoft.DependencyInjection.Logging;

/// <summary>
/// Logger provider that creates <see cref="OutputLogger"/> instances writing to a shared
/// <see cref="ITestOutputHelper"/>.
/// </summary>
public class OutputLoggerProvider : ILoggerProvider
{
	private readonly ITestOutputHelper _testOutputHelper;
	private readonly bool _followCurrentTest;

	/// <summary>
	/// Creates a provider whose loggers write to <paramref name="testOutputHelper"/>.
	/// </summary>
	public OutputLoggerProvider(ITestOutputHelper testOutputHelper)
		: this(testOutputHelper, followCurrentTest: false)
	{
	}

	/// <summary>
	/// Used by <see cref="Abstracts.TestBedFixture"/>: when <paramref name="followCurrentTest"/> is true, loggers
	/// write to the output of the test running when they log, falling back to <paramref name="testOutputHelper"/>.
	/// </summary>
	internal OutputLoggerProvider(ITestOutputHelper testOutputHelper, bool followCurrentTest)
	{
		_testOutputHelper = testOutputHelper;
		_followCurrentTest = followCurrentTest;
	}

	/// <summary>
	/// Creates a new <see cref="OutputLogger"/> for the specified category.
	/// </summary>
	public ILogger CreateLogger(string categoryName)
		=> new OutputLogger(categoryName, _testOutputHelper, _followCurrentTest);

	/// <summary>
	/// Disposes the provider (no-op other than suppressing finalization).
	/// </summary>
	public void Dispose() => GC.SuppressFinalize(this);
}
