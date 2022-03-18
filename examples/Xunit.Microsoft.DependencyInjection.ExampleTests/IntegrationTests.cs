using Microsoft.Extensions.Options;
using Options = Xunit.Microsoft.DependencyInjection.ExampleTests.Services.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

public class IntegrationTests : TestBed<TestFixture>
{
    public IntegrationTests(ITestOutputHelper testOutputHelper, TestFixture fixture)
        : base(testOutputHelper, fixture)
    {
    }

    [Theory]
    [InlineData(1, 2)]
    public void Test1(int x, int y)
    {
        var calculator = _fixture.GetService<ICalculator>(_testOutputHelper);
        var option = _fixture.GetService<IOptions<Options>>(_testOutputHelper);
        var calculated = calculator?.Add(x, y);
        var expected = option?.Value.Rate * (x + y);
        Assert.True(expected == calculated);
    }

    [Theory]
    [InlineData(1, 2)]
    public void Test2(int x, int y)
    {
        var calculator = _fixture.GetScopedService<ICalculator>(_testOutputHelper);
        var option = _fixture.GetScopedService<IOptions<Options>>(_testOutputHelper);
        var calculated = calculator?.Add(x, y);
        var expected = option?.Value.Rate * (x + y);
        Assert.True(expected == calculated);
    }
}
