namespace Xunit.Microsoft.DependencyInjection.Abstracts;

public class TestBed<TFixture> : IDisposable, IClassFixture<TFixture>, IAsyncDisposable
	where TFixture : class
{
	protected readonly ITestOutputHelper _testOutputHelper;
	protected readonly TFixture _fixture;
	private bool _disposedValue;
	private bool _disposedAsync;

	public TestBed(ITestOutputHelper testOutputHelper, TFixture fixture)
		=> (_testOutputHelper, _fixture) = (testOutputHelper, fixture);

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
				Clear();
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			_disposedValue = true;
		}
	}

	// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	// ~AbstractTest()
	// {
	//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
	//     Dispose(disposing: false);
	// }

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public async ValueTask DisposeAsync()
	{
		if (!_disposedAsync)
		{
			await DisposeAsyncCore();
			GC.SuppressFinalize(this);
			_disposedAsync = true;
		}
	}

	protected virtual void Clear() { }
	protected virtual ValueTask DisposeAsyncCore() => new();
}
