[![Build Status](https://dev.azure.com/umplify/Grain/_apis/build/status/Xunit/xunit-dependency-injection-PR?branchName=refs%2Fpull%2F94%2Fmerge)](https://dev.azure.com/umplify/Grain/_build/latest?definitionId=18&branchName=refs%2Fpull%2F94%2Fmerge)
![Nuget](https://img.shields.io/nuget/v/Xunit.Microsoft.DependencyInjection)
![Nuget](https://img.shields.io/nuget/dt/Xunit.Microsoft.DependencyInjection)
[![Xunit.Microsoft.DependencyInjection on fuget.org](https://www.fuget.org/packages/Xunit.Microsoft.DependencyInjection/badge.svg)](https://www.fuget.org/packages/Xunit.Microsoft.DependencyInjection)

# Xunit Dependency Injection framework
Xunit does not support any built-in dependency injection features, therefore developers have to come up with a solution to recruit their favourite dependency injection framework in their tests.

This library brings in Microsoft's dependency injection container to Xunit by leveraging Xunit's fixture.

## Getting started

### Nuget package
First add the following [nuget package](https://www.nuget.org/packages/Xunit.Microsoft.DependencyInjection/) to your Xunit project:

```
Install-Package Xunit.Microsoft.DependencyInjection
```

### Setup your fixture

The abstract class of `Xunit.Microsoft.DependencyInjection.Abstracts.TestBedFixture` contains the necessary functionalities to add services and configurations to Microsoft's dependency injection container. Your concrete test fixture class must derive from this abstract class and implement the following two abstract methods:

```csharp
protected abstract IEnumerable<string> GetConfigurationFiles();
protected abstract void AddServices(IServiceCollection services, IConfiguration configuration);
```

`GetConfigurationFiles(...)` method returns a collection of the configuration files in your Xunit test project to the framework. `AddServices(...)` method must be used to wire up the implemented services.

### Access the wired up services
There are two method that you can use to access the wired up service depending on your context:

```csharp
public T GetScopedService<T>(ITestOutputHelper testOutputHelper);
public T GetService<T>(ITestOutputHelper testOutputHelper);
```

### Adding custom logging provider
Test developers can add their own desired logger provider by overriding ```AddLoggingProvider(...)``` virtual method defined in ```TestBedFixture``` class.

### Preparing Xunit test classes
Your Xunit test class must be derived from ```Xunit.Microsoft.DependencyInjection.Abstracts.TestBed<T>``` class where ```T``` should be your fixture class derived from ```TestBedFixture```.

Also, the test class should be decorated by the following attribute:

```csharp
[CollectionDefinition("Dependency Injection")]
```

## Running tests in order
The library also has a bonus feature that simplifies running tests in order. The test class does not have to be derived from ```TestBed<T>``` class though and it can apply to all Xunit classes.

Decorate your Xunit test class with the following attribute and associate ```TestOrder(...)``` with ```Fact``` and ```Theory```:

```csharp
[TestCaseOrderer("Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer", "Xunit.Microsoft.DependencyInjection")]
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
