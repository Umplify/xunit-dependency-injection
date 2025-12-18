namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

public class ConfigurationTestsWithoutAppsettings : TestBed<TestProjectFixtureWithoutAppsettings>
{
    private const string Key = "CONFIG_KEY";
    private const string Value = "Value";
    
    public ConfigurationTestsWithoutAppsettings(ITestOutputHelper testOutputHelper, TestProjectFixtureWithoutAppsettings fixture) 
        : base(testOutputHelper, fixture)
    {
        Environment.SetEnvironmentVariable(Key, Value);
    }
    
    [Fact]
    public void EnvironmentVariablesViaConstructorAreAvailable()
    {
        _fixture.GetServiceProvider(_testOutputHelper);

        var value = _fixture.Configuration!.GetValue<string>(Key);

        Assert.Equal(Value, value);
    }
}
