namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// By default (10.x) user secrets are added before the JSON files, so appsettings.json overrides them.
/// </summary>
public class LegacyConfigurationPrecedenceTests(ITestOutputHelper testOutputHelper, LegacyConfigurationPrecedenceFixture fixture)
    : TestBed<LegacyConfigurationPrecedenceFixture>(testOutputHelper, fixture)
{
    [Fact]
    public void JsonOverridesUserSecrets()
    {
        _fixture.GetServiceProvider(_testOutputHelper);

        Assert.Equal("from-json", _fixture.Configuration![ConfigurationPrecedenceFixtureBase.JsonOrSecretsKey]);
        Assert.Equal("from-env", _fixture.Configuration![ConfigurationPrecedenceFixtureBase.SecretsOrEnvironmentKey]);
    }
}

/// <summary>
/// With <see cref="TestBedFixture.UseStandardConfigurationPrecedence"/> the fixture follows the
/// standard .NET order: JSON files, then user secrets, then environment variables.
/// </summary>
public class StandardConfigurationPrecedenceTests(ITestOutputHelper testOutputHelper, StandardConfigurationPrecedenceFixture fixture)
    : TestBed<StandardConfigurationPrecedenceFixture>(testOutputHelper, fixture)
{
    [Fact]
    public void UserSecretsOverrideJson()
    {
        _fixture.GetServiceProvider(_testOutputHelper);

        Assert.Equal("from-secrets", _fixture.Configuration![ConfigurationPrecedenceFixtureBase.JsonOrSecretsKey]);
        Assert.Equal("from-env", _fixture.Configuration![ConfigurationPrecedenceFixtureBase.SecretsOrEnvironmentKey]);
    }
}
