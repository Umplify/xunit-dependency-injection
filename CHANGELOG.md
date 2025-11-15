# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [10.0.0] - 2025-11-15

### Breaking Changes
- **Upgraded to .NET 10.0** - This version targets `net10.0` exclusively
- **Dropped support for all previous .NET versions** - .NET 9.0, 8.0, and earlier are no longer supported
- Users must upgrade to .NET 10.0 SDK to use this version

### Changed
- Updated all Microsoft.Extensions.* packages from 9.0.10 to 10.0.0
- Updated xUnit packages from 3.1.0 to 3.2.0
- Updated Microsoft.NET.Test.Sdk from 18.0.0 to 18.0.1
- Updated Azure DevOps build pipelines to use .NET 10.0.100 SDK

### Fixed
- Fixed lambda parameter naming conflict with C# `scoped` keyword in example tests

### Migration Guide
To migrate from version 9.x to 10.0.0:
1. Install .NET 10.0 SDK on your development machine
2. Update your project's `TargetFramework` to `net10.0`
3. Update package reference to version 10.0.0 or later:
   ```xml
   <PackageReference Include="Xunit.Microsoft.DependencyInjection" Version="10.0.0" />
   ```
4. Rebuild and test your project

## [9.2.2] - Previous Release
- See git history for changes prior to version 10.0.0
