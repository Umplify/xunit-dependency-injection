
using Microsoft.Extensions.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

public class UserSecretTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture) : TestBed<TestProjectFixture>(testOutputHelper, fixture)
{
    [Fact]
    public void TestSecretValues()
    {
        /*
         * TODO: Create a user secret entry like the following payload in user secrets and remove the same from appsettings.json file:
         * 
         * "SecretValues": {
         *   "Secret1": "secret1value",
         *   "Secret2": "secret2value"
         * }
         */
        var secretValues = _fixture.GetService<IOptions<SecretValues>>(_testOutputHelper)!.Value;
        Assert.NotEmpty(secretValues?.Secret1 ?? string.Empty);
        Assert.NotEmpty(secretValues?.Secret1 ?? string.Empty);
    }
}