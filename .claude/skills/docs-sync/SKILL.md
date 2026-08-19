---
name: docs-sync
description: Keep README.md, Examples.md and CONSTRUCTOR_INJECTION.md in step with the public API in src/, including the compilable snippets and the matching example tests. Use after changing anything public in src/ or when docs and code look out of step.
---

# Docs sync

This library documents its public surface in prose *and* in copy-pasteable C# snippets
spread across three long documents. A change to `src/` that stops at the code leaves the
documentation quietly wrong for NuGet consumers.

## The documentation set

| File | Covers |
|------|--------|
| `README.md` | Quick start, the three injection approaches, keyed services, configuration, async disposal, test ordering |
| `Examples.md` | Long-form worked examples for each feature |
| `CONSTRUCTOR_INJECTION.md` | The factory / constructor-injection approach |
| `examples/Xunit.Microsoft.DependencyInjection.ExampleTests/` | The runnable counterpart of every snippet |

## What to check

1. **Signatures.** Every method quoted in the docs still exists with that exact
   signature - `GetService<T>`, `GetKeyedService<T>`, `GetScopedService<T>`,
   `GetAsyncScope`, `AddServices`, `GetTestAppSettings`, and the
   `[Inject]` / `[FromKeyedService]` / `[TestOrder]` attributes.
2. **Snippets compile.** The snippets mirror real types in the examples project. When a
   snippet changes, the corresponding file under `examples/` usually changes with it.
3. **Feature parity.** A new public feature needs all three: an entry in `README.md`,
   a worked example in `Examples.md`, and a test in the examples project. Async disposal
   is the reference case - `AsyncDisposableTests.cs` + `AsyncDisposableFixture.cs` plus
   the README section landed together.
4. **XML docs.** Public members in `src/` carry `///` comments; CS1591 is intentionally
   not suppressed because the generated `.xml` is packed into the NuGet package.
5. **Dependency versions quoted in docs.** `README.md` shows a
   `<PackageReference Include="xunit.v3" ... />` sample - it should match the version
   pinned in `Directory.Packages.props`.

Leave the historical xUnit-compatibility notes in `README.md` alone (the "up to 9.0.5" /
"from 9.1.0" lines) - those describe past releases, not the current one.

## Verify

```bash
dotnet build src/Xunit.Microsoft.DependencyInjection.sln
dotnet test examples/Xunit.Microsoft.DependencyInjection.ExampleTests
```

Report which documents you changed and which snippets you verified against real code.
