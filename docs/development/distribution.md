# C3 distribution doctrine

This document owns C3's distribution vocabulary, supported delivery surfaces,
artifact grammar, and relationship with external setup tooling. Machine-readable
lane and payload facts remain in `build/lanes.json` and `release/profiles`; this
document explains their product contract without duplicating their file lists.

## One product, several components

The product is **Compact Cassette Catalogue (C3)**. Its stable machine identity is
`c3`, its publisher identity is `Jules Carboni`, and its repository identity is
`compact-cassette-catalogue`. C3 Desktop and C3 CLI are components of the same
product and always carry the same product, file, assembly, and informational
release identity.

| Component | Canonical owner | Delivery status |
| --- | --- | --- |
| C3 Desktop | `src/C3.WinForms` | required in both portable lanes |
| C3 CLI (`c3.exe`) | `src/C3.Cli` | required in both portable lanes |
| Domain and catalogue behavior | `src/C3.Domain`, `src/C3.Catalogue` | shared product implementation |
| External mechanisms | `src/C3.Infrastructure` | shared product implementation |
| Shared desktop presentation | `src/C3.Presentation.WinForms` | required in both portable lanes |
| Portable payload definition | `release/profiles/portable-payload.v1.json` | authoritative |
| Universal Setup binding | not yet created | planned, gated on a stable external schema |
| Packs and executable extensions | not yet created | future, separately versioned contracts |

There is no Classic or Modern edition. Runtime lanes are compatibility targets,
not products, feature tiers, or forks.

## Distribution-status vocabulary

Every delivery surface uses one of these terms:

- `supported`: qualified for public use under its recorded matrix;
- `preview`: publicly available beta or release-candidate behavior;
- `internal`: implemented for an intentionally unpublished alpha;
- `planned`: accepted direction without a distributable implementation;
- `experimental`: explicitly non-production investigation;
- `reserved`: a name or compatibility space protected for later design;
- `community-maintained`: supported through community effort without a service
  guarantee;
- `archive-only`: retained for reconstruction or rollback, not active use; and
- `unsupported`: outside the product contract.

The profile validator derives each implemented portable lane's status from the
release channel: alpha is `internal`, beta/RC is `preview`, and stable is
`supported`. Prose cannot promote an artifact to a stronger status.

## Current delivery matrix

| Profile | Architecture/runtime | Current contract |
| --- | --- | --- |
| `win-x86-net40-portable` | x86 / .NET Framework 4.0 | implemented; XP SP3 compatibility lane, subject to candidate evidence |
| `win-x64-net48-portable` | x64 / .NET Framework 4.8 | implemented; Windows 7 SP1+ lane, subject to candidate evidence |
| Universal Setup | consumes an exact staged lane | planned; no guessed binding exists |

ARM64, WinUI, macOS, Linux, newer managed-runtime lanes, web delivery, and other
native shells are reserved product directions only. They do not gain source
directories, build profiles, downloads, or compatibility claims until an
accepted design and executable gate exist.

## Artifact grammar

Portable archives use:

```text
C3-v<product-version>-<os>-<arch>-<runtime>-<delivery>.<ext>
```

The current lane IDs already encode `<os>-<arch>-<runtime>`, producing names such
as:

```text
C3-v2.0.0-beta.1-win-x86-net40-portable.zip
C3-v2.0.0-beta.1-win-x64-net48-portable.zip
SHA256SUMS.txt
```

Machine identifiers use lowercase kebab-case. Manifest paths use `/`. SHA-256
values are lowercase hexadecimal. The CLI executable is always `c3.exe`; its
project and namespace are `src/C3.Cli/C3.Cli.csproj` and `C3.Cli`.

## Authoritative portable payload

Portable ZIP is the complete authoritative C3 distribution. It requires no
installer, elevation, or administrator account and can be copied to another
supported computer. “Portable” does not mean that preferences, diagnostics, or
other user state live beside the executable; C3 uses its documented per-user
application-data locations unless a future explicit portable-profile contract is
accepted.

Each ZIP has exactly one versioned root:

```text
C3-v<release-label>-<lane>-portable/
```

The root's exact entries come only from
`release/profiles/portable-payload.v1.json`. Today that contract includes the
Desktop executable/configuration, C3 managed assemblies, `c3.exe`, build
identity, README, and release notes. Packaging and setup tooling must consume the
manifest; they may not maintain another list. A loose application executable is
not a C3 distribution.

PDBs and compiler documentation remain build artifacts. Public symbols,
machine-readable release manifests, an SBOM, provenance statements, and
signatures may be added by their stage-specific contracts without changing the
portable payload owner. `LICENSE` and third-party notices enter packages only
after the repository owner accepts the project licence and the applicable notice
inventory.

## Setup boundary

Universal Setup is optional. It may become a public C3 delivery only after its
stable versioned binding schema can consume the exact staged portable tree and
hashes. It must not rebuild C3, parse a ZIP as a substitute for product facts,
or carry a second payload enumeration.

An accepted binding must define per-user/per-machine scope, install locations,
side-by-side 1.x/2.x and channel identities, x86/x64 transition, opt-in shell
associations, verify/repair/rollback/uninstall ownership, interruption recovery,
and separation from catalogue and user-profile data. Installer rollback never
reverses catalogue migration. Until those gates pass, portable remains the
complete distribution and no speculative binding file is committed.

C3 does not produce MSI, MSIX, ClickOnce, a web installer, an automatic updater,
or an HTTP fallback as alternate owners of the product payload.

## Publication is independent from branches

The four permanent branches carry source history. Channels carry availability:

- alpha: qualified immutable tag, intentionally unpublished binaries and feed;
- beta: owner-qualified GitHub prerelease, downloaded-asset verification, beta
  feed promoted last;
- release candidate: public prerelease under the beta channel and the complete
  stable-readiness matrix; and
- stable: newly stable identity built and requalified from the accepted RC source,
  public release, downloaded verification, stable feed promoted last.

See [versioning and channels](../governance/versioning-and-channels.md) and
[ADR 0010](../architecture/decisions/0010-stable-release-identity.md). A branch,
tag, filename, or generated manifest never proves publication by itself.

## Evidence layers

A distributable checkpoint is supported only when the applicable layers agree:

1. clean, committed source and canonical version metadata;
2. both implemented build lanes and exact binary/PE identities;
3. behavior, compatibility, security, preference, and migration suites;
4. one manifest-derived staged payload per lane;
5. two clean, path-distinct builds with identical archives and checksums;
6. stage-required manual OS, workflow, DPI, and accessibility evidence; and
7. for public releases, immutable uploaded assets, clean re-download, rehash,
   launch, and feed promotion last.

Signing never substitutes for hashing, reproducibility, or post-download checks.
A signing policy, keys, and public verification workflow require an explicit
owner decision before C3 claims signed distribution.

## Product horizons

- 2.0 owns both portable lanes, the minimal CLI, native/legacy catalogue paths,
  deterministic migration/export, the OEM+ workspace, and release evidence.
- 2.1 may add declarative packs, custom fields, saved views, richer import/export,
  and media/J-card foundations through versioned contracts.
- 2.2 may add measured indexing, search, and analytics improvements.
- 2.3 may expand CLI automation and introduce a separately accepted isolated
  extension protocol.
- 2.4 may mature Universal Setup and evaluate ARM64 only after evidence warrants
  another lane.
- 3.x and later may add alternate shells while preserving the language-neutral
  catalogue and behavior contracts.

Future horizons reserve vocabulary; they do not create present support claims.
