# Building C3

## Supported build environment

Use Windows with Visual Studio 2019 16.11 and the .NET desktop development
workload as the canonical development environment. Visual Studio 2017 remains a
working transitional environment. The .NET Framework 4.0 and 4.8 developer/
targeting packs are required once both final lanes are present.

Visual Studio 2022 and newer are useful for other repository work but are not the
authoritative compiler for C3's .NET Framework 4.0 lane. Keep a documented,
reproducible Visual Studio 2019 build environment for as long as the XP lane is
supported.

## Commands

From a Windows PowerShell prompt at the repository root:

```powershell
# Verify metadata, build every active lane, and check the diff.
.\build\verify.ps1 -Rebuild

# Build all active lanes.
.\build\build.ps1

# Build one lane.
.\build\build.ps1 -Lane win-x86-net40 -Configuration Debug

# Regenerate version projections after changing build/Version.props.
.\build\sync-version.ps1
.\build\verify-metadata.ps1
```

`build/lanes.json` is the canonical list of build lanes that exist now. A roadmap
entry does not belong in that manifest. Build scripts, CI, packaging, and release
validation consume the same lane identifiers.

## Current transition

The repository temporarily describes `win-x64-net40-transition` because the
legacy project still targets .NET Framework 4.0 for x64. It will be replaced by
`win-x64-net48` only when the separate Net48 project, configuration, shared-source
parity check, and verification path are committed together.

## Compatibility checks

A successful compile does not prove an operating-system claim. Publication also
requires runtime smoke testing on the minimum supported OS for each lane,
catalogue round-trip tests, PE architecture checks, and the manual workflows in
the release validation record.

