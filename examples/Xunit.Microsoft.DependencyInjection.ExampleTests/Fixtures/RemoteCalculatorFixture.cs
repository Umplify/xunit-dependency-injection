using Microsoft.AspNetCore.Mvc.Testing;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures;

public class RemoteCalculatorFixture : TestBedFixture
{
    public WebApplicationFactory<Program> Factory { get; }

    public RemoteCalculatorFixture()
    {
        Factory = new WebApplicationFactory<Program>();
    }

    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
            .AddSingleton<IHttpClientFactory>(_ => new StaticClientFactory(Factory.CreateClient()))
            .AddTransient<ICalculator, RemoteCalculator>()
            .Configure<Options>(config => configuration?.GetSection("Options").Bind(config));

    protected override ValueTask DisposeAsyncCore()
        => new();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = false };
    }
}
