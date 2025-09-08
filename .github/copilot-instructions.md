# Xunit Dependency Injection Library for .NET 9.0

Xunit Dependency Injection is a .NET library that brings Microsoft's dependency injection container to Xunit by leveraging Xunit's fixture pattern. This library enables dependency injection in xUnit tests using familiar Microsoft.Extensions.DependencyInjection patterns.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Installation
- **CRITICAL**: This project requires .NET 9.0 SDK (version 9.0.304 or later) and .NET 9.0 runtime (version 9.0.8 or later).
- Install .NET 9.0 SDK:
  ```bash
  # Download the install script
  curl -sSL -o dotnet-install.sh https://dot.net/v1/dotnet-install.sh
  # Download the official SHA-256 checksum (replace URL with actual checksum file if available)
  curl -sSL -o dotnet-install.sh.sha256 https://dot.net/v1/dotnet-install.sh.sha256
  # Verify checksum
  sha256sum -c dotnet-install.sh.sha256
  # If verification passes, install .NET SDK
  bash dotnet-install.sh --version 9.0.304 --install-dir /tmp/dotnet
  export PATH="/tmp/dotnet:$PATH"
  export DOTNET_ROOT="/tmp/dotnet"
  ```
- Verify installation: `dotnet --version` should return `9.0.304` or later
- Verify runtime: `dotnet --list-runtimes` should show `Microsoft.NETCore.App 9.0.8`

### Build and Test Commands
- **Navigate to source directory**: `cd /path/to/xunit-dependency-injection/src`
- **Restore packages**: `dotnet restore` -- takes ~8 seconds. NEVER CANCEL. Set timeout to 30+ seconds.
- **Build library**: `dotnet build --configuration Release` -- takes ~5.5 seconds. NEVER CANCEL. Set timeout to 30+ seconds.
- **Build examples**: The build command automatically builds both the main library and example tests.
- **Run example tests**: 
  ```bash
  cd ../examples/Xunit.Microsoft.DependencyInjection.ExampleTests
  dotnet test --configuration Release
  ```
  -- takes ~10.8 seconds with 43 tests passing. NEVER CANCEL. Set timeout to 60+ seconds.
- **Package library**: `dotnet pack --configuration Release` -- takes ~1.9 seconds. NEVER CANCEL. Set timeout to 30+ seconds.

### Code Quality and Formatting
- **Format code**: `dotnet format Xunit.Microsoft.DependencyInjection.sln` -- fixes whitespace and code style issues per .editorconfig
- **Verify formatting**: `dotnet format Xunit.Microsoft.DependencyInjection.sln --verify-no-changes --verbosity diagnostic`
- **ALWAYS** run `dotnet format` before committing changes to maintain code style consistency
- The project uses .editorconfig with specific C# coding standards including tabs for indentation and CRLF line endings

### Development Workflow
- Always set `PATH="/tmp/dotnet:$PATH"` and `DOTNET_ROOT="/tmp/dotnet"` in your session when working with .NET 9.0
- Test your changes by running the example tests which demonstrate real usage scenarios
- The library targets `net9.0` framework exclusively
- Use Visual Studio Code tasks defined in `.vscode/tasks.json` for build, publish, and watch operations

## Understanding Test Patterns

The library supports multiple dependency injection approaches:

### 1. Traditional Fixture-Based (Fully Supported)
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

### 2. Property Injection (Recommended - New in 9.2.0+)
```csharp
public class MyTests : TestBedWithDI<TestProjectFixture>
{
    [Inject] private ICalculator Calculator { get; set; } = null!;
    [Inject("Porsche")] private ICarMaker PorscheMaker { get; set; } = null!;
    
    [Fact]
    public async Task TestCalculation()
    {
        var result = await Calculator.AddAsync(1, 2);
        Assert.Equal(3, result);
    }
}
```

### 3. Factory Pattern (Experimental)
```csharp
public class MyTests : TestBed<FactoryTestProjectFixture>
{
    [Fact] 
    public async Task TestConstructorInjection()
    {
        var service = _fixture.CreateTestInstance<SimpleService>(_testOutputHelper);
        var result = await service.CalculateAsync(10, 5);
        Assert.True(result > 0);
    }
}
```

## Validation Scenarios

After making any changes to the library code:
1. **Build validation**: Run `dotnet build --configuration Release` and ensure it completes successfully
2. **Test validation**: Run example tests with `dotnet test --configuration Release` and verify all 43 tests pass
3. **Format validation**: Run `dotnet format` to ensure code follows project standards
4. **Package validation**: Run `dotnet pack --configuration Release` to ensure the library can be packaged

### Manual Testing Scenarios
The example tests demonstrate complete usage patterns:
- **Dependency injection setup**: Tests show how to configure services in `TestProjectFixture`
- **Service resolution**: Tests verify both scoped and singleton service resolution
- **Keyed services**: Tests validate keyed service injection (new in .NET 9.0)
- **Configuration binding**: Tests demonstrate configuration file and user secrets integration
- **Test ordering**: Tests show the test ordering feature with `TestOrder` attributes

