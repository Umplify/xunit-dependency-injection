using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures
{
    public class TestFixture : TestBedFixture
    {
        protected override void AddServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddTransient<ICalculator, Calculator>()
                .Configure<Options>(config => configuration.GetSection("Options").Bind(config));

        protected override string GetConfigurationFile()
            => "appsettings.json";
    }
}
