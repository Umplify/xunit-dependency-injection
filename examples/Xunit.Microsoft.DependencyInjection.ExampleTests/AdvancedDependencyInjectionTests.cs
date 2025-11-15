using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options = Xunit.Microsoft.DependencyInjection.ExampleTests.Services.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Example tests demonstrating advanced dependency injection patterns including IOptions, Func&lt;T&gt;, and Action&lt;T&gt;
/// </summary>
public class AdvancedDependencyInjectionTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture) : TestBedWithDI<TestProjectFixture>(testOutputHelper, fixture)
{
    [Inject]
    private IOptions<Options>? Options { get; set; }

    [Inject]
    private IOptionsSnapshot<Options>? OptionsSnapshot { get; set; }

    [Inject]
    private IOptionsMonitor<Options>? OptionsMonitor { get; set; }

    [Inject]
    private ICalculator? Calculator { get; set; }

    [Inject]
    private IScopedService? ScopedService { get; set; }

    [Inject]
    private ISingletonService? SingletonService { get; set; }

    [Fact]
    public void TestIOptionsPatterns()
    {
        // Arrange & Assert - All options patterns should be available
        Assert.NotNull(Options);
        Assert.NotNull(OptionsSnapshot);
        Assert.NotNull(OptionsMonitor);

        // IOptions<T> - Singleton, same configuration throughout app lifetime
        Assert.NotNull(Options.Value);
        Assert.True(Options.Value.Rate > 0);

        // IOptionsSnapshot<T> - Scoped, allows for configuration changes per request/scope
        Assert.NotNull(OptionsSnapshot.Value);
        Assert.Equal(Options.Value.Rate, OptionsSnapshot.Value.Rate);

        // IOptionsMonitor<T> - Singleton, supports configuration change notifications
        Assert.NotNull(OptionsMonitor.CurrentValue);
        Assert.Equal(Options.Value.Rate, OptionsMonitor.CurrentValue.Rate);
    }

    [Fact]
    public void TestFuncTPatternWithCalculator()
    {
        // Arrange - Create Func<ICalculator> factory
        var calculatorFactory = GetService<Func<ICalculator>>();
        Assert.NotNull(calculatorFactory);

        // Act - Create calculators through factory
        var calc1 = calculatorFactory();
        var calc2 = calculatorFactory();

        // Assert - Should create new transient instances each time
        Assert.NotNull(calc1);
        Assert.NotNull(calc2);
        Assert.NotSame(calc1, calc2); // Different instances for transient service
    }

    [Fact]
    public void TestFuncTPatternWithScopedService()
    {
        // Arrange - Create Func<IScopedService> factory
        var scopedFactory = GetService<Func<IScopedService>>();
        Assert.NotNull(scopedFactory);

        // Act - Get scoped service through factory
        var scopedThroughFactory = scopedFactory();

        // Assert - Should be the same scoped instance as injected property
        Assert.NotNull(ScopedService);
        Assert.NotNull(scopedThroughFactory);
        Assert.Same(ScopedService, scopedThroughFactory); // Same instance for scoped service
        Assert.Equal(ScopedService.InstanceId, scopedThroughFactory.InstanceId);
    }

    [Fact]
    public void TestFuncTPatternWithSingletonService()
    {
        // Arrange - Create Func<ISingletonService> factory
        var singletonFactory = GetService<Func<ISingletonService>>();
        Assert.NotNull(singletonFactory);

        // Act - Get singleton service through factory
        var singletonThroughFactory = singletonFactory();

        // Assert - Should be the same singleton instance as injected property
        Assert.NotNull(SingletonService);
        Assert.NotNull(singletonThroughFactory);
        Assert.Same(SingletonService, singletonThroughFactory); // Same instance for singleton service
        Assert.Equal(SingletonService.InstanceId, singletonThroughFactory.InstanceId);
    }

