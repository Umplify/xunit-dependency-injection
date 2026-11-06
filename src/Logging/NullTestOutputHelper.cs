namespace Xunit.Microsoft.DependencyInjection.Logging;

/// <summary>
/// An <see cref="ITestOutputHelper"/> that discards everything. Used as the fallback for loggers
/// created when no test is running, so that later messages are routed to the current test.
/// </summary>
internal sealed class NullTestOutputHelper : ITestOutputHelper
{
	public static readonly NullTestOutputHelper Instance = new();

	private NullTestOutputHelper() { }

	public string Output => string.Empty;

	public void Write(string message) { }

	public void Write(string format, params object[] args) { }

	public void WriteLine(string message) { }

	public void WriteLine(string format, params object[] args) { }
}
