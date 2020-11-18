![Nuget](https://img.shields.io/nuget/v/Xunit.Microsoft.DependencyInjection)

# Xunit Dependency Injection
Xunit does not come with any built-in dependency injection features, therefore developers have to come up with their own solution to recruit their favourite dependency injection frameworks in their tests.

This library brings in Microsoft's dependency injection container to Xunit by leveraging fixtures.

## Get started

### Nuget package
First add the nuget package to your Xunit project:

```
Install-Package Xunit.Microsoft.DependencyInjection
```

### Setup your fixture

There is an abstract class called ```Xunit.Microsoft.DependencyInjection.Abstracts.TestBedFixture``` which contains the necessary functionalities to add services and configurations to Microsoft's dependency injection container. Your concrete test fixture class derived from this abstract class must implement the following two abstract methods:

```csharp
protected abstract string GetConfigurationFile();
protected abstract void AddServices(IServiceCollection services, IConfiguration configuration);
```

```GetConfigurationFile(...)``` method returns the name of the configuration file in your Xunit test project. ```AddServices(...)``` is used to wire up services.

### Access the wired up services
There are two method that you can use to access the wired up service depending on your context:

```csharp
public T GetScopedService<T>(ITestOutputHelper testOutputHelper);
public T GetService<T>(ITestOutputHelper testOutputHelper);
```

### Preparing Xunit test classes
Your Xunit test class must be derived from ```Xunit.Microsoft.DependencyInjection.Abstracts.TestBed<T>``` class where ```T``` should be your fixture class derived from ```TestBedFixture```.

Also, the test class should be decorated by the following attribute:

```csharp
[CollectionDefinition("Dependency Injection")]
```

## Running tests in order
The library has a bonus feature that simplifies running tests in order. The test class does not have to be derived from ```TestBed<T>``` class though and it can apply to all Xunit classes.

Decorate your Xunit test class with the following attribute and associate ```TestOrder(...)``` with ```Fact```and ```Theory```:

```csharp
    [TestCaseOrderer("Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer", "Xunit.Microsoft.DependencyInjection.TestsOrder")]
```

## Examples
Please [follow this link](https://github.com/Umplify/xunit-dependency-injection/tree/main/examples/Xunit.Microsoft.DependencyInjection.ExampleTests) to view a couple of examples on utilizing this library.
