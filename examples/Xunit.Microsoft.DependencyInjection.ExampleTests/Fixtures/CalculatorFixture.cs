namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures;

public class CalculatorFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
            .AddTransient<ICalculator, Calculator>()
            .Configure<Options>(config => configuration?.GetSection("Options").Bind(config));

    protected override ValueTask DisposeAsyncCore()
        => new();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = false };
    }
}
