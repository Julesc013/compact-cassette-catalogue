# Compact Cassette Catalogue (C3)

C3 is an offline-first native Windows catalogue for compact cassettes, their
recordings, cassette models, brands, and tape decks. Catalogues remain ordinary,
inspectable local files that users can copy, back up, and move between supported
builds.

> **Development status:** C3 2.0.0 Alpha 1 is being developed on the permanent
> `dev` branch. It is not a published stable release and does not yet implement a
> native-v2 catalogue format. The maintained 1.2 Beta line remains on `master`.

![C3 main window](assets/screenshots/demonstration-screenshot.png)

## Product direction

C3 2.0 is an evolutionary overhaul of the existing product: archival-grade data
safety, typed identity and commands, deterministic migration, recovery and
undo/redo, an OEM+ accessible WinForms workspace, reproducible distribution, and
open language-neutral contracts. It is not a big-bang rewrite.

The managed implementation is moving toward explicit C# 7.3 in proven slices.
The current VB code remains the behavioral oracle until each replacement passes
compatibility and UI parity. C11 belongs to Universal Setup/bootstrap work;
C++11 is reserved for a measured isolated native boundary, never catalogue logic.

Read the [product vision](docs/product/vision.md),
[accepted 2.0 scope](docs/product/c3-2.0-scope.md), and
[execution plan](docs/planning/2.0-execution-plan.md) for the complete contract.

## Downloads and build lanes

Published releases are available only from the
[GitHub releases page](https://github.com/Julesc013/compact-cassette-catalogue/releases).
Do not infer release availability from a development branch or version file.

C3 remains one product with two portable build lanes:

| Lane | Runtime | Compatibility purpose |
| --- | --- | --- |
| `win-x86-net40` | x86, .NET Framework 4.0 | Windows XP SP3 compatibility lane |
| `win-x64-net48` | x64, .NET Framework 4.8 | Windows 7 SP1+ 64-bit lane |

The x86 package is the conservative compatibility choice, including on newer
64-bit Windows. Minimum-OS and DPI claims are published only after the exact
candidate passes its recorded runtime matrix.

Portable means no installer and no administrator requirement. Per-user settings
and diagnostics currently use Windows application-data locations; a future
portable-profile mode will be named explicitly rather than implied.

## Catalogue compatibility

Product and catalogue-format versions are independent. Alpha 1 still reads and
writes the legacy catalogue 1.1.0 profile. A native-v2 format is only a design
draft until its typed model, schema, migration, legacy mode, loss-aware export,
security limits, fixtures, and both implementations pass together.

C3 2.0's full 1.x compatibility target covers catalogues, settings, update
channels, side-by-side use, portable/setup payloads, and rollback. The current
repository has strong v1.1 characterization but not yet a complete corpus of all
public 1.x producers, so compatibility remains an explicit programme gate. See
the [1.x to 2.x charter](docs/compatibility/1x-to-2x-charter.md).

## First use of a published portable build

1. Download a ZIP and `SHA256SUMS.txt` from the same GitHub release.
2. Verify its SHA-256 value, then extract the complete ZIP to a writable folder.
3. Run `Compact Cassette Catalogue.exe`.
4. Create or open a catalogue and keep an independent backup before preview work.

C3 does not require a network connection for catalogue work. Update checks are
optional. Never weaken HTTPS/TLS validation to make an old operating system reach
GitHub.

## Repository map

```text
src/C3.Catalogue       catalogue concepts, rules, commands, and session semantics
src/C3.Infrastructure  versioned XML, settings, diagnostics, and external adapters
src/C3.WinForms        shared native UI and two compatibility-lane projects
src/Shared             generated identity source linked into managed assemblies
tests                  executable compatibility characterization
fixtures               privacy-safe valid, invalid, culture, and security examples
spec                    language-neutral catalogue contracts and design drafts
build                   build, verification, and deterministic packaging automation
release/feeds           independently promoted legacy/preview update metadata
release/validation      immutable evidence for exact release candidates
docs                    product, architecture, design, development, and user contracts
assets                  canonical branding, design, screenshot, and reference sources
```

The [documentation index](docs/README.md) and
[repository ownership map](docs/architecture/repository-layout.md) are canonical.
Do not create parallel `Core`, `Common`, `Helpers`, `Managers`, `Platform`, or
miscellaneous planning trees.

## Build and verify

On the maintained machine, Visual Studio 2017 Enterprise 15.9 MSBuild is the
canonical compiler for both lanes. Visual Studio 2010, 2022, and 2026 have
documented supporting roles; newer IDEs cannot prove the net40 build.

```powershell
.\build\verify.ps1 -Rebuild
.\build\package.ps1 -SkipBuild
```

See the [toolchain policy](docs/development/toolchain.md),
[building instructions](docs/development/building.md), and
[contribution guide](CONTRIBUTING.md).

## Branches and support

- `master`: maintained C3 1.2 Beta/release line until stable 2.0 promotion.
- `dev`: permanent integration line for C3 2.0 and later unreleased work.
- feature/fix branches: short-lived review branches targeting the correct line.

1.x fixes flow forward into `dev`; 2.x changes never flow backward into `master`.
Branch presence is not release publication. See
[versioning and channels](docs/governance/versioning-and-channels.md).

- [Current candidate notes](RELEASE_NOTES.md)
- [Changelog](CHANGELOG.md)
- [Roadmap](ROADMAP.md)
- [Support](SUPPORT.md)
- [Security policy](SECURITY.md)

Copyright © 2019–2026 Jules Carboni.
