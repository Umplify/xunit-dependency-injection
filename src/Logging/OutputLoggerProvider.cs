namespace Xunit.Microsoft.DependencyInjection.Logging;

public class OutputLoggerProvider(ITestOutputHelper testOutputHelper) : ILoggerProvider
{
	private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

	public ILogger CreateLogger(string categoryName)
		=> new OutputLogger(categoryName, _testOutputHelper);

	public void Dispose()
	{
	}
}
