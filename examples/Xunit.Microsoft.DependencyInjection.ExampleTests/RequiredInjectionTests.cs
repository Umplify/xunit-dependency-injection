namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>A service that <see cref="TestProjectFixture"/> deliberately does not register.</summary>
public interface IUnregisteredService;

/// <summary>
/// <c>[Inject(Required = true)]</c> fails test-class construction with a message naming the
/// missing service and the fixture, instead of leaving the property null.
/// </summary>
public class RequiredInjectionTests(TestProjectFixture fixture) : TestBedWithDI<TestProjectFixture>(fixture)
{
    [Inject(Required = true)]
    private ICalculator? Calculator { get; set; }

    [Inject("Porsche", Required = true)]
    private ICarMaker? Porsche { get; set; }

    [Fact]
    public void RegisteredServicesAreInjected()
    {
        Assert.NotNull(Calculator);
        Assert.NotNull(Porsche);
    }

    [Fact]
    public void MissingRequiredServiceThrowsWithAClearMessage()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new NeedsUnregisteredService(_fixture));

        Assert.Contains(nameof(IUnregisteredService), exception.Message);
        Assert.Contains(nameof(TestProjectFixture), exception.Message);
        Assert.Contains(nameof(NeedsUnregisteredService.Service), exception.Message);
    }

    [Fact]
    public void MissingRequiredKeyedServiceThrows()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new NeedsUnknownKey(_fixture));

        Assert.Contains("Ferrari", exception.Message);
    }

    /// <summary>
    /// When resolving throws (here: a scoped service from the root of a container that validates
    /// scopes), the error is wrapped with the property and fixture names and kept as the inner exception.
    /// </summary>
    [Fact]
    public void FailingResolutionOfRequiredServiceIsWrapped()
    {
        using var fixture = new ValidatingFixture();

        var exception = Assert.Throws<InvalidOperationException>(() => new NeedsScopedService(fixture));

        Assert.Contains(nameof(NeedsScopedService.Scoped), exception.Message);
        Assert.Contains(nameof(ValidatingFixture), exception.Message);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public void MissingOptionalServiceIsLeftNull()
        => Assert.Null(new OptionalUnregisteredService(_fixture).Service);

    // Not test classes: constructed directly to observe what happens during injection.
    public class NeedsUnregisteredService(TestProjectFixture fixture) : TestBedWithDI<TestProjectFixture>(fixture)
    {
        [Inject(Required = true)]
        public IUnregisteredService? Service { get; set; }
    }

    public class NeedsUnknownKey(TestProjectFixture fixture) : TestBedWithDI<TestProjectFixture>(fixture)
    {
        [Inject("Ferrari", Required = true)]
        internal ICarMaker? CarMaker { get; set; }
    }

    public class NeedsScopedService(ValidatingFixture fixture) : TestBedWithDI<ValidatingFixture>(fixture)
    {
        [Inject(Required = true)]
        public IScopedService? Scoped { get; set; }
    }

    public class OptionalUnregisteredService(TestProjectFixture fixture) : TestBedWithDI<TestProjectFixture>(fixture)
    {
        [Inject]
        public IUnregisteredService? Service { get; set; }
    }
}
