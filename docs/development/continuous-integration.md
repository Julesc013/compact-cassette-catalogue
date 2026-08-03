# Continuous Integration

C3 uses two complementary automation levels. A workflow name describes evidence
that it actually produces; repository checks are never presented as compiled
compatibility evidence.

## Hosted repository checks

`.github/workflows/repository-checks.yml` runs for pushes and pull requests on a
pinned Windows Server 2022 image. It verifies:

- canonical version and release metadata;
- dependency direction between production modules;
- typed WinForms access through the catalogue services and settings adapter;
- identical physical source ownership in both WinForms project files; and
- whitespace integrity.

These checks intentionally do not compile C3. Microsoft documents that Visual
Studio 2022 and later cannot build applications targeting .NET Framework 4.0
through 4.5.1. GitHub retired its Visual Studio 2019-capable `windows-2019`
hosted runner on 30 June 2025.

## Authoritative compatibility gate

`.github/workflows/full-compatibility.yml` is a manually dispatched workflow on
a self-hosted runner with these labels:

```text
self-hosted
Windows
X64
c3-vs2019
```

The runner must have:

- Visual Studio 2019 or Build Tools 2019 with MSBuild and Visual Basic support;
- the .NET Framework 4.0 targeting pack;
- the .NET Framework 4.8 targeting pack; and
- Windows PowerShell 5.1 or later.

The workflow runs the complete repository gate, compiles both release lanes,
runs the characterization suite, verifies PE architecture, builds deterministic
portable archives, verifies their contents and hashes, and uploads the result as
a short-lived workflow artifact.

## Branch protection

Make `Repository checks / Metadata, boundaries, and project parity` required for
pull requests. Do not make the manually dispatched self-hosted job a pull-request
requirement because an offline private runner would leave every contribution
permanently queued.

Before merging a release candidate, a maintainer must run the authoritative
compatibility workflow and record its run URL in the release-validation record.
The absence of that evidence blocks a release, not ordinary documentation-only
contributions.

## Supply-chain policy

Workflows have read-only repository permissions. Third-party actions are limited
to GitHub-maintained checkout and artifact actions. Renovation of action versions
must be reviewed like any other dependency change; a moving `@main` reference is
not permitted.

## References

- [Microsoft: .NET Framework and Windows OS versions](https://learn.microsoft.com/en-us/dotnet/framework/install/versions-and-dependencies)
- [GitHub: Windows 2019 runner retirement](https://github.com/actions/runner-images/issues/12045)
