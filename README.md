[![Build Status](https://dev.azure.com/umplify/Grain/_apis/build/status/Xunit/xunit-dependency-injection-PR?branchName=refs%2Fpull%2F94%2Fmerge)](https://dev.azure.com/umplify/Grain/_build/latest?definitionId=18&branchName=refs%2Fpull%2F94%2Fmerge)
![Nuget](https://img.shields.io/nuget/v/Xunit.Microsoft.DependencyInjection)
![Nuget](https://img.shields.io/nuget/dt/Xunit.Microsoft.DependencyInjection)

# Xunit Dependency Injection framework - .NET 9.0

Xunit does not support any built-in dependency injection features, therefore developers have to come up with a solution to recruit their favourite dependency injection framework in their tests.

This library brings Microsoft's dependency injection container to Xunit by leveraging Xunit's fixture pattern and now supports **two approaches** for dependency injection:

1. **Traditional Fixture-Based Approach** - Access services via `_fixture.GetService<T>(_testOutputHelper)` (fully supported, backward compatible)
2. **Constructor Dependency Injection** - Inject services directly into test class properties using the `[Inject]` attribute (new feature)

## Important: xUnit versions

* For **xUnit** packages use Xunit.Microsoft.DependencyInjection versions **up to** 9.0.5
* For **xUnit.v3** packages use Xunit.Microsoft.DependencyInjection versions **from** 9.1.0

Also please check the [migration guide](https://xunit.net/docs/getting-started/v3/migration) from xUnit for test authors.

### Example on how to reference xunit.v3

```xml
<PackageReference Include="xunit.v3" Version="2.0.3" />
```

## Getting started

### Nuget package

First add the following [nuget package](https://www.nuget.org/packages/Xunit.Microsoft.DependencyInjection/) to your Xunit project:

```ps
Install-Package Xunit.Microsoft.DependencyInjection
```

### Setup your fixture

The abstract class of `Xunit.Microsoft.DependencyInjection.Abstracts.TestBedFixture` contains the necessary functionalities to add services and configurations to Microsoft's dependency injection container. Your concrete test fixture class must derive from this abstract class and implement the following two abstract methods:

```csharp
protected abstract void AddServices(IServiceCollection services, IConfiguration? configuration);
protected abstract IEnumerable<TestAppSettings> GetTestAppSettings();
protected abstract ValueTask DisposeAsyncCore();
```

`GetConfigurationFiles(...)` method returns a collection of the configuration files in your Xunit test project to the framework. `AddServices(...)` method must be used to wire up the implemented services.

#### Secret manager

[Secret manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows#how-the-secret-manager-tool-works) is a great tool to store credentials, API keys, and other secret information for development purposes. This library has started supporting user secrets from version 8.2.0 onwards. To utilize user secrets in your tests, simply override the `virtual` method below from the `TestBedFixture` class:

```csharp
protected override void AddUserSecrets(IConfigurationBuilder configurationBuilder); 
```

### Access the wired up services

There are two method that you can use to access the wired up service depending on your context:

```csharp
public T GetScopedService<T>(ITestOutputHelper testOutputHelper);
public T GetService<T>(ITestOutputHelper testOutputHelper);
```

To access async scopes simply call the following method in the abstract fixture class:

```csharp
public AsyncServiceScope GetAsyncScope(ITestOutputHelper testOutputHelper);
```

### Accessing the keyed wired up services in .NET 9.0

You can call the following method to access the keyed already-wired up services:

```csharp
T? GetKeyedService<T>([DisallowNull] string key, ITestOutputHelper testOutputHelper);
```

## Constructor Dependency Injection

**New in this version**: The library now supports constructor-style dependency injection while maintaining full backward compatibility with the existing fixture-based approach.

### Property Injection with TestBedWithDI (Recommended)

For cleaner test code, inherit from `TestBedWithDI<TFixture>` instead of `TestBed<TFixture>` and use the `[Inject]` attribute:

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
        // Dependencies are automatically injected after construction
    }

    [Fact]
    public async Task TestWithCleanSyntax()
    {
        // Dependencies are immediately available - no fixture calls needed
        Assert.NotNull(Calculator);
        var result = await Calculator.AddAsync(5, 3);
        Assert.True(result > 0);
    }
}
```

### Keyed Services with Property Injection

Use the `[Inject("key")]` attribute for keyed services:

```csharp
public class PropertyInjectionTests : TestBedWithDI<TestProjectFixture>
{
    [Inject("Porsche")]
    internal ICarMaker? PorscheCarMaker { get; set; }

    [Inject("Toyota")]
    internal ICarMaker? ToyotaCarMaker { get; set; }

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

### Convenience Methods

The `TestBedWithDI` class provides convenience methods that don't require the `_testOutputHelper` parameter:

```csharp
protected T? GetService<T>()
protected T? GetScopedService<T>()
protected T? GetKeyedService<T>(string key)
```

### Benefits of Constructor Dependency Injection

- ✅ **Clean, declarative syntax** - Use `[Inject]` attribute on properties
- ✅ **No manual fixture calls** - Dependencies available immediately in test methods  
- ✅ **Full keyed services support** - Both regular and keyed services work seamlessly
- ✅ **Backward compatible** - All existing `TestBed<TFixture>` code continues to work unchanged
- ✅ **Gradual migration** - Adopt new approach incrementally without breaking existing tests

### Migration Guide

You can migrate existing tests gradually:

1. **Keep existing approach** - Continue using `TestBed<TFixture>` with fixture methods
2. **Hybrid approach** - Change to `TestBedWithDI<TFixture>` and use both `[Inject]` properties and fixture methods
3. **Full migration** - Use property injection for all dependencies for cleanest code

### Factory Pattern (Experimental)

For true constructor injection into service classes, see [CONSTRUCTOR_INJECTION.md](CONSTRUCTOR_INJECTION.md) for the factory-based approach.

### Adding custom logging provider

Test developers can add their own desired logger provider by overriding ```AddLoggingProvider(...)``` virtual method defined in ```TestBedFixture``` class.

### Preparing Xunit test classes

Your Xunit test class must be derived from ```Xunit.Microsoft.DependencyInjection.Abstracts.TestBed<T>``` class where ```T``` should be your fixture class derived from ```TestBedFixture```.

Also, the test class should be decorated by the following attribute:

```csharp
[CollectionDefinition("Dependency Injection")]
```

#### Clearing managed resources

To have managed resources cleaned up, simply override the virtual method of `Clear()`. This is an optional step.

#### Clearing managed resourced asynchronously

Simply override the virtual method of `DisposeAsyncCore()` for this purpose. This is also an optional step.

## Running tests in order

The library also has a bonus feature that simplifies running tests in order. The test class does not have to be derived from ```TestBed<T>``` class though and it can apply to all Xunit classes.

Decorate your Xunit test class with the following attribute and associate ```TestOrder(...)``` with ```Fact``` and ```Theory```:

```csharp
[TestCaseOrderer("Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer", "Xunit.Microsoft.DependencyInjection")]
```

## Supporting configuration from `UserSecrets`

This library's `TestBedFixture` abstract class exposes an instance of `IConfigurationBuilder` that can be used to support `UserSecrets` when configuring the test projects:

```csharp
public IConfigurationBuilder ConfigurationBuilder { get; private set; }
```

## Examples

* Please [follow this link](https://github.com/Umplify/xunit-dependency-injection/tree/main/examples/Xunit.Microsoft.DependencyInjection.ExampleTests) to view examples utilizing both the traditional fixture-based approach and the new constructor dependency injection features.
* **Traditional approach**: See examples using `TestBed<TFixture>` and `_fixture.GetService<T>(_testOutputHelper)`  
* **Constructor injection**: See `PropertyInjectionTests.cs` for examples using `TestBedWithDI<TFixture>` with `[Inject]` attributes
* **Factory pattern**: See `FactoryConstructorInjectionTests.cs` for experimental constructor injection scenarios
* [Digital Silo](https://digitalsilo.io/)'s unit tests and integration tests are using this library.

### One more thing

Do not forget to include the following nuget packages to your Xunit project:

* Microsoft.Extensions.DependencyInjection
* Microsoft.Extensions.Configuration
* Microsoft.Extensions.Options
* Microsoft.Extensions.Configuration.Binder
* Microsoft.Extensions.Configuration.FileExtensions
* Microsoft.Extensions.Configuration.Json
* Microsoft.Extensions.Logging
* Microsoft.Extensions.Configuration.EnvironmentVariables