#### Testing Specific Features
Run individual test scenarios to validate changes:
```bash
# Test property injection (new dependency injection pattern)
dotnet test --filter "TestCalculatorThroughPropertyInjection" --configuration Release

# Test keyed services (Porsche/Toyota car makers)  
dotnet test --filter "GetKeyedService" --configuration Release

# Test factory pattern (constructor injection)
dotnet test --filter "TestConstructorInjectionViaFactory" --configuration Release

# Test configuration and user secrets
dotnet test --filter "TestSecretValues" --configuration Release

# List all available tests
dotnet test --list-tests --configuration Release
```

## Project Structure

### Key Directories and Files
- `/src/` - Main library source code
  - `Abstracts/TestBedFixture.cs` - Base class for test fixtures
  - `Abstracts/TestBed.cs` - Base class for test classes
  - `TestsOrder/TestPriorityOrderer.cs` - Test ordering implementation
  - `.editorconfig` - Code style configuration
- `/examples/Xunit.Microsoft.DependencyInjection.ExampleTests/` - Working examples and integration tests
  - `Fixtures/TestProjectFixture.cs` - Example fixture setup
  - Various test files demonstrating usage patterns
- `.github/workflows/` - CI/CD automation (Azure Pipelines used, not GitHub Actions)
- `.vscode/` - VS Code configuration for development

### Important Configuration Files
- `src/Xunit.Microsoft.DependencyInjection.csproj` - Main project file targeting net9.0
- `azure-pipelines.yml` - Build automation configuration  
- `examples/*/Xunit.Microsoft.DependencyInjection.ExampleTests.csproj` - Example test project

## Common Development Tasks

### Adding New Features
1. Modify source code in `/src/` directory
2. Add corresponding tests in `/examples/` directory 
3. Run build and test validation
4. Format code with `dotnet format`
5. Verify all example tests still pass

### Troubleshooting Build Issues
- **SDK Version Error**: Ensure .NET 9.0 SDK is installed and in PATH
- **Runtime Error during Tests**: Ensure .NET 9.0 runtime is installed and DOTNET_ROOT is set
- **Format Issues**: Run `dotnet format` to auto-fix most style problems
- **Missing Dependencies**: Run `dotnet restore` to restore NuGet packages

#### Common Validation Failures
- **Build succeeds but tests fail**: Check if you're in the correct directory (`examples/Xunit.Microsoft.DependencyInjection.ExampleTests`)
- **"No tests found" error**: Verify test filter syntax with `dotnet test --list-tests` first
- **Timeout during restore/build**: DO NOT CANCEL - operations take 8-15 seconds normally, set 60+ second timeouts
- **SourceLink warnings**: These are normal during format validation and can be ignored
- **Test count mismatch**: Current test suite has 43 tests; if you see different counts, investigate test changes

### Working with Examples
The examples project is a fully functional test suite that demonstrates:
- **Traditional fixture-based approach**: See `CalculatorTests.cs` using `TestBed<TFixture>` and `_fixture.GetService<T>(_testOutputHelper)`
- **Property injection approach**: See `PropertyInjectionTests.cs` using `TestBedWithDI<TFixture>` with `[Inject]` attributes
- **Factory constructor injection**: See `FactoryConstructorInjectionTests.cs` for experimental true constructor injection
- **Service registration patterns**: Multiple service lifetimes (transient, scoped, singleton)
- **Configuration file usage**: `appsettings.json` integration
- **User secrets integration**: Sensitive data handling for development
- **Keyed services**: Porsche/Toyota car maker examples demonstrating .NET 9.0 keyed services
- **Test ordering**: Custom attributes for controlling test execution order
- **Advanced patterns**: `Func<T>`, `Action<T>`, and `IOptions<T>` injection patterns

Always use the examples to validate that your changes don't break real-world usage scenarios.

## Build Times and Performance Expectations
- **Package restore**: ~1-8 seconds (varies with cache state)
- **Build (Release)**: ~4-6 seconds
- **Test execution**: ~9-11 seconds (43 tests pass)
- **Code formatting**: ~7-10 seconds
- **Package creation**: ~1-2 seconds
- **Complete workflow**: ~25-35 seconds total

**CRITICAL**: NEVER CANCEL builds or tests. These times are normal. Set timeouts to 2-5 minutes to catch actual hanging processes, but actual operations complete much faster.

## Azure DevOps Integration
The project uses Azure Pipelines (not GitHub Actions) for CI/CD:
- Build configuration in `azure-pipelines.yml`
- Targets Ubuntu 22.04 build agents
- Uses .NET 9.0.304 SDK version
- Publishes to NuGet.org on successful builds
- Example tests are run as part of the pipeline

## Frequently Used Commands Summary
```bash
# Setup environment
export PATH="/tmp/dotnet:$PATH"
export DOTNET_ROOT="/tmp/dotnet"

# Build and test workflow  
cd src
dotnet restore                           # ~8s
dotnet build --configuration Release    # ~5.5s
cd ../examples/Xunit.Microsoft.DependencyInjection.ExampleTests
dotnet test --configuration Release     # ~10.8s

# Code quality
cd ../../src  
dotnet format Xunit.Microsoft.DependencyInjection.sln

# Package
dotnet pack --configuration Release     # ~1.9s
```

Always validate your changes by running through this complete workflow before committing.