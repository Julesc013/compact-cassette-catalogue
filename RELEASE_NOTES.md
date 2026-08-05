# Compact Cassette Catalogue 1.3.0 Alpha 2

C3 1.3.0 Alpha 2 is the planned, intentionally unpublished three-lane build and
release-control checkpoint for the final original C3 line. The source now
projects the exact `1.3.0a2 / Alpha 2 / v1.3.0a2` identity, but this document is
not evidence that the tag or distributions already exist.

Alpha 2 becomes complete only after the maintained builders are serviced, the
full Preparation and Candidate paths pass from one clean pushed source and one
immutable external lock, the three retained packages reproduce, and the
annotated tag is created and verified.

## Intended retained assets

```text
C3-v1.3.0a2-win-x86-net40-portable.zip
C3-v1.3.0a2-win-x64-net48-portable.zip
C3-v1.3.0a2-win-arm64-net481-portable.zip
SHA256SUMS.txt
```

Every ZIP is restricted to the classic WinForms EXE, its matching config,
`README.txt`, `RELEASE_NOTES.txt`, and `BUILD.txt`. The release label and stage
are bound through source constants, assembly informational/product metadata,
the lane manifest, filenames, `BUILD.txt`, entry manifests, checksums, and the
eventual annotated tag.

## What Alpha 2 is intended to prove

- Exactly three source-identical lanes: `win-x86-net40`, `win-x64-net48`, and
  native `win-arm64-net481`.
- Exact serviced Visual Studio, MSBuild, VB compiler, reference-assembly, and
  resource-tool authority captured in one external source-bound lock.
- Genuine x86, x64, and `0xaa64` ARM64 binaries with their exact framework and
  CorFlags contracts.
- One clean pushed source, one immutable lock, and final
  source/ref/submodule/genome/lock closure across the complete package set.
- Deterministic five-entry portable ZIPs reproduced from clean path-distinct
  builds and authenticated by retained entry manifests.
- Builder launch smoke for x86 and x64. ARM64 execution remains deferred until
  the exact native Windows-on-ARM qualification environment is available.

## Current gate status

Repository-side Alpha identity and package projection are implemented. Alpha 2
is not yet tagged or distributed because the installed Visual Studio releases
remain below the declared servicing floors:

```text
VS2017 >= 15.9.81
VS2022 >= 17.14.37
VS2026 >= 18.8.2
```

Administrator servicing, fresh Preparation evidence, external lock capture,
the live Candidate build, package reproduction, smoke, and final Alpha evidence
remain required. Old preparation outputs do not qualify as Alpha 2 bytes.

## Explicit deferrals

Alpha 2 is an engineering preview, not Beta qualification. It does not claim:

- historical Gate 1 completion;
- any lifecycle, persistence, catalogue, settings, counter, or diagnostic repair;
- minimum-operating-system qualification on XP, Windows 7, or Windows 11 ARM64;
- native ARM64 runtime execution;
- public GitHub publication;
- update-feed promotion; or
- movement of `legacy/1.x`.

The inherited recursive close/cancellation defect remains recorded and is not
an Alpha blocker. It is assigned to the first lifecycle repair wave after
historical Gate 1.

## Release authority

The owner authorizes the deliberate `v1.3.0a2` annotated tag and retained
Alpha-labelled packages after the Alpha 2 checks pass. That authority does not
authorize a public GitHub release.

Explicit human approval remains mandatory before any `v1.3.0b1` tag,
`1.3.0b1`-labelled package, retained Beta ZIP, or public Beta prerelease is
created. Stable publication, feed promotion, and `legacy/1.x` advancement remain
behind their separate stable gates.

## Historical Alpha 1 boundary

The immutable `v1.3.0a1` checkpoint remains the source-only recovered
maintenance foundation. Its four diagnostic x86/x64 lanes remain truthful
historical evidence; the later owner decision superseded that matrix with the
three lanes above without rewriting Alpha 1.

## Update feed

The repository-root `VERSION` feed intentionally remains the available public
release:

```text
1.2.0
Release
14/05/2026
```

See `docs/planning/1.3.0-alpha.2.md`, the qualification matrix, and `TODO.md` for
the exact Alpha 2 production gate and later work.
