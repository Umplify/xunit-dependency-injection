using Microsoft.Extensions.Options;
using Options = Xunit.Microsoft.DependencyInjection.ExampleTests.Services.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

public class CalculatorTests : TestBed<TestProjectFixture>
{
    private readonly Options _options;

    public CalculatorTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture)
        : base(testOutputHelper, fixture) => _options = _fixture.GetService<IOptions<Options>>(_testOutputHelper)!.Value;

    [Theory]
    [InlineData(1, 2)]
    public async Task TestServiceAsync(int x, int y)
    {
        var calculator = _fixture.GetService<ICalculator>(_testOutputHelper)!;
        var calculatedValue = await calculator.AddAsync(x, y);
        var expected = _options.Rate * (x + y);
        Assert.Equal(expected, calculatedValue);
    }

    [Theory]
    [InlineData(1, 2)]
    public async Task TestScopedServiceAsync(int x, int y)
    {
        var calculator = _fixture.GetScopedService<ICalculator>(_testOutputHelper)!;
        var calculatedValue = await calculator.AddAsync(x, y);
        var expected = _options.Rate * (x + y);
        Assert.Equal(expected, calculatedValue);
    }
}
