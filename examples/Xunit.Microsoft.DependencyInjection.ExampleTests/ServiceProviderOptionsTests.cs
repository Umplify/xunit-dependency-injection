namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Turns on the container's own checks: scoped services may not be resolved from the root
/// provider (which would silently turn them into singletons), and every registration is
/// validated when the container is built.
/// </summary>
public class ValidatingFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
        => services.AddScoped<IScopedService, ScopedService>();

    protected override ServiceProviderOptions? GetServiceProviderOptions()
        => new() { ValidateScopes = true, ValidateOnBuild = true };
}

/// <summary>A service whose dependency is never registered.</summary>
public class NeedsMissingDependency(IUnregisteredService dependency)
{
    public IUnregisteredService Dependency { get; } = dependency;
}

/// <summary>
/// With <c>ValidateOnBuild</c>, a registration whose dependencies cannot be satisfied fails when the
/// container is built, not when the service is first resolved.
/// </summary>
public class ValidateOnBuildFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
        => services.AddTransient<NeedsMissingDependency>();

    protected override ServiceProviderOptions? GetServiceProviderOptions()
        => new() { ValidateOnBuild = true };
}

public class ValidateOnBuildTests(ValidateOnBuildFixture fixture) : TestBed<ValidateOnBuildFixture>(fixture)
{
    [Fact]
    public void MissingDependencyIsReportedWhenTheContainerIsBuilt()
    {
        var exception = Assert.Throws<AggregateException>(() => _fixture.GetServiceProvider());

        Assert.Contains(nameof(IUnregisteredService), exception.Message);
    }
}

public class ServiceProviderOptionsTests(ITestOutputHelper testOutputHelper, ValidatingFixture fixture)
    : TestBed<ValidatingFixture>(testOutputHelper, fixture)
{
    [Fact]
    public void ScopedServiceCannotBeResolvedFromTheRootProvider()
        => Assert.Throws<InvalidOperationException>(() => _fixture.GetService<IScopedService>(_testOutputHelper));

    [Fact]
    public async Task ScopedServiceResolvesFromAScope()
    {
        await using var scope = _fixture.GetAsyncScope(_testOutputHelper);

        Assert.NotNull(scope.ServiceProvider.GetService<IScopedService>());
    }
}
