# Continuous Integration

C3 uses two complementary automation levels. A workflow name describes evidence
that it actually produces; repository checks are never presented as compiled
compatibility evidence.

## Hosted repository checks

`.github/workflows/repository-checks.yml` runs for pushes and pull requests on a
pinned Windows Server 2022 image. It verifies:

- canonical version and release metadata;
- release-catalogue lifecycle and checkpoint contracts;
- the shared deterministic compiler contract;
- dependency direction between production modules;
- typed WinForms access through catalogue services and the C3-owned preference
  boundary, with `My.Settings` reintroduction prohibited;
- identical physical source ownership in both WinForms project files; and
- whitespace integrity.

These checks intentionally do not compile C3. Microsoft documents that Visual
Studio 2022 and later cannot build applications targeting .NET Framework 4.0
through 4.5.1. GitHub retired its Visual Studio 2019-capable `windows-2019`
hosted runner on 30 June 2025.

## Authoritative compatibility gate

`.github/workflows/full-compatibility.yml` is a manually dispatched workflow on
a self-hosted runner with these capability labels:

```text
self-hosted
Windows
X64
c3-legacy-msbuild
```

The runner must satisfy the [toolchain policy](toolchain.md). The maintained
machine currently resolves Visual Studio 2017 Enterprise 15.9. It must have:

- Visual Studio 2017 15.9 with MSBuild and VB/C# desktop support;
- the .NET Framework 4.0 targeting pack;
- the .NET Framework 4.8 targeting pack; and
- Windows PowerShell 5.1 or later.

The workflow runs the complete repository gate, compiles both release lanes,
runs characterization, verifies binary identities and PE architecture, builds
the candidate twice from clean path-distinct source roots, verifies exact
contents/hashes, and uploads the retained proven result as a short-lived workflow
artifact.

Qualified `v2.*` tag pushes also run the release-contract check with full Git
history. It verifies annotated-tag identity, reachability from `master`, the
matching catalogue/validation record, the frozen source ancestor, the
evidence-only attestation diff, and recorded package identities. A shallow
checkout cannot provide this evidence.

## Branch protection

Make `Repository checks / Metadata, lifecycle, boundaries, and project parity`
required for pull requests. Protect `master` from direct feature commits,
force-push, and deletion; protect `maintenance/1.x` from force-push/deletion and
2.x identity changes; and prevent replacement of `v2.*` tags. `master` advances
only by the documented fast-forward checkpoint promotion.

Do not make the manually dispatched self-hosted job a routine pull-request
requirement because an offline private runner would leave ordinary contributions
permanently queued. It is mandatory evidence for checkpoint promotion and public
release candidates.

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
