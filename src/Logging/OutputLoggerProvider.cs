namespace Xunit.Microsoft.DependencyInjection.Logging;

/// <summary>
/// Logger provider that creates <see cref="OutputLogger"/> instances writing to a shared
/// <see cref="ITestOutputHelper"/>.
/// </summary>
public class OutputLoggerProvider(ITestOutputHelper testOutputHelper) : ILoggerProvider
{
	private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

	public ILogger CreateLogger(string categoryName)
		=> new OutputLogger(categoryName, _testOutputHelper);

	public void Dispose() => GC.SuppressFinalize(this);
}
