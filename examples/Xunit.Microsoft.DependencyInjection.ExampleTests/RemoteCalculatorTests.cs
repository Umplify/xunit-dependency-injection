using Microsoft.Extensions.Options;
using Options = Xunit.Microsoft.DependencyInjection.ExampleTests.Services.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

public class RemoteCalculatorTests : TestBed<RemoteCalculatorFixture>
{
    public RemoteCalculatorTests(ITestOutputHelper testOutputHelper, RemoteCalculatorFixture fixture)
        : base(testOutputHelper, fixture)
    {
    }

    [Theory]
    [InlineData(1, 2)]
    public async Task Test1(int x, int y)
    {
        var calculator = _fixture.GetService<ICalculator>(_testOutputHelper);
        var option = _fixture.GetService<IOptions<Options>>(_testOutputHelper);
        var calculated = await calculator?.Add(x, y);
        var expected = option?.Value.Rate * (x + y);
        Assert.True(expected == calculated);
    }
}
