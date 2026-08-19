# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [10.1.0] - 2026-08-18

### Breaking Changes
- **Upgraded to xUnit.net v4** (`xunit.v3` 3.2.2 → 4.0.0). Consuming test projects must upgrade to
  `xunit.v3` 4.x as well.
- **Microsoft Testing Platform is now required to run tests.** xunit.v3 4.x runs on MTP v2, and the .NET 10
  SDK no longer runs MTP test projects through VSTest. Add a `test` section to your `global.json`:
  ```json
  { "test": { "runner": "Microsoft.Testing.Platform" } }
  ```
  Without it, `dotnet test` fails with "Testing with VSTest target is no longer supported by
  Microsoft.Testing.Platform on .NET 10 SDK and later."

### Added
- Fixtures can now use xUnit.net v4's lifecycle notification interfaces (`INotifyTestClassLifecycleAsync`
  and its siblings) for per-class setup and teardown. See `LifecycleAwareFixture` and
  `LifecycleNotificationTests` in the examples project.
- Documentation for xUnit.net v4's full test parallelization (`ParallelMode`), including what this library
  guarantees under `ParallelMode.All` and what still requires opting out.

### Fixed
- `TestBedFixture.GetServiceProvider` is now thread-safe. Previously, concurrent first access to a shared
  fixture could run `AddServices` more than once against the same `ServiceCollection` (throwing
  "Collection was modified") and build multiple `ServiceProvider` instances, so tests could observe
  different singletons. This is required for shared fixtures to work under `ParallelMode.All`.

### Changed
- Updated all Microsoft.Extensions.* packages from 10.0.9 to 10.0.11
- Updated `xunit.runner.visualstudio` from 3.1.5 to 4.0.0
- Updated `Microsoft.NET.Test.Sdk` from 18.7.0 to 18.9.0
- Updated `Microsoft.SourceLink.GitHub` from 10.0.300 to 10.0.400
- Replaced `coverlet.collector` (a VSTest data collector, inert under MTP) with
  `Microsoft.Testing.Extensions.CodeCoverage`; collect coverage with `dotnet test --coverage`
- Added `Microsoft.Testing.Extensions.TrxReport` so CI can keep publishing TRX results via `--report-trx`
- Azure Pipelines now use the .NET 10.0.400 SDK and publish MTP-produced TRX results

### Migration Guide
To migrate from 10.0.x to 10.1.0:
1. Update `xunit.v3` (and `xunit.runner.visualstudio`, if referenced) to 4.0.0
2. Update package reference to version 10.1.0:
   ```xml
   <PackageReference Include="Xunit.Microsoft.DependencyInjection" Version="10.1.0" />
   ```
3. Add the `test` section shown above to `global.json`
4. In CI, replace VSTest switches: `--logger trx` → `--report-trx`,
   `--collect "XPlat Code Coverage"` → `--coverage`, and pass projects with `--project`
5. Replace any string-based `[TestCaseOrderer("Type", "Assembly")]` with `[TestCaseOrderer(typeof(T))]`
   or `[TestCaseOrderer<T>]`

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
