[![Build Status](https://dev.azure.com/umplify/Grain/_apis/build/status/Umplify.xunit-dependency-injection?branchName=main)](https://dev.azure.com/umplify/Grain/_build/latest?definitionId=17&branchName=main)
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

### Adding custom logging provider
Test developers can add their own desired logger provider by overriding ```AddLoggingProvider(...)``` virtual method defined in ```TestBedFixture``` class.

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
[TestCaseOrderer("Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer", "Xunit.Microsoft.DependencyInjection")]
```

## Examples
Please [follow this link](https://github.com/Umplify/xunit-dependency-injection/tree/main/examples/Xunit.Microsoft.DependencyInjection.ExampleTests) to view a couple of examples on utilizing this library.

### One more thing
Do not forget to include the following nuget packages in your Xunit project:

```xml
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="3.1.10" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="3.1.10" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="3.1.10" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="3.1.10" />
    <PackageReference Include="Microsoft.Extensions.Configuration.FileExtensions" Version="3.1.10" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="3.1.10" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="3.1.10" />
```

## A note on .NET 5.0
The .net 5.0 version of this library will be available on early 2021 and the reason for that is Microsoft's support on Azure Functions. Should there is an absolute need on supporting .net 5.0 in this library, please create a PR.

An update on Mar 14th, 2021: Due to the gaps in supporting Azure Functions described on [this post](related to the gaps in binding and durable tasks), we are delaying upgrading this library to .NET 5.0 until those gaps are closed by Microsoft.
