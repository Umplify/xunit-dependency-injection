namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

public class AsyncDisposableTests : TestBed<AsyncDisposableFixture>
{
    public AsyncDisposableTests(ITestOutputHelper testOutputHelper, AsyncDisposableFixture fixture) 
        : base(testOutputHelper, fixture)
    {
    }
    
    [Fact]
    public void EnvironmentVariablesViaConstructorAreAvailable()
    {
        var service = _fixture.GetService<AsyncDisposableService>(_testOutputHelper);
        Assert.NotNull(service);
    }
}
