
using Microsoft.Extensions.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

public class UserSecretTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture) : TestBed<TestProjectFixture>(testOutputHelper, fixture)
{
    [Fact]
    public void TestSecretValues()
    {
        var secretValues = _fixture.GetService<IOptions<SecretValues>>(_testOutputHelper)!.Value;
        Assert.NotEmpty(secretValues?.Secret1 ?? string.Empty);
        Assert.NotEmpty(secretValues?.Secret1 ?? string.Empty);
    }
}