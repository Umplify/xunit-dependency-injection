namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures;

public class TestProjectFixtureWithoutAppsettings : TestBedFixture
{
  protected override void AddServices(IServiceCollection services, IConfiguration configuration)
  {
  }

  protected override ValueTask DisposeAsyncCore() => new();
}