namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures;

/// <summary>
/// Supplies the same keys from appsettings.json, "user secrets" and environment variables so the
/// tests can observe which source wins. An in-memory source stands in for user secrets, so the
/// tests do not depend on secrets being configured on the machine.
/// </summary>
public abstract class ConfigurationPrecedenceFixtureBase : TestBedFixture
{
    internal const string JsonOrSecretsKey = "ConfigurationPrecedence:JsonOrSecrets";
    internal const string SecretsOrEnvironmentKey = "ConfigurationPrecedence:SecretsOrEnvironment";

    static ConfigurationPrecedenceFixtureBase()
        => Environment.SetEnvironmentVariable("ConfigurationPrecedence__SecretsOrEnvironment", "from-env");

    protected override void AddServices(IServiceCollection services, IConfiguration configuration) { }

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = false };
    }

    protected override void AddUserSecrets(IConfigurationBuilder configurationBuilder)
        => configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [JsonOrSecretsKey] = "from-secrets",
            [SecretsOrEnvironmentKey] = "from-secrets",
        });
}

/// <summary>Keeps the 10.x default order: user secrets, then JSON files, then environment variables.</summary>
public class LegacyConfigurationPrecedenceFixture : ConfigurationPrecedenceFixtureBase;

/// <summary>Opts in to the standard .NET order: JSON files, then user secrets, then environment variables.</summary>
public class StandardConfigurationPrecedenceFixture : ConfigurationPrecedenceFixtureBase
{
    protected override bool UseStandardConfigurationPrecedence => true;
}
