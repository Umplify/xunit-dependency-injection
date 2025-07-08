[![Build Status](https://dev.azure.com/umplify/Grain/_apis/build/status/Xunit/xunit-dependency-injection-PR?branchName=refs%2Fpull%2F94%2Fmerge)](https://dev.azure.com/umplify/Grain/_build/latest?definitionId=18&branchName=refs%2Fpull%2F94%2Fmerge)
![Nuget](https://img.shields.io/nuget/v/Xunit.Microsoft.DependencyInjection)
![Nuget](https://img.shields.io/nuget/dt/Xunit.Microsoft.DependencyInjection)

# Xunit Dependency Injection framework - .NET 9.0

Xunit does not support any built-in dependency injection features, therefore developers have to come up with a solution to recruit their favourite dependency injection framework in their tests.

This library brings in Microsoft's dependency injection container to Xunit by leveraging Xunit's fixture.

## Important: xUnit versions

For xUnit packages use Xunit.Microsoft.DependencyInjection up to 9.x.x
For xUnit.v3 packages use Xunit.Microsoft.DependencyInjection from 10.x.x

Also check the [migration guide](https://xunit.net/docs/getting-started/v3/migration) from xUnit for test authors.

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

[Secret manage](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows#how-the-secret-manager-tool-works) is a great tool to store credentials, api keys and other secret information for development purpose. This library has started supporting user secrets from version 8.2.0 onwards. To utilize user secrets in your tests, simply override the `virtual` method below from `TestBedFixture` class:

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
public AsyncServiceScope GetAsyncScope<T>(ITestOutputHelper testOutputHelper)
```

### Accessing the keyed wired up services in .NET 9.0

You can call the following method to access the keyed already-wired up services:

```csharp
T? GetKeyedService<T>([DisallowNull] string key, ITestOutputHelper testOutputHelper);
```

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

* Please [follow this link](https://github.com/Umplify/xunit-dependency-injection/tree/main/examples/Xunit.Microsoft.DependencyInjection.ExampleTests) to view a couple of examples on utilizing this library.
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
