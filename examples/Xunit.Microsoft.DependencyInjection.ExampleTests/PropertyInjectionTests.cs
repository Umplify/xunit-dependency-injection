using Microsoft.Extensions.Options;
using Options = Xunit.Microsoft.DependencyInjection.ExampleTests.Services.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Example tests demonstrating property injection using the new TestBedWithDI base class
/// </summary>
public class PropertyInjectionTests : TestBedWithDI<TestProjectFixture>
{
    [Inject]
    public ICalculator? Calculator { get; set; }

    [Inject]
    public IOptions<Options>? Options { get; set; }

    [Inject("Porsche")]
    internal ICarMaker? PorscheCarMaker { get; set; }

    [Inject("Toyota")]
    internal ICarMaker? ToyotaCarMaker { get; set; }

    public PropertyInjectionTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture)
        : base(testOutputHelper, fixture)
    {
    }

    [Fact]
    public async Task TestCalculatorThroughPropertyInjection()
    {
        // Arrange - dependencies are already injected via properties
        Assert.NotNull(Calculator);
        Assert.NotNull(Options);

        // Act
        var result = await Calculator.AddAsync(5, 3);

        // Assert
        var expected = Options.Value.Rate * (5 + 3);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestKeyedServicesThroughPropertyInjection()
    {
        // Arrange - keyed services are already injected via properties
        Assert.NotNull(PorscheCarMaker);
        Assert.NotNull(ToyotaCarMaker);

        // Assert
        Assert.Equal("Porsche", PorscheCarMaker.Manufacturer);
        Assert.Equal("Toyota", ToyotaCarMaker.Manufacturer);
    }

    [Theory]
    [InlineData(10, 20)]
    public async Task TestConvenienceMethodsStillWork(int x, int y)
    {
        // Demonstrate that convenience methods from the base class still work
        var calculator = GetService<ICalculator>();
        var options = GetService<IOptions<Options>>();
        var porsche = GetKeyedService<ICarMaker>("Porsche");

        Assert.NotNull(calculator);
        Assert.NotNull(options);
        Assert.NotNull(porsche);

        var result = await calculator.AddAsync(x, y);
        var expected = options.Value.Rate * (x + y);
        Assert.Equal(expected, result);
    }
}