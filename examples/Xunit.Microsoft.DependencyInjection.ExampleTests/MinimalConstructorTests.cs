namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Test classes only need to accept the fixture: the test output helper is taken from
/// <see cref="TestContext.Current"/>, which xUnit.net sets before it constructs the test class.
/// </summary>
public class MinimalConstructorTests(TestProjectFixture fixture) : TestBedWithDI<TestProjectFixture>(fixture)
{
    [Inject]
    private ICalculator? Calculator { get; set; }

    [Fact]
    public void PropertiesAreInjected() => Assert.NotNull(Calculator);

    [Fact]
    public void OutputHelperComesFromTheCurrentTest()
        => Assert.Same(TestContext.Current.TestOutputHelper, _testOutputHelper);

    [Fact]
    public async Task FixtureResolvesServicesWithoutAnOutputHelper()
    {
        Assert.NotNull(_fixture.GetService<ICalculator>());
        Assert.IsType<Porsche>(_fixture.GetKeyedService<ICarMaker>("Porsche"));
        Assert.Same(_fixture.GetServiceProvider(), _fixture.GetServiceProvider(_testOutputHelper));

        await using var scope = _fixture.GetAsyncScope();
        Assert.NotNull(scope.ServiceProvider.GetService<IScopedService>());
    }
}

/// <summary>
/// The same single-argument constructor is available on the plain <see cref="TestBed{TFixture}"/>.
/// </summary>
public class MinimalConstructorTestBedTests(TestProjectFixture fixture) : TestBed<TestProjectFixture>(fixture)
{
    [Fact]
    public async Task ResolvesThroughTheFixture()
    {
        var calculator = _fixture.GetService<ICalculator>()!;

        Assert.Equal(80, await calculator.AddAsync(5, 3));
    }
}
