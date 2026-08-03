# Building C3

The [toolchain policy](toolchain.md) owns compiler/IDE requirements. The current
canonical compiler is Visual Studio 2017 Enterprise 15.9 MSBuild because it can
build both .NET Framework 4.0 and 4.8 lanes and explicit C# 7.3 ports.

## Commands

Run from Windows PowerShell at the repository root:

```powershell
# Verify generated/published metadata, boundaries, tests, both builds,
# every packaged binary identity, PE architecture, docs, and whitespace.
.\build\verify.ps1 -Rebuild

# Build all active lanes or one named lane.
.\build\build.ps1
.\build\build.ps1 -Lane win-x86-net40 -Configuration Debug

# Regenerate current build/channel/assembly projections after Version.props.
# This deliberately does not modify root VERSION or the legacy-1x feed.
.\build\sync-version.ps1
.\build\verify-metadata.ps1

# Create verified deterministic portable candidates after a successful build.
.\build\package.ps1 -SkipBuild

# Prove release reproducibility with two independent full rebuild/package passes.
.\build\verify-reproducible-packages.ps1
```

`build/lanes.json` is the canonical list of lanes that exist now. Scripts,
projects, CI, packaging, and validation consume the same lane IDs. Roadmap entries
do not belong in the manifest before implementation.

## Current project roles

- `C3.Catalogue.vbproj`: net40-compatible catalogue concepts/rules.
- `C3.Infrastructure.vbproj`: net40-compatible adapters and external mechanisms.
- `C3.WinForms.Net40.vbproj`: x86/net40 executable and authoritative current
  WinForms designer project.
- `C3.WinForms.Net48.vbproj`: x64/net48 executable over the same feature sources.
- `C3.Characterization.vbproj`: dependency-light net40 compatibility runner.

Generated product identity under `src/Shared/Generated` is linked into each
managed project. Do not hand-edit it; change `build/Version.props` and run the
synchronizer. `build/C3.Common.props` is the required compiler contract for every
managed project; it enables deterministic output and maps machine-specific source
roots out of compiler artifacts.

## Compatibility evidence

A successful compile proves neither minimum-OS launch nor catalogue/preference
compatibility. Publication also requires candidate-specific runtime workflows,
old/new reader matrices, preference migration, PE and binary metadata checks,
package reproducibility, and the applicable accessibility/DPI evidence in the
release-validation record.
