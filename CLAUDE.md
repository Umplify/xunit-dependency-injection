# Xunit.Microsoft.DependencyInjection

Microsoft dependency-injection container for xUnit, shipped as the
[`Xunit.Microsoft.DependencyInjection`](https://www.nuget.org/packages/Xunit.Microsoft.DependencyInjection/)
NuGet package. **This is a published library** - anything public in `src/` is consumed by
projects outside this repository.

## Layout

| Path | What it is |
|------|------------|
| `src/` | The packaged library. `Abstracts/` holds the base classes users derive from, plus `Attributes/`, `Logging/`, `TestsOrder/` |
| `examples/Xunit.Microsoft.DependencyInjection.ExampleTests/` | The test suite *and* the source of the documentation snippets - one test class per feature |
| `Directory.Packages.props` | Central package management - **all** versions are pinned here, never in a csproj |
| `azure-pipelines.yml` | Release pipeline; packs and pushes to nuget.org on a tag |
| `azure-pipeline-PR.yml` | PR validation pipeline |

## Build and test

```bash
dotnet build src/Xunit.Microsoft.DependencyInjection.sln     # covers both projects
dotnet test examples/Xunit.Microsoft.DependencyInjection.ExampleTests
```

Targets `net10.0` only, on **xUnit v3** (`xunit.v3.extensibility.core`). xUnit v2 APIs do
not apply here.

## Conventions that are easy to get wrong

- **XML docs are mandatory on public members.** `GenerateDocumentationFile` is on and
  CS1591 is deliberately *not* suppressed, because the generated `.xml` is packed for
  NuGet consumers. A missing `/// <summary>` is a real defect, not a nit.
- **`src/.editorconfig` uses tabs and CRLF.** Run `dotnet format` rather than formatting
  by hand; the `PostToolUse` hook in `.claude/settings.json` does this automatically.
- **New dependencies go in `Directory.Packages.props`**, then a bare
  `<PackageReference Include="..." />` in the csproj.
- **A feature is not done without an example test.** Add a test class under `examples/`
  following the existing one-class-per-feature pattern, and update the docs that quote it.
- **The version number is hard-coded in three places** - the csproj and both Azure
  pipelines - plus the docs. Use the `release-prep` skill when bumping it.
- **Example tests do not gate the release pipeline** (`continueOnError: true` in
  `azure-pipelines.yml`), so run them locally and take failures seriously.

## Documentation

`README.md`, `Examples.md` and `CONSTRUCTOR_INJECTION.md` document the public API in prose
and in compilable snippets. Changing the public surface means changing these too - the
`docs-sync` skill lists what covers what.

The xUnit-compatibility notes in `README.md` ("up to 9.0.5", "from 9.1.0") are historical
and should not be bumped with the version.

## Claude Code setup in this repo

- `.claude/skills/release-prep` - version bump across every site, invoked with `/release-prep`
- `.claude/skills/docs-sync` - keep docs and public API in step
- `.claude/agents/public-api-reviewer` - breaking-change and semver review
- `.claude/agents/example-coverage-checker` - feature-to-example coverage
- `.claude/hooks/post-edit-cs.sh` - formats edited C# and flags missing public XML docs
- `.mcp.json` - NuGet MCP server, for target-framework-aware package version updates
