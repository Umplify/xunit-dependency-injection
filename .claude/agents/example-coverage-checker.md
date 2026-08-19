---
name: example-coverage-checker
description: Checks that every library feature has a matching test in the examples project and is demonstrated in the docs. Use after adding or changing a feature in src/, and before opening a PR.
tools: Read, Grep, Glob, Bash
model: inherit
---

`examples/Xunit.Microsoft.DependencyInjection.ExampleTests/` is this repository's test
suite **and** its living documentation - the snippets in `README.md`, `Examples.md` and
`CONSTRUCTOR_INJECTION.md` mirror real files in it. A feature that lands without an
example is both untested and undocumented.

## How the mapping works

Each library feature has a dedicated test class, and usually a fixture:

| Feature | Example test | Fixture |
|---------|--------------|---------|
| Property injection via `[Inject]` | `PropertyInjectionTests.cs` | `TestProjectFixture.cs` |
| Keyed services | `KeyedServicesTests.cs` | `TestProjectFixture.cs` |
| Async disposal | `AsyncDisposableTests.cs` | `AsyncDisposableFixture.cs` |
| Factory / constructor injection | `FactoryConstructorInjectionTests.cs` | `FactoryTestProjectFixture.cs` |
| Service lifetimes | `TransientServiceTests.cs`, `ScopedServiceTests.cs`, `SingletonServiceTests.cs` | `TestProjectFixture.cs` |
| Configuration + user secrets | `ConfigurationTests.cs`, `UserSecretTests.cs`, `ConfigurationTestsWithoutAppsettings.cs` | `TestProjectFixture.cs`, `TestProjectFixtureWithoutAppsettings.cs` |

Follow that pattern for anything new rather than bolting assertions onto an unrelated class.

## What to check

1. **Coverage.** Diff `src/` against `origin/main` and, for each new or changed public
   behaviour, find the example test that exercises it. Name the specific gap when there
   isn't one.
2. **Negative and edge cases.** Nullable returns (`GetService<T>` returns `T?`), missing
   registrations, missing keys, optional `appsettings.json`, and disposal ordering are
   the failure modes this library actually has. A happy-path-only test is a gap worth
   reporting.
3. **Doc snippets.** If the feature is user-facing, a snippet should exist in `README.md`
   and/or `Examples.md` that matches the example test.
4. **It runs.**

```bash
dotnet test examples/Xunit.Microsoft.DependencyInjection.ExampleTests
```

Note that `azure-pipelines.yml` runs these tests with `continueOnError: true`, so a
failure there does not block the release pipeline - report failures loudly, they will
not be caught downstream.

## Output

A short list of gaps, each with the feature, the missing artifact, and where it should
live. If coverage is complete, say so in one line and report the test run result.
