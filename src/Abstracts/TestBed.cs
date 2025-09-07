namespace Xunit.Microsoft.DependencyInjection.Abstracts;

/// <summary>
/// Base class for test classes which use a fixture of type <typeparamref name="TFixture"/>.
/// Provides synchronous and asynchronous disposal semantics and a hook (<see cref="Clear"/>)
/// that derived classes can override to release managed resources created during tests.
/// </summary>
/// <typeparam name="TFixture">The fixture type shared across all tests in the class.</typeparam>
public class TestBed<TFixture>(ITestOutputHelper testOutputHelper, TFixture fixture) : IDisposable, IClassFixture<TFixture>, IAsyncDisposable
	where TFixture : class
{
	/// <summary>
	/// The <see cref="ITestOutputHelper"/> used to write diagnostic output during test execution.
	/// </summary>
	protected readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

	/// <summary>
	/// The fixture instance supplied by xUnit for this test class.
	/// </summary>
	protected readonly TFixture _fixture = fixture;

	private bool _disposedValue;
	private bool _disposedAsync;

	/// <summary>
	/// Releases managed resources. Override to add custom cleanup logic. Unmanaged resources
	/// should be released only if added by derived classes.
	/// </summary>
	/// <param name="disposing">True when called from <see cref="Dispose()"/>; false when from a finalizer.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				// Dispose managed state
				Clear();
			}
			_disposedValue = true;
		}
	}

	// Finalizer intentionally omitted. Add only if unmanaged resources are introduced.

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Asynchronously disposes resources. Calls <see cref="DisposeAsyncCore"/> once.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		if (!_disposedAsync)
		{
			await DisposeAsyncCore();
			GC.SuppressFinalize(this);
			_disposedAsync = true;
		}
	}

	/// <summary>
	/// Override to clear managed resources created by a derived test class.
	/// </summary>
	protected virtual void Clear() { }

	/// <summary>
	/// Override to implement asynchronous disposal of resources.
	/// Default implementation is a completed <see cref="ValueTask"/>.
	/// </summary>
	protected virtual ValueTask DisposeAsyncCore() => new();
}
