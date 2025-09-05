namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures;

public class FactoryTestProjectFixture : TestBedFactoryFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
        .AddTransient<ICalculator, Calculator>()
        .AddKeyedTransient<ICarMaker, Porsche>("Porsche")
        .AddKeyedTransient<ICarMaker, Toyota>("Toyota")
        .Configure<Services.Options>(config => configuration?.GetSection("Options").Bind(config))
        .Configure<SecretValues>(config => configuration?.GetSection(nameof(SecretValues)).Bind(config))
        // Register the CalculatorService as well for factory injection
        .AddTransient<CalculatorService>();

    protected override ValueTask DisposeAsyncCore()
        => new();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = false };
    }

    protected override void AddUserSecrets(IConfigurationBuilder configurationBuilder) 
        => configurationBuilder.AddUserSecrets<FactoryTestProjectFixture>();
}