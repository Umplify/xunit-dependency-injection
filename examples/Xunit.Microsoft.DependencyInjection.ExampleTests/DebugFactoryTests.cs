using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Attributes;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Debug tests for keyed service resolution
/// </summary>
public class DebugFactoryTests : TestBed<FactoryTestProjectFixture>
{
    public DebugFactoryTests(ITestOutputHelper testOutputHelper, FactoryTestProjectFixture fixture)
        : base(testOutputHelper, fixture)
    {
    }

    [Fact]
    public void TestDirectKeyedServiceResolution()
    {
        // Test that keyed services work directly through the fixture
        var porsche = _fixture.GetKeyedService<ICarMaker>("Porsche", _testOutputHelper);
        var toyota = _fixture.GetKeyedService<ICarMaker>("Toyota", _testOutputHelper);

        Assert.NotNull(porsche);
        Assert.NotNull(toyota);
        Assert.Equal("Porsche", porsche.Manufacturer);
        Assert.Equal("Toyota", toyota.Manufacturer);
    }

    [Fact]
    public void TestSingleKeyedServiceFactory()
    {
        // Test creating a service with a single keyed dependency
        var singleKeyedService = _fixture.CreateTestInstance<SingleKeyedService>(_testOutputHelper);

        Assert.NotNull(singleKeyedService);
        Assert.Equal("Porsche", singleKeyedService.GetManufacturer());
    }
}

/// <summary>
/// Service with only one keyed dependency for easier testing
/// </summary>
public class SingleKeyedService
{
    private readonly ICarMaker _carMaker;

    internal SingleKeyedService([FromKeyedService("Porsche")] ICarMaker carMaker)
    {
        _carMaker = carMaker ?? throw new ArgumentNullException(nameof(carMaker));
    }

    public string GetManufacturer() => _carMaker.Manufacturer;
}