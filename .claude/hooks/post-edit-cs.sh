#!/usr/bin/env bash
# PostToolUse hook: keep edited C# files aligned with src/.editorconfig, and
# enforce XML documentation on the public surface of the packaged library.
#
# The library ships with GenerateDocumentationFile=true and deliberately does
# NOT suppress CS1591, so every public member in src/ must carry /// docs or the
# packed .xml that NuGet consumers rely on becomes incomplete.
#
# Reads the PostToolUse JSON payload on stdin.
# Exit 2 => stderr is fed back to Claude so it can fix the problem immediately.

set -uo pipefail

repo_root="${CLAUDE_PROJECT_DIR:-$(git rev-parse --show-toplevel 2>/dev/null)}"
[ -n "$repo_root" ] || exit 0
cd "$repo_root" || exit 0

file_path=$(jq -r '.tool_input.file_path // empty')
[ -n "$file_path" ] || exit 0
case "$file_path" in
  *.cs) ;;
  *) exit 0 ;;
esac
[ -f "$file_path" ] || exit 0

command -v dotnet >/dev/null 2>&1 || exit 0

solution="src/Xunit.Microsoft.DependencyInjection.sln"
library="src/Xunit.Microsoft.DependencyInjection.csproj"

# 1. Formatting - the .editorconfig here is unusual (tabs, CRLF), so never
#    hand-format; let dotnet format apply it.
dotnet format "$solution" --include "$file_path" --no-restore >/dev/null 2>&1

# 2. Public XML documentation - library sources only. The examples project is
#    IsPackable=false and has no documentation requirement.
case "$file_path" in
  src/*|"$repo_root"/src/*) ;;
  *) exit 0 ;;
esac

missing_docs=$(dotnet build "$library" --no-restore -v q 2>&1 | grep 'CS1591' | sort -u | head -20)
if [ -n "$missing_docs" ]; then
  {
    echo "Missing XML documentation on the public API surface (CS1591)."
    echo "This library packs its .xml docs for NuGet consumers - add /// <summary> comments:"
    echo "$missing_docs"
  } >&2
  exit 2
fi

exit 0
