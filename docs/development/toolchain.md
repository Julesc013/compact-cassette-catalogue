# Development toolchain policy

This document owns compiler and IDE requirements. Build scripts resolve the
compiler; other documentation links here instead of naming a different
authoritative Visual Studio release.

## Canonical compiler

The current repository resolves Visual Studio 2017 Enterprise 15.9 MSBuild on
the maintained development machine. It is the canonical compiler because it can
target both .NET Framework 4.0 and 4.8 and supports explicit C# 7.3 for staged
managed ports.

Required components:

- MSBuild and Visual Basic/C# desktop compilers;
- .NET Framework 4.0 reference assemblies/targeting pack;
- .NET Framework 4.8 developer/targeting pack; and
- Windows PowerShell 5.1 or later for repository automation.

`build/resolve-msbuild.ps1` accepts Visual Studio 2017 or 2019 because either can
provide the required legacy MSBuild contract. A release record names the exact
resolved path and version. Do not describe an uninstalled toolchain as the local
authority.

## Installed IDE roles

| IDE | C3 role | Authority |
| --- | --- | --- |
| Visual Studio 2010 Enterprise | Historical net40/VB designer and compatibility investigation | Reference only; not the full current solution gate |
| Visual Studio 2017 Enterprise 15.9 | Compile, test, and design both current lanes; compile explicit C# 7.3 | Canonical current build/designer toolchain |
| Visual Studio 2022 Enterprise | Editing, review, analysis, and modern-lane experiments | Must not be used to claim a net40 build |
| Visual Studio 2026 Enterprise | Editing, review, analysis, and forward experiments | Must not be used to claim a net40 build |

Microsoft documents that Visual Studio 2022 and later cannot build projects
targeting .NET Framework 4.0 through 4.5.1. They may coexist and help with
repository work, but the full gate must use the legacy-capable compiler while the
XP lane remains supported.

## Language policy

- Existing VB projects retain `Option Explicit`, `Option Strict`, and warnings as
  errors.
- New/ported C# .NET Framework projects pin `<LangVersion>7.3</LangVersion>` and
  warnings as errors.
- Do not set `latest` or `preview`; the source contract must not change when an
  IDE updates.
- Framework APIs must be available in the project's target, irrespective of what
  syntax a newer compiler accepts.
- Native setup/bootstrap work follows Universal Setup's pinned C11 policy; C++11
  requires a separate measured boundary decision.

## Verification

The full gate records MSBuild file version, target reference assemblies, build
configuration, source commit, and output identities. IDE designer-open evidence
is recorded for changed WinForms surfaces. A successful build on a modern host is
not evidence that the output launches on Windows XP or Windows 7.

## References

- [Microsoft: configure the C# language version](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version)
- [Microsoft: .NET Framework versions and dependencies](https://learn.microsoft.com/en-us/dotnet/framework/install/versions-and-dependencies)
- [Microsoft: Visual Studio 2017 15.8 release notes](https://learn.microsoft.com/en-us/visualstudio/releases/2017/vs2017-relnotes-v15.8)
