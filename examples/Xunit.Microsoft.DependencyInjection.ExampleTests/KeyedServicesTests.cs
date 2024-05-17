namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

public class KeyedServicesTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture) : TestBed<TestProjectFixture>(testOutputHelper, fixture)
{
    [Theory]
    [InlineData(nameof(Porsche))]
    [InlineData(nameof(Toyota))]
    public void GetKeyedService(string key)
    {
        var carMaker = _fixture.GetKeyedService<ICarMaker>(key, _testOutputHelper)!;
        Assert.Equal(key, carMaker.Manufacturer);
    }
}
