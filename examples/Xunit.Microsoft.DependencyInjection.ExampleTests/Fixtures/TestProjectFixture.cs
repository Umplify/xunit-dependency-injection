namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures;

public class TestProjectFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
        => services
        // Transient services - new instance for each injection
        .AddTransient<ICalculator, Calculator>()
        .AddTransient<ITransientService, TransientService>()
        .AddKeyedTransient<ICarMaker, Porsche>("Porsche")
        .AddKeyedTransient<ICarMaker, Toyota>("Toyota")

        // Scoped services - same instance within a scope (test)
        .AddScoped<IScopedService, ScopedService>()

        // Singleton services - same instance across entire application lifetime
        .AddSingleton<ISingletonService, SingletonService>()

        // Register Func<T> factories for all services
        .AddTransient<Func<ICalculator>>(provider => () => provider.GetService<ICalculator>()!)
        .AddTransient<Func<ITransientService>>(provider => () => provider.GetService<ITransientService>()!)
        .AddTransient<Func<IScopedService>>(provider => () => provider.GetService<IScopedService>()!)
        .AddTransient<Func<ISingletonService>>(provider => () => provider.GetService<ISingletonService>()!)

        // Configure options
        .Configure<Options>(config => configuration.GetSection("Options").Bind(config))
        .Configure<SecretValues>(config => configuration.GetSection(nameof(SecretValues)).Bind(config));

    protected override ValueTask DisposeAsyncCore()
        => new();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = false };
    }

    protected override void AddUserSecrets(IConfigurationBuilder configurationBuilder)
        => configurationBuilder.AddUserSecrets<TestProjectFixture>();
}
