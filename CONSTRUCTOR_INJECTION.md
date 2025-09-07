# Constructor Dependency Injection

This document describes the new constructor dependency injection capabilities added to the xUnit Dependency Injection framework while maintaining full backward compatibility with the existing fixture-based approach.

## Overview

The framework now supports two approaches for dependency injection:

1. **Traditional Fixture-Based Approach** (existing) - Access services via `_fixture.GetService<T>(_testOutputHelper)`
2. **Constructor Dependency Injection** (new) - Inject services directly into test class properties during construction

## Property Injection with TestBedWithDI

### Basic Usage

Inherit from `TestBedWithDI<TFixture>` instead of `TestBed<TFixture>` and use the `[Inject]` attribute on properties:

```csharp
public class PropertyInjectionTests : TestBedWithDI<TestProjectFixture>
{
    [Inject]
    public ICalculator? Calculator { get; set; }

    [Inject]
    public IOptions<Options>? Options { get; set; }

    public PropertyInjectionTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture)
        : base(testOutputHelper, fixture)
    {
        // Dependencies are automatically injected after base constructor completes
    }

    [Fact]
    public async Task TestCalculatorThroughPropertyInjection()
    {
        // Dependencies are already available - no need to call _fixture methods
        Assert.NotNull(Calculator);
        Assert.NotNull(Options);

        var result = await Calculator.AddAsync(5, 3);
        var expected = Options.Value.Rate * (5 + 3);
        Assert.Equal(expected, result);
    }
}
```

### Keyed Services

Use the `[Inject("key")]` attribute for keyed services:

```csharp
public class PropertyInjectionTests : TestBedWithDI<TestProjectFixture>
{
    [Inject("Porsche")]
    internal ICarMaker? PorscheCarMaker { get; set; }

    [Inject("Toyota")]
    internal ICarMaker? ToyotaCarMaker { get; set; }

    [Fact]
    public void TestKeyedServicesThroughPropertyInjection()
    {
        Assert.NotNull(PorscheCarMaker);
        Assert.NotNull(ToyotaCarMaker);
        Assert.Equal("Porsche", PorscheCarMaker.Manufacturer);
        Assert.Equal("Toyota", ToyotaCarMaker.Manufacturer);
    }
}
```

### Convenience Methods

The `TestBedWithDI` class provides convenience methods that don't require the `_testOutputHelper` parameter:

```csharp
protected T? GetService<T>()
protected T? GetScopedService<T>()
protected T? GetKeyedService<T>(string key)
```

```csharp
[Theory]
[InlineData(10, 20)]
public async Task TestConvenienceMethodsStillWork(int x, int y)
{
    // These methods are available without needing _fixture
    var calculator = GetService<ICalculator>();
    var options = GetService<IOptions<Options>>();
    var porsche = GetKeyedService<ICarMaker>("Porsche");

    Assert.NotNull(calculator);
    Assert.NotNull(options);
    Assert.NotNull(porsche);
}
```

## Factory-Based Constructor Injection (Experimental)

For true constructor injection, use `TestBedFactoryFixture` with the factory pattern:

### Setup

```csharp
public class FactoryTestProjectFixture : TestBedFactoryFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
        .AddTransient<ICalculator, Calculator>()
        .AddKeyedTransient<ICarMaker, Porsche>("Porsche")
        .AddKeyedTransient<ICarMaker, Toyota>("Toyota")
        .AddTransient<SimpleService>(); // Register classes that need constructor injection
}
```

### Usage

```csharp
public class FactoryConstructorInjectionTests : TestBed<FactoryTestProjectFixture>
{
    [Fact]
    public async Task TestConstructorInjectionViaFactory()
    {
        // Create instances with constructor injection
        var simpleService = _fixture.CreateTestInstance<SimpleService>(_testOutputHelper);
        
        var result = await simpleService.CalculateAsync(10, 5);
        Assert.True(result > 0);
    }
}
```

### Service Class with Constructor Injection

```csharp
public class SimpleService
{
    private readonly ICalculator _calculator;
    private readonly Options _options;

    public SimpleService(ICalculator calculator, IOptions<Options> options)
    {
        _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<int> CalculateAsync(int x, int y)
    {
        return await _calculator.AddAsync(x, y);
    }
}
```

### Keyed Services in Factory Pattern

Use the `[FromKeyedService("key")]` attribute for keyed service constructor parameters:

```csharp
public class CalculatorService
{
    public CalculatorService(
        ICalculator calculator,
        IOptions<Options> options,
        [FromKeyedService("Porsche")] ICarMaker porsche,
        [FromKeyedService("Toyota")] ICarMaker toyota)
    {
        // Constructor injection with keyed services
    }
}
```

## Backward Compatibility

All existing code continues to work unchanged. The new approaches are additive:

- `TestBed<TFixture>` continues to work as before
- `_fixture.GetService<T>(_testOutputHelper)` methods work as before
- Existing test classes require no changes

## Migration Path

You can migrate existing tests gradually:

1. **Option 1**: Keep using `TestBed<TFixture>` with existing fixture methods
2. **Option 2**: Change to `TestBedWithDI<TFixture>` and use `[Inject]` properties for new dependencies while keeping existing fixture method calls
3. **Option 3**: Fully migrate to property injection for cleaner test code

## Benefits

### Property Injection Approach
- ✅ Clean, declarative syntax
- ✅ No need to pass `_testOutputHelper` around
- ✅ Dependencies available immediately in test methods
- ✅ Full support for regular and keyed services
- ✅ Maintains all existing fixture capabilities
- ✅ Works perfectly with xUnit lifecycle

### Factory Approach
- ✅ True constructor injection for service classes
- ✅ Works for regular services and additional parameters
- ⚠️ Keyed services support is experimental
- ⚠️ More complex setup required

## Recommendation

Use the **Property Injection with TestBedWithDI** approach for most scenarios as it provides the cleanest developer experience while maintaining full compatibility with the existing framework.