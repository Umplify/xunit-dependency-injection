using Microsoft.Extensions.Options;
using Options = Xunit.Microsoft.DependencyInjection.ExampleTests.Services.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

public class CalculatorTests : TestBed<CalculatorFixture>
{
    private readonly ICalculator _calculator;
    private readonly Options _options;

    public CalculatorTests(ITestOutputHelper testOutputHelper, CalculatorFixture fixture)
        : base(testOutputHelper, fixture)
    {
        _calculator = _fixture.GetService<ICalculator>(_testOutputHelper)!;
        _options = _fixture.GetService<IOptions<Options>>(_testOutputHelper)!.Value;
    }

    [Theory]
    [InlineData(1, 2)]
    public async Task Test1Async(int x, int y)
    {
        var calculatedValue = await _calculator.AddAsync(x, y);
        var expected = _options.Rate * (x + y);
        Assert.True(expected == calculatedValue);
    }

    [Theory]
    [InlineData(1, 2)]
    public async Task Test2Async(int x, int y)
    {
        var calculatedValue = await _calculator.AddAsync(x, y);
        var expected = _options.Rate * (x + y);
        Assert.True(expected == calculatedValue);
    }
}
