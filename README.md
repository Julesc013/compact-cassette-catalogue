# Compact Cassette Catalogue (C3)

C3 is an offline-first native Windows catalogue for compact cassettes, their
recordings, cassette models, brands, and tape decks. Catalogues remain ordinary,
inspectable local files that users can copy, back up, and move between supported
builds.

> **Development status:** C3 2.0.0 Alpha 1 is active on `dev`. Its source is
> visible, but its binaries, GitHub release, and update feed are intentionally
> unpublished. C3 1.2 Beta maintenance is preserved on `maintenance/1.x`.

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

Download published C3 builds only from the
[GitHub releases page](https://github.com/Julesc013/compact-cassette-catalogue/releases).
A branch, tag, package filename, or development feed does not by itself mean that
a build has been published.

C3 remains one product with two portable build lanes:

| Lane | Runtime | Compatibility purpose |
| --- | --- | --- |
| `win-x86-net40` | x86, .NET Framework 4.0 | Windows XP SP3 compatibility lane |
| `win-x64-net48` | x64, .NET Framework 4.8 | Windows 7 SP1+ 64-bit lane |

The x86 package is the conservative compatibility choice, including on newer
64-bit Windows. Minimum-OS and DPI claims are published only after the exact
candidate passes its recorded runtime matrix.

Portable means no installer and no administrator requirement. Both 2.0 lanes
share the C3-owned profile at
`%LOCALAPPDATA%\Jules Carboni\C3\2\preferences.xml`; diagnostics also use Windows
application-data locations. A future portable-profile mode will be named
explicitly rather than implied. C3 1.x retains its historical per-user settings
behavior on the maintenance line.

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
optional; use the releases page manually if an older Windows installation cannot
negotiate GitHub's current HTTPS requirements. Never weaken TLS validation.
The [project wiki](https://github.com/Julesc013/compact-cassette-catalogue/wiki)
contains the current user guide.

## What C3 records

- cassette brand, model, type, year, length, region, and condition;
- per-side recording date, deck, input, levels, noise reduction, speed, bias,
  equalization, contents, artist, and title; and
- tape-deck capabilities and technical specifications.

## Repository map

```text
src/C3.Catalogue       catalogue concepts, rules, commands, and session semantics
src/C3.Infrastructure  versioned XML, preferences, diagnostics, external adapters
src/C3.WinForms        shared native UI and two compatibility-lane projects
src/Shared             generated identity source linked into managed assemblies
tests                  executable compatibility characterization
fixtures               privacy-safe valid, invalid, culture, and security examples
spec                    catalogue and preference contracts plus design drafts
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

Visual Studio 2017 Enterprise 15.9 MSBuild is the canonical current compiler for
both 2.0 lanes. It targets .NET Framework 4.0 and 4.8 from one reproducible build
contract. Visual Studio 2010 is a historical designer/reference environment;
Visual Studio 2022 and 2026 support editing, analysis, and forward experiments
but cannot prove the net40 lane.

Historical maintenance facts remain explicit: C3 1.1.2 used the Visual Studio
2019/.NET Framework 4.6 line, while C3 1.2.0 Beta 1 used its Visual Studio 2015
project/toolchain line with the required targeting packs. Those facts do not
change the canonical 2.0 compiler.

From Windows PowerShell:

```powershell
.\build\verify.ps1 -Rebuild
.\build\package.ps1 -SkipBuild
# Required for a checkpoint candidate; builds two clean, path-distinct snapshots.
.\build\verify-reproducible-packages.ps1
```

See the [toolchain policy](docs/development/toolchain.md),
[building instructions](docs/development/building.md),
[continuous-integration policy](docs/development/continuous-integration.md), and
[contribution guide](CONTRIBUTING.md).

## Branches, checkpoints, and publication

- `maintenance/1.x`: bounded C3 1.x maintenance and compatibility-feed fixes.
- `master`: append-only ledger of the latest qualified product checkpoint.
- `dev`: active, unqualified work toward the next checkpoint.
- `feature/*`, `fix/*`, and temporary `candidate/*`: short-lived branches targeting
  the appropriate permanent line.

Every promoted alpha, beta, release candidate, and stable tag is reachable from
`master`. Alpha checkpoints are tagged but intentionally unpublished. Beta tags
require owner manual qualification before a public GitHub prerelease. Stable
promotes unchanged qualified release-candidate payloads. Branch or tag presence
never substitutes for publication status; see
[versioning and channels](docs/governance/versioning-and-channels.md).

Applicable 1.x fixes flow forward into `dev`; 2.x-only changes never flow into
`maintenance/1.x`.

- [Current candidate notes](RELEASE_NOTES.md)
- [Changelog](CHANGELOG.md)
- [Roadmap](ROADMAP.md)
- [Support](SUPPORT.md)
- [Security policy](SECURITY.md)

Copyright © 2019–2026 Jules Carboni.
