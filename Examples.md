# Xunit Dependency Injection - Comprehensive Examples

This document provides comprehensive examples demonstrating all the ways to use the Xunit.Microsoft.DependencyInjection library. All examples are taken from working test code in the `examples/` directory.

## Table of Contents

1. [Basic Setup](#basic-setup)
2. [Traditional Fixture-Based Approach](#traditional-fixture-based-approach)
3. [Property Injection (Recommended)](#property-injection-recommended)
4. [Keyed Services](#keyed-services)
5. [Factory Pattern (Experimental)](#factory-pattern-experimental)
6. [Configuration and User Secrets](#configuration-and-user-secrets)
7. [Advanced Dependency Injection Patterns](#advanced-dependency-injection-patterns)
8. [Service Lifetimes](#service-lifetimes)
9. [Test Ordering](#test-ordering)

## Basic Setup

### 1. Creating a Test Fixture

First, create a test fixture that derives from `TestBedFixture`:

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

public class TestProjectFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
        // Transient services - new instance for each injection
        .AddTransient<ICalculator, Calculator>()
        .AddTransient<ITransientService, TransientService>()
        .AddKeyedTransient<ICarMaker, Porsche>("Porsche")
        .AddKeyedTransient<ICarMaker, Toyota>("Toyota")

        // Scoped services - same instance within a scope (test)
        .AddScoped<IScopedService, ScopedService>()

        // Singleton services - same instance across entire application lifetime
        .AddSingleton<ISingletonService, SingletonService>()

        // Configure options
        .Configure<Options>(config => configuration?.GetSection("Options").Bind(config))
        .Configure<SecretValues>(config => configuration?.GetSection(nameof(SecretValues)).Bind(config));

    protected override ValueTask DisposeAsyncCore()
        => new();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = false };
    }

    protected override void AddUserSecrets(IConfigurationBuilder configurationBuilder)
        => configurationBuilder.AddUserSecrets<TestProjectFixture>();
}
```

### 2. Configuration File

Create an `appsettings.json` file in your test project:

```json
{
  "Options": {
    "Rate": 10
  },
  "SecretValues": {
    "Secret1": "StoreSecret1InUserSecrets",
    "Secret2": "StoreSecret2InUserSecrets"
  }
}
```

### 3. Example Services

```csharp
public interface ICalculator
{
    Task<int> AddAsync(int x, int y);
}

public class Calculator : ICalculator
{
    private readonly Options _option;
    private readonly ILogger<Calculator> _logger;

    public Calculator(ILogger<Calculator> logger, IOptions<Options> option)
    {
        _option = option.Value;
        _logger = logger;
    }

    public Task<int> AddAsync(int x, int y)
    {
        var result = (x + y) * _option.Rate;
        _logger.LogInformation("The result is {@Result}", result);
        return Task.FromResult(result);
    }
}

public class Options
{
    public int Rate { get; set; }
}
```

## Traditional Fixture-Based Approach

This is the classic approach that works with all versions of the library:

```csharp
using Microsoft.Extensions.Options;
using Xunit.Microsoft.DependencyInjection.Abstracts;

public class CalculatorTests : TestBed<TestProjectFixture>
{
    private readonly Options _options;

    public CalculatorTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture)
        : base(testOutputHelper, fixture) 
    {
        _options = _fixture.GetService<IOptions<Options>>(_testOutputHelper)!.Value;
    }

    [Theory]
    [InlineData(1, 2)]
    public async Task TestServiceAsync(int x, int y)
    {
        // Get service from fixture
        var calculator = _fixture.GetService<ICalculator>(_testOutputHelper)!;
        
        // Use the service
        var calculatedValue = await calculator.AddAsync(x, y);
        var expected = _options.Rate * (x + y);
        
        Assert.Equal(expected, calculatedValue);
    }

    [Theory]
    [InlineData(1, 2)]
    public async Task TestScopedServiceAsync(int x, int y)
    {
        // Get scoped service from fixture
        var calculator = _fixture.GetScopedService<ICalculator>(_testOutputHelper)!;
        
        var calculatedValue = await calculator.AddAsync(x, y);
        var expected = _options.Rate * (x + y);
        
        Assert.Equal(expected, calculatedValue);
    }
}
```

### Available Methods in Traditional Approach

- `_fixture.GetService<T>(_testOutputHelper)` - Gets a service instance
- `_fixture.GetScopedService<T>(_testOutputHelper)` - Gets a scoped service instance
- `_fixture.GetKeyedService<T>("key", _testOutputHelper)` - Gets a keyed service instance
- `_fixture.GetAsyncScope(_testOutputHelper)` - Gets an async service scope

## Property Injection (Recommended)

**New in version 9.2.0+**: Clean, declarative syntax using property injection with the `[Inject]` attribute:

```csharp
using Microsoft.Extensions.Options;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Microsoft.DependencyInjection.Attributes;

/// <summary>
/// Example tests demonstrating property injection using the new TestBedWithDI base class
/// </summary>
public class PropertyInjectionTests : TestBedWithDI<TestProjectFixture>
{
    // Regular service injection
    [Inject]
    public ICalculator? Calculator { get; set; }

    [Inject]
    public IOptions<Options>? Options { get; set; }

    // Keyed service injection
    [Inject("Porsche")]
    internal ICarMaker? PorscheCarMaker { get; set; }

    [Inject("Toyota")]
    internal ICarMaker? ToyotaCarMaker { get; set; }

    public PropertyInjectionTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture)
        : base(testOutputHelper, fixture)
    {
        // Dependencies are automatically injected after construction
    }

    [Fact]
    public async Task TestCalculatorThroughPropertyInjection()
    {
        // Arrange - dependencies are already injected via properties
        Assert.NotNull(Calculator);
        Assert.NotNull(Options);

        // Act
        var result = await Calculator.AddAsync(5, 3);

        // Assert
        var expected = Options.Value.Rate * (5 + 3);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestKeyedServicesThroughPropertyInjection()
    {
        // Arrange - keyed services are already injected via properties
        Assert.NotNull(PorscheCarMaker);
        Assert.NotNull(ToyotaCarMaker);

        // Assert
        Assert.Equal("Porsche", PorscheCarMaker.Manufacturer);
        Assert.Equal("Toyota", ToyotaCarMaker.Manufacturer);
    }

    [Theory]
    [InlineData(10, 20)]
    public async Task TestConvenienceMethodsStillWork(int x, int y)
    {
        // Demonstrate that convenience methods from the base class still work
        var calculator = GetService<ICalculator>();
        var options = GetService<IOptions<Options>>();
        var porsche = GetKeyedService<ICarMaker>("Porsche");

        Assert.NotNull(calculator);
        Assert.NotNull(options);
        Assert.NotNull(porsche);

        var result = await calculator.AddAsync(x, y);
        var expected = options.Value.Rate * (x + y);
        Assert.Equal(expected, result);
    }
}
```

### Benefits of Property Injection

- ✅ **Clean, declarative syntax** - Use `[Inject]` attribute on properties
- ✅ **No manual fixture calls** - Dependencies available immediately in test methods  
- ✅ **Full keyed services support** - Both regular and keyed services work seamlessly
- ✅ **Backward compatible** - All existing `TestBed<TFixture>` code continues to work unchanged
- ✅ **Gradual migration** - Adopt new approach incrementally without breaking existing tests

### Available Methods in Property Injection Approach

- `GetService<T>()` - Gets a service instance (no `_testOutputHelper` parameter needed)
- `GetScopedService<T>()` - Gets a scoped service instance
- `GetKeyedService<T>("key")` - Gets a keyed service instance

## Keyed Services

Keyed services are a .NET 9.0 feature that allows you to register multiple implementations of the same interface with different keys:

### Traditional Approach with Keyed Services

```csharp
public class KeyedServicesTests : TestBed<TestProjectFixture>
{
    public KeyedServicesTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture) 
        : base(testOutputHelper, fixture)
    {
    }

    [Theory]
    [InlineData("Porsche")]
    [InlineData("Toyota")]
    public void GetKeyedService(string key)
    {
        var carMaker = _fixture.GetKeyedService<ICarMaker>(key, _testOutputHelper)!;
        Assert.Equal(key, carMaker.Manufacturer);
    }
}
```

### Property Injection with Keyed Services

```csharp
public class PropertyInjectionTests : TestBedWithDI<TestProjectFixture>
{
    [Inject("Porsche")]
    internal ICarMaker? PorscheCarMaker { get; set; }

    [Inject("Toyota")]
    internal ICarMaker? ToyotaCarMaker { get; set; }

    // ... constructor and other code ...

    [Fact]
    public void TestKeyedServices()
    {
        Assert.NotNull(PorscheCarMaker);
        Assert.NotNull(ToyotaCarMaker);
        Assert.Equal("Porsche", PorscheCarMaker.Manufacturer);
        Assert.Equal("Toyota", ToyotaCarMaker.Manufacturer);
    }
}
```

### Keyed Service Registration

```csharp
protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
    => services
    .AddKeyedTransient<ICarMaker, Porsche>("Porsche")
    .AddKeyedTransient<ICarMaker, Toyota>("Toyota");

public interface ICarMaker
{
    string Manufacturer { get; }
}

public class Porsche : ICarMaker
{
    public string Manufacturer => "Porsche";
}

public class Toyota : ICarMaker
{
    public string Manufacturer => "Toyota";
}
```

## Factory Pattern (Experimental)

For true constructor injection into service classes, you can use the factory pattern:

### Factory Fixture Setup

```csharp
public class FactoryTestProjectFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
        .AddTransient<ICalculator, Calculator>()
        .AddKeyedTransient<ICarMaker, Porsche>("Porsche")
        .AddKeyedTransient<ICarMaker, Toyota>("Toyota")
        .Configure<Options>(config => configuration?.GetSection("Options").Bind(config));

    // Same implementation as TestProjectFixture for other methods...
}
```

### Factory Tests

```csharp
/// <summary>
/// Example tests demonstrating factory-based constructor injection
/// This approach allows for true constructor injection by creating instances via the fixture factory
/// </summary>
public class FactoryConstructorInjectionTests : TestBed<FactoryTestProjectFixture>
{
    public FactoryConstructorInjectionTests(ITestOutputHelper testOutputHelper, FactoryTestProjectFixture fixture)
        : base(testOutputHelper, fixture)
    {
    }

    [Fact]
    public async Task TestSimpleConstructorInjectionViaFactory()
    {
        // Arrange - Create instance with constructor injection via factory
        var simpleService = _fixture.CreateTestInstance<SimpleService>(_testOutputHelper);

        // Act
        var result = await simpleService.CalculateAsync(10, 5);
        var rate = simpleService.GetRate();

        // Assert
        var expected = rate * (10 + 5);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestFactoryWithAdditionalParameters()
    {
        // Create a custom test class that needs both DI services and custom parameters
        var testString = "test-data";
        var testInstance = _fixture.CreateTestInstance<CustomTestClass>(_testOutputHelper, testString);

        Assert.NotNull(testInstance.Calculator);
        Assert.Equal(testString, testInstance.CustomData);
    }
}

/// <summary>
/// Example class that demonstrates constructor injection with both DI services
/// and custom parameters
/// </summary>
public class CustomTestClass
{
    public ICalculator Calculator { get; }
    public string CustomData { get; }

    public CustomTestClass(ICalculator calculator, string customData)
    {
        Calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        CustomData = customData ?? throw new ArgumentNullException(nameof(customData));
    }
}
```

## Configuration and User Secrets

### Configuration Setup

The library supports configuration files and user secrets for sensitive data:

```csharp
protected override IEnumerable<TestAppSettings> GetTestAppSettings()
{
    yield return new() { Filename = "appsettings.json", IsOptional = false };
}

protected override void AddUserSecrets(IConfigurationBuilder configurationBuilder)
    => configurationBuilder.AddUserSecrets<TestProjectFixture>();
```

### Using Configuration in Tests

```csharp
public class UserSecretTests : TestBed<TestProjectFixture>
{
    public UserSecretTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture) 
        : base(testOutputHelper, fixture)
    {
    }

    [Fact]
    public void TestSecretValues()
    {
        /*
         * Create a user secret entry like the following payload in user secrets:
         * 
         * "SecretValues": {
         *   "Secret1": "secret1value",
         *   "Secret2": "secret2value"
         * }
         */
        var secretValues = _fixture.GetService<IOptions<SecretValues>>(_testOutputHelper)!.Value;
        Assert.NotEmpty(secretValues?.Secret1 ?? string.Empty);
        Assert.NotEmpty(secretValues?.Secret2 ?? string.Empty);
    }
}

public record SecretValues
{
    public string? Secret1 { get; set; }
    public string? Secret2 { get; set; }
}
```

### Setting Up User Secrets

1. Right-click your test project and select "Manage User Secrets"
2. Add your secret configuration:

```json
{
  "SecretValues": {
    "Secret1": "secret1value",
    "Secret2": "secret2value"
  }
}
```

## Advanced Dependency Injection Patterns

### IOptions<T> Pattern

```csharp
public class AdvancedDependencyInjectionTests : TestBedWithDI<TestProjectFixture>
{
    [Inject]
    public IOptions<Options>? Options { get; set; }

    [Fact]
    public void TestOptionsPattern()
    {
        Assert.NotNull(Options);
        Assert.True(Options.Value.Rate > 0);
    }
}
```

### Func<T> Factory Pattern

Register and use service factories:

```csharp
// In fixture
protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
    => services
    .AddTransient<ICalculator, Calculator>()
    .AddTransient<Func<ICalculator>>(provider => () => provider.GetService<ICalculator>()!);

// In tests
[Inject]
public Func<ICalculator>? CalculatorFactory { get; set; }

[Fact]
public async Task TestFactoryPattern()
{
    Assert.NotNull(CalculatorFactory);
    
    var calculator = CalculatorFactory();
    var result = await calculator.AddAsync(1, 2);
    
    Assert.True(result > 0);
}
```

### Action<T> Pattern

```csharp
[Fact]
public void TestActionTPatternWithServices()
{
    var calculatorResults = new List<int>();
    
    Action<ICalculator> calculatorAction = async calc =>
    {
        var result = await calc.AddAsync(10, 5);
        calculatorResults.Add(result);
    };

    // Use the action with injected calculator
    calculatorAction(Calculator!);
    
    Assert.Single(calculatorResults);
    Assert.True(calculatorResults[0] > 0);
}
```

## Service Lifetimes

### Transient Services

New instance for each injection:

```csharp
public class TransientServiceTests : TestBed<TestProjectFixture>
{
    [Fact]
    public void TestTransientServicesAreDifferentInstances()
    {
        var service1 = _fixture.GetService<ITransientService>(_testOutputHelper)!;
        var service2 = _fixture.GetService<ITransientService>(_testOutputHelper)!;

        Assert.NotEqual(service1.InstanceId, service2.InstanceId);
    }
}
```

### Scoped Services

Same instance within a scope (test):

```csharp
public class ScopedServiceTests : TestBed<TestProjectFixture>
{
    [Fact]
    public void TestScopedServicesAreSameInstanceWithinScope()
    {
        var service1 = _fixture.GetScopedService<IScopedService>(_testOutputHelper)!;
        var service2 = _fixture.GetScopedService<IScopedService>(_testOutputHelper)!;

        Assert.Equal(service1.InstanceId, service2.InstanceId);
    }
}
```

### Singleton Services

Same instance across entire application lifetime:

```csharp
public class SingletonServiceTests : TestBed<TestProjectFixture>
{
    [Fact]
    public void TestSingletonServicesAreSameInstance()
    {
        var service1 = _fixture.GetService<ISingletonService>(_testOutputHelper)!;
        var service2 = _fixture.GetService<ISingletonService>(_testOutputHelper)!;

        Assert.Equal(service1.InstanceId, service2.InstanceId);
    }
}
```

## Test Ordering

The library provides a bonus feature for running tests in order:

```csharp
[TestCaseOrderer("Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer", "Xunit.Microsoft.DependencyInjection")]
public class UnitTests : TestBed<TestProjectFixture>
{
    public UnitTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture) 
        : base(testOutputHelper, fixture)
    {
    }

    [Fact, TestOrder(1)]
    public async Task Test1()
    {
        var calculator = _fixture.GetService<ICalculator>(_testOutputHelper)!;
        var result = await calculator.AddAsync(1, 2);
        Assert.True(result > 0);
    }

    [Fact, TestOrder(2)]
    public async Task Test2()
    {
        var calculator = _fixture.GetService<ICalculator>(_testOutputHelper)!;
        var result = await calculator.AddAsync(3, 4);
        Assert.True(result > 0);
    }

    [Theory, TestOrder(3)]
    [InlineData(5, 6)]
    public async Task Test3(int x, int y)
    {
        var calculator = _fixture.GetService<ICalculator>(_testOutputHelper)!;
        var result = await calculator.AddAsync(x, y);
        Assert.True(result > 0);
    }
}
```

## Best Practices

1. **Use Property Injection for new projects** - It provides the cleanest syntax
2. **Gradual Migration** - You can mix both approaches in the same test suite
3. **Keyed Services** - Use for multiple implementations of the same interface
4. **Configuration** - Store non-sensitive data in `appsettings.json`, sensitive data in user secrets
5. **Service Lifetimes** - Choose appropriate lifetimes based on your testing needs
6. **Factory Pattern** - Use for true constructor injection when needed
7. **Test Ordering** - Use sparingly, only when tests have dependencies on each other

## Migration Guide

### From Traditional to Property Injection

**Before:**
```csharp
public class MyTests : TestBed<TestProjectFixture>
{
    [Fact]
    public async Task TestCalculation()
    {
        var calculator = _fixture.GetService<ICalculator>(_testOutputHelper);
        var result = await calculator.AddAsync(1, 2);
        Assert.Equal(3, result);
    }
}
```

**After:**
```csharp
public class MyTests : TestBedWithDI<TestProjectFixture>
{
    [Inject] private ICalculator Calculator { get; set; } = null!;
    
    [Fact]
    public async Task TestCalculation()
    {
        var result = await Calculator.AddAsync(1, 2);
        Assert.Equal(3, result);
    }
}
```

This concludes the comprehensive examples for the Xunit.Microsoft.DependencyInjection library. For more details, visit the [repository examples](https://github.com/Umplify/xunit-dependency-injection/tree/main/examples/Xunit.Microsoft.DependencyInjection.ExampleTests).