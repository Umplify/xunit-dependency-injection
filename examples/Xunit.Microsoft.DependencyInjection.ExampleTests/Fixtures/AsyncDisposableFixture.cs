namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures;

public class AsyncDisposableFixture : TestBedFixture
{
  protected override void AddServices(
    IServiceCollection services,
    IConfiguration configuration
  )
  {
    services.AddSingleton<AsyncDisposableService>();
  }
}