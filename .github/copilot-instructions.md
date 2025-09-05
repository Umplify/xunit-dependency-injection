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
  -- takes ~10.8 seconds with 9 tests passing. NEVER CANCEL. Set timeout to 60+ seconds.
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

## Validation Scenarios

After making any changes to the library code:
1. **Build validation**: Run `dotnet build --configuration Release` and ensure it completes successfully
2. **Test validation**: Run example tests with `dotnet test --configuration Release` and verify all 9 tests pass
3. **Format validation**: Run `dotnet format` to ensure code follows project standards
4. **Package validation**: Run `dotnet pack --configuration Release` to ensure the library can be packaged

### Manual Testing Scenarios
The example tests demonstrate complete usage patterns:
- **Dependency injection setup**: Tests show how to configure services in `TestProjectFixture`
- **Service resolution**: Tests verify both scoped and singleton service resolution
- **Keyed services**: Tests validate keyed service injection (new in .NET 9.0)
- **Configuration binding**: Tests demonstrate configuration file and user secrets integration
- **Test ordering**: Tests show the test ordering feature with `TestOrder` attributes

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

### Working with Examples
The examples project is a fully functional test suite that demonstrates:
- Service registration and dependency injection patterns
- Configuration file usage with `appsettings.json`
- User secrets integration for sensitive data
- Keyed services (Porsche/Toyota car maker examples)
- Test ordering with custom attributes
- Both synchronous and asynchronous test patterns

Always use the examples to validate that your changes don't break real-world usage scenarios.

## Build Times and Performance Expectations
- **Package restore**: ~1-8 seconds (varies with cache state)
- **Build (Release)**: ~4-6 seconds
- **Test execution**: ~9-11 seconds (9 tests pass)
- **Code formatting**: ~7-10 seconds
- **Package creation**: ~1-2 seconds
- **Complete workflow**: ~20-25 seconds total

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