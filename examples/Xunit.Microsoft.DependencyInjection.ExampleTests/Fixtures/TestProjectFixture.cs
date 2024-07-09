﻿namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures;

public class TestProjectFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
        .AddTransient<ICalculator, Calculator>()
        .AddKeyedTransient<ICarMaker, Porsche>("Porsche")
        .AddKeyedTransient<ICarMaker, Toyota>("Toyota")
        .Configure<Options>(config => configuration?.GetSection("Options").Bind(config))
        .Configure<SecretValues>(config => configuration?.GetSection(nameof(SecretValues)));

    protected override ValueTask DisposeAsyncCore()
        => new();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = false };
    }

    protected override void AddUserSecrets(IConfigurationBuilder configurationBuilder) 
        => configurationBuilder.AddUserSecrets<TestProjectFixture>();
}
