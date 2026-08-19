---
name: release-prep
description: Bump the library version across every place it is hard-coded (csproj, both Azure pipelines, CHANGELOG, README), then verify no stale version strings remain. Use when cutting a new release of Xunit.Microsoft.DependencyInjection.
disable-model-invocation: true
---

# Release prep

The package version lives in **three independent places** that are not derived from one
another, plus documentation that quotes it. They drift. As of this writing `README.md`
still advertised `Version="9.2.0"` while the library shipped `10.0.5`.

Take the target version as `MAJOR.MINOR.PATCH` from the user. If they did not give one,
read the current version from the csproj and ask which part to bump.

## 1. Update the version sites

Every one of these must be changed - none of them read from the others:

| File | What to change |
|------|----------------|
| `src/Xunit.Microsoft.DependencyInjection.csproj` | `<Version Condition="'$(Version)' == ''">` |
| `azure-pipelines.yml` | `Major` / `Minor` / `Revision` variables (release + nuget.org push) |
| `azure-pipeline-PR.yml` | `Major` / `Minor` / `Patch` variables |
| `README.md` | the `PackageReference` sample under *Nuget package* |
| `CHANGELOG.md` | new entry at the top |

Also refresh `<Copyright>` in the csproj if the year has rolled over.

## 2. Write the CHANGELOG entry

Format is [Keep a Changelog](https://keepachangelog.com/en/1.0.0/); the project follows
semver. Build the entry from the commits since the last release:

```bash
git log --oneline "$(git describe --tags --abbrev=0)"..HEAD
```

Group under `### Added` / `### Changed` / `### Fixed` / `### Breaking Changes`.
A major bump **must** also carry a `### Migration Guide` section - see the `10.0.0`
entry for the shape to copy.

## 3. Do not touch the historical compatibility notes

`README.md` contains xUnit-version guidance that is history, not the current version:

* "use versions **up to** 9.0.5" (xUnit v2)
* "use versions **from** 9.1.0" (xUnit v3)
* "For **.NET 10.0** use version **10.0.0 or later**"

These stay as they are. Only bump the copy-paste `PackageReference` sample.

## 4. Verify

```bash
dotnet build src/Xunit.Microsoft.DependencyInjection.sln -c Release
dotnet test examples/Xunit.Microsoft.DependencyInjection.ExampleTests -c Release
dotnet pack src/Xunit.Microsoft.DependencyInjection.csproj -c Release \
  /p:Version=<NEW_VERSION> -o /tmp/pack-check
```

Then confirm nothing stale is left behind - the old version must not appear anywhere
except `CHANGELOG.md`:

```bash
grep -rn "<OLD_VERSION>" --include="*.csproj" --include="*.yml" --include="*.md" . \
  | grep -v CHANGELOG.md
```

Report the pack output (`.nupkg` + `.snupkg`) and the grep result before handing back.
The release itself is published by `azure-pipelines.yml` on a tag push - do not push
tags or publish to nuget.org yourself unless explicitly asked.
