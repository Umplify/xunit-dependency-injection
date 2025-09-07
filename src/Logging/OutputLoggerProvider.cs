namespace Xunit.Microsoft.DependencyInjection.Logging;

/// <summary>
/// Logger provider that creates <see cref="OutputLogger"/> instances writing to a shared
/// <see cref="ITestOutputHelper"/>.
/// </summary>
public class OutputLoggerProvider(ITestOutputHelper testOutputHelper) : ILoggerProvider
{
	private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

	/// <summary>
	/// Creates a new <see cref="OutputLogger"/> for the specified category.
	/// </summary>
	public ILogger CreateLogger(string categoryName)
		=> new OutputLogger(categoryName, _testOutputHelper);

	/// <summary>
	/// Disposes the provider (no-op other than suppressing finalization).
	/// </summary>
	public void Dispose() => GC.SuppressFinalize(this);
}