    [Fact]
    public void TestActionTPatternWithServices()
    {
        // Arrange - Create actions to work with different service types
        var calculatorResults = new List<int>();
        var scopedResults = new List<string>();
        var singletonResults = new List<string>();

        // Capture initial states to make relative assertions
        Assert.NotNull(Calculator);
        Assert.NotNull(ScopedService);
        Assert.NotNull(SingletonService);
        Assert.NotNull(Options);

        var initialScopedCounter = ScopedService.Counter;
        var initialGlobalCounter = SingletonService.GlobalCounter;

        Action<ICalculator> calculatorAction = async calc =>
        {
            var result = await calc.AddAsync(10, 5);
            calculatorResults.Add(result);
        };

        Action<IScopedService> scopedAction = service =>
        {
            service.Increment();
            scopedResults.Add($"Counter: {service.Counter}, Instance: {service.InstanceId}");
        };

        Action<ISingletonService> singletonAction = service =>
        {
            service.IncrementGlobal();
            singletonResults.Add($"Global: {service.GlobalCounter}, Instance: {service.InstanceId}");
        };

        // Act - Execute actions with services
        calculatorAction(Calculator!);
        scopedAction(ScopedService!);
        singletonAction(SingletonService!);

        // Execute actions with services obtained through service provider
        var anotherCalculator = GetService<ICalculator>();
        var sameScopedService = GetService<IScopedService>();
        var sameSingletonService = GetService<ISingletonService>();

        calculatorAction(anotherCalculator!);
        scopedAction(sameScopedService!);
        singletonAction(sameSingletonService!);

        // Assert - Actions should have been executed
        Assert.Equal(2, calculatorResults.Count);
        Assert.Equal(2, scopedResults.Count);
        Assert.Equal(2, singletonResults.Count);

        // Calculator results should be the same (same options, same calculation)
        var expectedResult = Options.Value.Rate * (10 + 5);
        Assert.All(calculatorResults, result => Assert.Equal(expectedResult, result));

        // Scoped service results should show incrementing counter for same instance
        Assert.All(scopedResults, result => Assert.Contains(ScopedService.InstanceId.ToString(), result));

        // Use relative assertions - counter should increment from initial state
        Assert.Contains($"Counter: {initialScopedCounter + 1}", scopedResults[0]);
        Assert.Contains($"Counter: {initialScopedCounter + 2}", scopedResults[1]);

        // Singleton service results should show incrementing global counter
        Assert.All(singletonResults, result => Assert.Contains(SingletonService.InstanceId.ToString(), result));

        // Use relative assertions - global counter should increment by 2 from initial state
        Assert.Contains($"Global: {initialGlobalCounter + 1}", singletonResults[0]);
        Assert.Contains($"Global: {initialGlobalCounter + 2}", singletonResults[1]);
    }

    [Fact]
    public void TestComplexFuncPatternWithMultipleServices()
    {
        // Arrange - Create a complex function that uses multiple services
        Func<IScopedService, ISingletonService, ICalculator, string> complexOperation =
            (scopedSvc, singletonSvc, calc) =>
            {
                scopedSvc.Increment();
                singletonSvc.IncrementGlobal();
                return $"Scoped: {scopedSvc.InstanceId}, Singleton: {singletonSvc.InstanceId}, " +
                       $"ScopedCounter: {scopedSvc.Counter}, GlobalCounter: {singletonSvc.GlobalCounter}";
            };

        // Act - Execute complex operation
        Assert.NotNull(ScopedService);
        Assert.NotNull(SingletonService);
        Assert.NotNull(Calculator);

        var result = complexOperation(ScopedService, SingletonService, Calculator);

        // Assert - Result should contain information from all services
        Assert.Contains(ScopedService.InstanceId.ToString(), result);
        Assert.Contains(SingletonService.InstanceId.ToString(), result);
        Assert.Contains("ScopedCounter:", result);
        Assert.Contains("GlobalCounter:", result);
    }

    [Fact]
    public void TestIServiceProviderInjection()
    {
        // Arrange - IServiceProvider should be injectable
        var serviceProvider = GetService<IServiceProvider>();
        Assert.NotNull(serviceProvider);

        // Act - Use IServiceProvider to resolve services
        var calculator = serviceProvider.GetService<ICalculator>();
        var scopedService = serviceProvider.GetService<IScopedService>();
        var singletonService = serviceProvider.GetService<ISingletonService>();
        var options = serviceProvider.GetService<IOptions<Options>>();

        // Assert - All services should be resolvable
        Assert.NotNull(calculator);
        Assert.NotNull(scopedService);
        Assert.NotNull(singletonService);
        Assert.NotNull(options);

        // Scoped and singleton services should be the same as injected ones
        Assert.Same(ScopedService, scopedService);
        Assert.Same(SingletonService, singletonService);
        Assert.Same(Options, options);
    }
}