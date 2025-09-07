namespace Xunit.Microsoft.DependencyInjection.Logging;

/// <summary>
/// Lightweight disposable that performs no action. Used to satisfy API contracts requiring a disposable scope.
/// </summary>
internal class NoOpDisposable : IDisposable
{
	/// <inheritdoc />
	public void Dispose() { }
}
