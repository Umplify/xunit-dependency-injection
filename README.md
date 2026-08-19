[![Build Status](https://dev.azure.com/umplify/Grain/_apis/build/status/Xunit/xunit-dependency-injection-PR?branchName=refs%2Fpull%2F94%2Fmerge)](https://dev.azure.com/umplify/Grain/_build/latest?definitionId=18&branchName=refs%2Fpull%2F94%2Fmerge)
![Nuget](https://img.shields.io/nuget/v/Xunit.Microsoft.DependencyInjection)
![Nuget](https://img.shields.io/nuget/dt/Xunit.Microsoft.DependencyInjection)

# Xunit Dependency Injection framework - .NET 10.0

Xunit does not support any built-in dependency injection features, therefore developers have to come up with a solution to recruit their favourite dependency injection framework in their tests.

This library brings **Microsoft's dependency injection container** to Xunit by leveraging Xunit's fixture pattern and provides **three approaches** for dependency injection in your tests:

1. **🆕 Property Injection (Recommended)** - Clean, declarative syntax using `[Inject]` attributes on properties
2. **🔧 Traditional Fixture-Based** - Access services via `_fixture.GetService<T>(_testOutputHelper)` (fully backward compatible)
3. **⚡ Factory Pattern** - True constructor injection into service classes (experimental)

## ✨ Key Features

- 🎯 **Multiple injection patterns** - Choose the approach that fits your team's style
- 🔑 **Keyed services support** - Full .NET 10.0 keyed services integration
- ⚙️ **Configuration integration** - Support for `appsettings.json`, user secrets, and environment variables
- 🧪 **Service lifetime management** - Transient, Scoped, and Singleton services work as expected
- ♻️ **Async disposal support** - Container-managed `IAsyncDisposable` services are disposed asynchronously during fixture teardown
- 📦 **Microsoft.Extensions ecosystem** - Built on the same DI container used by ASP.NET Core
- 🔓 **Parallel-safe fixtures** - A shared `TestBedFixture` builds exactly one container even under xUnit.net v4's `ParallelMode.All`
- 🪢 **xUnit.net v4 lifecycle hooks** - Fixtures can implement `INotifyTestClassLifecycleAsync` and friends for per-class setup
- 🔄 **Gradual migration** - Adopt new features incrementally without breaking existing tests
- 🏗️ **Production-ready** - Used by [Digital Silo](https://digitalsilo.io/) and other production applications

## Important: xUnit versions

* For **xUnit** packages use Xunit.Microsoft.DependencyInjection versions **up to** 9.0.5
* For **xUnit.v3 3.x** packages use Xunit.Microsoft.DependencyInjection versions **9.1.0 – 10.0.5**
* For **xUnit.v3 4.x** packages use Xunit.Microsoft.DependencyInjection version **10.1.0 or later**

Also please check the [migration guide](https://xunit.net/docs/getting-started/v3/migration) from xUnit for test authors.

### Example on how to reference xunit.v3

```xml
<PackageReference Include="xunit.v3" Version="4.0.0" />
```

> ⚠️ **xUnit.net v4 requires Microsoft Testing Platform.** `dotnet test` no longer runs v4 test
> projects through VSTest on the .NET 10 SDK. See [Running your tests on xUnit.net v4](#running-your-tests-on-xunitnet-v4)
> for the one-time `global.json` change you need.

## Getting started

### Prerequisites

Before you begin, ensure you have:
- **.NET 10.0 SDK** installed on your development machine
- **Visual Studio 2022** or **Visual Studio Code** with C# extension
- Basic understanding of dependency injection concepts
- Familiarity with xUnit testing framework

### Nuget package

First add the following [nuget package](https://www.nuget.org/packages/Xunit.Microsoft.DependencyInjection/) to your Xunit test project:

#### Package Manager Console
```ps
Install-Package Xunit.Microsoft.DependencyInjection
```

#### .NET CLI
```bash
dotnet add package Xunit.Microsoft.DependencyInjection
```

#### PackageReference (in your .csproj file)
```xml
<PackageReference Include="Xunit.Microsoft.DependencyInjection" Version="10.1.0" />
```

**✨ That's it!** All required Microsoft.Extensions dependencies are now automatically included with the package, so you don't need to manually add them to your test project.

### Quick Start Example

Here's a minimal example to get you started quickly:

#### 1. Create a Test Fixture

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

public class MyTestFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
            .AddTransient<IMyService, MyService>()
            .AddScoped<IMyScopedService, MyScopedService>();

    protected override ValueTask DisposeAsyncCore() => new();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = true };
    }
}
```

#### 2. Create Your Test Class (Property Injection - Recommended)

```csharp
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Microsoft.DependencyInjection.Attributes;

[Collection("Dependency Injection")]
public class MyTests : TestBedWithDI<MyTestFixture>
{
    [Inject] private IMyService MyService { get; set; } = null!;
    [Inject] private IMyScopedService MyScopedService { get; set; } = null!;

    public MyTests(ITestOutputHelper testOutputHelper, MyTestFixture fixture)
        : base(testOutputHelper, fixture) { }

    [Fact]
    public async Task TestMyService()
    {
        // Your services are automatically injected and ready to use
        var result = await MyService.DoSomethingAsync();
        Assert.NotNull(result);
    }
}
```

#### 3. Alternative: Traditional Fixture Approach

```csharp
[CollectionDefinition("Dependency Injection")]
public class MyTraditionalTests : TestBed<MyTestFixture>
{
    public MyTraditionalTests(ITestOutputHelper testOutputHelper, MyTestFixture fixture)
        : base(testOutputHelper, fixture) { }

    [Fact]
    public async Task TestMyService()
    {
        // Get services from the fixture
        var myService = _fixture.GetService<IMyService>(_testOutputHelper)!;
        var result = await myService.DoSomethingAsync();
        Assert.NotNull(result);
    }
}
```

### Setup your fixture

The abstract class of `Xunit.Microsoft.DependencyInjection.Abstracts.TestBedFixture` contains the necessary functionalities to add services and configurations to Microsoft's dependency injection container. Your concrete test fixture class must derive from this abstract class and implement the following abstract methods:

```csharp
protected abstract void AddServices(IServiceCollection services, IConfiguration? configuration);
protected abstract IEnumerable<TestAppSettings> GetTestAppSettings();
protected abstract ValueTask DisposeAsyncCore();
```

Use `DisposeAsyncCore()` to clean up fixture-owned resources (for example, files, sockets, or external clients created by the fixture). Service cleanup for dependencies resolved from the DI container is handled by the framework during async teardown.

`TestBedFixture` now ignores any `TestAppSettings` entries whose `Filename` is null or empty before calling `AddJsonFile`. That means you can safely return placeholder descriptors or rely only on environment variables; optional JSON files can simply leave `Filename` blank and the framework skips them automatically when building the configuration root.
 
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

### Accessing the keyed wired up services in .NET 10.0

You can call the following method to access the keyed already-wired up services:

```csharp
T? GetKeyedService<T>([DisallowNull] string key, ITestOutputHelper testOutputHelper);
```

## Constructor Dependency Injection

**Available from version 9.2.0 onward**: The library supports constructor-style dependency injection while maintaining full backward compatibility with the existing fixture-based approach.

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

#### Clearing managed resources asynchronously

`TestBedFixture` performs async teardown and disposes the DI `ServiceProvider` asynchronously. This ensures container-managed services implementing `IAsyncDisposable` are disposed correctly during fixture teardown.

If you need additional async cleanup for fixture-owned resources, override `DisposeAsyncCore()`:

```csharp
public sealed class MyTestFixture : TestBedFixture
{
    protected override ValueTask DisposeAsyncCore()
    {
        // Cleanup resources created/owned by the fixture itself.
        return ValueTask.CompletedTask;
    }
}
```

For a full working example, see `AsyncDisposableTests` and `AsyncDisposableFixture` in the examples project.

## Running your tests on xUnit.net v4

Version 10.1.0 of this library builds against **xunit.v3 4.0.0**. Most of your test code carries over
unchanged, but the runner and the parallelization model both moved, so read this section before you upgrade.

### Microsoft Testing Platform is now required

xunit.v3 4.x runs on **Microsoft Testing Platform (MTP) v2**, and the .NET 10 SDK no longer bridges MTP
test projects through VSTest. Without any change, `dotnet test` fails during the build:

```text
error : Testing with VSTest target is no longer supported by Microsoft.Testing.Platform on .NET 10 SDK
and later. If you use dotnet test, you should opt-in to the new dotnet test experience.
```

Opt in once, per repository, by adding a `test` section to `global.json` next to your solution:

```json
{
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

That is the only change most projects need. Two follow-ups apply if you script `dotnet test` in CI, because
MTP rejects VSTest-only switches:

| VSTest (xUnit.net v3 3.x)                  | Microsoft Testing Platform (xUnit.net v4)                             |
| ------------------------------------------ | --------------------------------------------------------------------- |
| `--logger trx`                              | `--report-trx` (reference `Microsoft.Testing.Extensions.TrxReport`)     |
| `--collect "XPlat Code Coverage"` (coverlet) | `--coverage` (reference `Microsoft.Testing.Extensions.CodeCoverage`)    |
| `dotnet test MyTests.csproj`                | `dotnet test --project MyTests.csproj`                                  |

The examples project in this repository shows the resulting package set, and `azure-pipelines.yml` shows the
matching CI configuration.

### Full test parallelization

The headline v4 feature is the ability to run **every** test in an assembly concurrently, instead of only
parallelizing across test collections.

| `ParallelMode` | Behaviour                                                                       |
| -------------- | ------------------------------------------------------------------------------- |
| `None`         | Every test runs sequentially.                                                     |
| `Collections`  | Tests in different collections run concurrently; tests inside one do not. **Default.** |
| `All`          | Every test runs concurrently, regardless of collection or shared fixture.         |

The default is unchanged, so upgrading does not alter how your tests are scheduled. To opt in, either set it
in `testconfig.json` at the root of your test project:

```json
{
  "xUnit": {
    "parallelMode": "all"
  }
}
```

...or declare it in code:

```csharp
using Xunit.Sdk;
using Xunit.v3;

[assembly: Parallelization(Mode = ParallelMode.All)]
```

#### What this library guarantees under `ParallelMode.All`

`TestBedFixture` builds its container lazily on first use. As of 10.1.0 that initialization is **thread-safe**:
however many tests reach the fixture at once, exactly one `ServiceProvider` is built and every caller gets the
same instance. See `ParallelFixtureAccessTests` in the examples project for the regression tests covering it.

#### What still needs your attention

`[Inject]` properties and `GetService<T>()` resolve from the fixture's **root** container, so a registered
instance is shared by every test that shares the fixture. Under `Collections` those tests run one at a time and
never observe each other. Under `All` they run simultaneously, and any service that carries mutable state —
counters, caches, collected output — will interleave across tests.

If you want per-test state, resolve through a scope instead, which hands out a fresh instance per call:

```csharp
var service = GetScopedService<IMyScopedService>();   // or _fixture.GetAsyncScope(_testOutputHelper)
```

Otherwise, opt the affected scope out of parallelization. Once parallelization is disabled at one layer it
cannot be re-enabled below it:

```csharp
[CollectionDefinition("Dependency Injection", DisableParallelization = true)]  // whole collection
public class DependencyInjectionCollection { }

[TestClass(DisableParallelization = true)]                                     // one class
public class MyTests : TestBedWithDI<MyTestFixture> { }

[Fact(DisableParallelization = true)]                                          // one test
public void MyTest() { }
```

### Fixtures can hook the test lifecycle

v4 lets a fixture observe the assembly, collection, class, method and test lifecycle directly, which is a
natural fit for a `TestBedFixture` that needs per-class setup beyond its DI registrations:

```csharp
using Xunit.v3;

public class MyFixture : TestBedFixture, INotifyTestClassLifecycleAsync
{
    public ValueTask OnTestClassStartingAsync(IXunitTestClass testClass) => /* per-class setup */ default;

    public ValueTask OnTestClassFinishedAsync(IXunitTestClass testClass) => /* per-class teardown */ default;

    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
        => services.AddSingleton<IMyService, MyService>();

    protected override ValueTask DisposeAsyncCore() => new();
}
```

Synchronous counterparts (`INotifyTestClassLifecycle`) and equivalents for the other levels
(`INotifyTestAssemblyLifecycle`, `INotifyTestCollectionLifecycle`, `INotifyTestMethodLifecycle`,
`INotifyTestLifecycle`, `INotifyTestCaseLifecycle`, plus `...Async` variants) are available too. A working
example lives in `Fixtures/LifecycleAwareFixture.cs` and `LifecycleNotificationTests.cs`.

### Other v4 additions worth knowing

* **Test class and method orderers.** `ITestClassOrderer` and `ITestMethodOrderer` join the existing collection
  and case orderers. Ordering is applied collection → class → method → case.
* **Generic attributes.** `[TestCaseOrderer<TOrderer>]`, `[TestClassOrderer<TOrderer>]` and friends replace the
  `typeof(...)` form with a compile-time checked one.
* **Assertion improvements.** `Assert.All` / `Assert.AllAsync` take a `throwIfEmpty` argument so an empty
  collection can be treated as a failure, and `Assert.OverrideMaxStringLength`,
  `Assert.OverrideMaxEnumerableLength`, `Assert.OverrideMaxObjectDepth` and
  `Assert.OverrideMaxObjectMemberCount` let a single test widen the truncation limits in failure messages.
* **`removeAsyncSuffix`.** A `methodDisplayOptions` value that strips the `Async` suffix from test names.
* **Native AOT.** Test projects can now be published ahead-of-time compiled.
* **Retired platforms.** MTP v1 and Mono are no longer supported by xUnit.net.

Full details are in the [xUnit.net v3 4.0.0 release notes](https://xunit.net/releases/v3/4.0.0).

## Running tests in order

The library also has a bonus feature that simplifies running tests in order. The test class does not have to be derived from ```TestBed<T>``` class though and it can apply to all Xunit classes.

Decorate your Xunit test class with the following attribute and associate ```TestOrder(...)``` with ```Fact``` and ```Theory```:

```csharp
[TestCaseOrderer(typeof(TestPriorityOrderer))]
public class MyOrderedTests
{
    [Fact, TestOrder(1)]
    public void RunsFirst() { }

    [Fact, TestOrder(2)]
    public void RunsSecond() { }
}
```

On xUnit.net v4 you can use the generic form instead, which is checked at compile time:

```csharp
[TestCaseOrderer<TestPriorityOrderer>]
public class MyOrderedTests { }
```

> The string-based overload (`[TestCaseOrderer("Type.Full.Name", "AssemblyName")]`) was removed in
> xUnit.net v3 — replace it with `typeof(...)` or the generic attribute above.

Ordering only holds while the tests being ordered are not running concurrently. See
[Full test parallelization](#full-test-parallelization) below.

## Supporting configuration from `UserSecrets`

This library's `TestBedFixture` abstract class exposes an instance of `IConfigurationBuilder` that can be used to support `UserSecrets` when configuring the test projects:

```csharp
public IConfigurationBuilder ConfigurationBuilder { get; private set; }
```

## Examples

📖 **[Complete Examples Documentation](Examples.md)** - Comprehensive guide with working code examples

* **[Live Examples](https://github.com/Umplify/xunit-dependency-injection/tree/main/examples/Xunit.Microsoft.DependencyInjection.ExampleTests)** - View the complete working examples that demonstrate all features
* **Traditional approach**: See examples using `TestBed<TFixture>` and `_fixture.GetService<T>(_testOutputHelper)`  
* **Property injection**: See `PropertyInjectionTests.cs` for examples using `TestBedWithDI<TFixture>` with `[Inject]` attributes
* **Factory pattern**: See `FactoryConstructorInjectionTests.cs` for experimental constructor injection scenarios
* **Keyed services**: See `KeyedServicesTests.cs` for .NET 10.0 keyed service examples
* **Configuration**: See `UserSecretTests.cs` for configuration and user secrets integration
* **Async disposal**: See `AsyncDisposableTests.cs` and `Fixtures/AsyncDisposableFixture.cs` for async teardown of `IAsyncDisposable` services
* **Advanced patterns**: See `AdvancedDependencyInjectionTests.cs` for `IOptions<T>`, `Func<T>`, and `Action<T>` examples
* **xUnit.net v4 lifecycle hooks**: See `Fixtures/LifecycleAwareFixture.cs` and `LifecycleNotificationTests.cs` for a fixture that reacts to test class start and finish
* **Parallel-safe fixtures**: See `ParallelFixtureAccessTests.cs` for the concurrency guarantees of `TestBedFixture`

🏢 [Digital Silo](https://digitalsilo.io/)'s unit tests and integration tests are using this library in production.

### Troubleshooting Common Issues

#### Missing Dependencies
If you encounter build errors, ensure all required Microsoft.Extensions packages are installed with compatible versions.

#### Configuration File Issues
- Ensure `appsettings.json` is set to "Copy to Output Directory: Copy if newer" in file properties
- Configuration files must be valid JSON format

#### User Secrets Issues
- Initialize user secrets: `dotnet user-secrets init`
- Set secrets: `dotnet user-secrets set "SecretKey" "SecretValue"`

#### xUnit Version Compatibility
- For **xUnit** packages use Xunit.Microsoft.DependencyInjection versions **up to** 9.0.5
- For **xUnit.v3 3.x** packages use Xunit.Microsoft.DependencyInjection versions **9.1.0 - 10.0.5**
- For **xUnit.v3 4.x** packages use Xunit.Microsoft.DependencyInjection version **10.1.0 or later**

#### `dotnet test` fails with "Testing with VSTest target is no longer supported"
xUnit.net v4 runs on Microsoft Testing Platform, which the .NET 10 SDK will not launch through VSTest.
Add the `test` section to `global.json` as described in
[Running your tests on xUnit.net v4](#running-your-tests-on-xunitnet-v4).

#### Tests interfere with each other after enabling `ParallelMode.All`
`[Inject]` resolves services from the fixture's root container, so stateful services are shared by every
test using that fixture. Resolve through `GetScopedService<T>()` for per-test state, or disable
parallelization for the affected class with `[TestClass(DisableParallelization = true)]`. See
[Full test parallelization](#full-test-parallelization).

### Need Help?

- 📖 **[Complete Examples Documentation](Examples.md)** - Step-by-step examples for all features
- 🐛 **[GitHub Issues](https://github.com/Umplify/xunit-dependency-injection/issues)** - Report bugs or request features
- 📦 **[NuGet Package](https://www.nuget.org/packages/Xunit.Microsoft.DependencyInjection/)** - Latest releases and changelog
- 📋 **[Migration Guide](https://xunit.net/docs/getting-started/v3/migration)** - For xUnit.v3 migration
