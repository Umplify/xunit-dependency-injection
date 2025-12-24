namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

public class AsyncDisposableTests : TestBed<AsyncDisposableFixture>
{
    public AsyncDisposableTests(ITestOutputHelper testOutputHelper, AsyncDisposableFixture fixture) 
        : base(testOutputHelper, fixture)
    {
    }

    // The test itself will pass but `dotnet  test` will fail in the teardown
    // when the AsyncDisposableService has not been disposed asynchronously.
    [Fact]
    public void AsyncDisposableServiceGetsDisposedAsynchronously()
    {
        var service = _fixture.GetService<AsyncDisposableService>(_testOutputHelper);
        Assert.NotNull(service);
    }
}
