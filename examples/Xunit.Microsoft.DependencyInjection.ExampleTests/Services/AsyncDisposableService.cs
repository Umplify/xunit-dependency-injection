namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

/// <summary>
/// A service that needs asynchronous disposal logic.
/// </summary>
public class AsyncDisposableService : IDisposable, IAsyncDisposable
{
  private bool _disposedAsync;

  public ValueTask DisposeAsync()
  {
    _disposedAsync = true;

    return ValueTask.CompletedTask;
  }

  public void Dispose()
  {
    if (!_disposedAsync)
    {
      throw new Exception("The AsyncDisposable service has NOT been disposed asynchronously.");
    }
  }
}