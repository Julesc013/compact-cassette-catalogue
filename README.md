# Compact Cassette Catalogue (C3)

C3 is an offline-first Windows desktop catalogue for blank cassettes, their recordings, cassette models, brands, and tape decks. Catalogues remain ordinary local XML files that users can copy, back up, and move between supported builds.

> C3 1.2.0 Beta 1 is under active stabilization. Keep backups of important catalogues and review the release validation status before relying on a beta.

![C3 main window](assets/screenshots/demonstration-screenshot.png)

## Download

Download C3 only from the [GitHub releases page](https://github.com/Julesc013/compact-cassette-catalogue/releases).

> Portable means that C3 requires no installer or administrator access. In **C3 v1.x**, per-user settings and diagnostics may still use Windows application-data locations; “portable” does not yet mean a completely self-contained profile.

## First use

1. Extract the complete ZIP to a writable folder.
2. Run `Compact Cassette Catalogue.exe`.
3. Create a catalogue, then add a brand, cassette model, and tape.
4. Save the catalogue as XML and keep a separate backup copy.

The [project wiki](https://github.com/Julesc013/compact-cassette-catalogue/wiki) contains the user guide.
C3 does not require a network connection for catalogue work.
> Update checks are optional; use the releases page manually if an older Windows installation cannot negotiate GitHub's current HTTPS requirements.

## What C3 records

- cassette brand, model, type, year, length, region, and condition
- per-side recording date, deck, input, levels, noise reduction, speed, bias, equalization, contents, artist, and title
- tape-deck capabilities and technical specifications

## Repository map

```text
src/C3.Catalogue       catalogue concepts and rules
src/C3.Infrastructure  XML, file-system, and diagnostic adapters
src/C3.WinForms        shared Windows UI and two build projects
tests                  executable compatibility characterization
fixtures               valid, invalid, culture, and security XML examples
spec                   language-neutral catalogue-format contract
build                  build, verification, and packaging automation
docs                   architecture, development, and user documentation
release/validation     versioned release evidence
assets                 canonical branding, design, and screenshot sources
```

Start with the [architecture guide](docs/architecture/README.md), [building instructions](docs/development/building.md), and [contribution guide](CONTRIBUTING.md).
The complete repository tree and ownership map are documented in [docs/architecture/repository-layout.md](docs/architecture/repository-layout.md).

## Build and verify

> The authoritative toolchain for `1.1.2` is Visual Studio 2019 with .NET Framework 4.6 targeting pack for 64 bit only.
> The authoritative toolchain for `1.2.0b1` is Visual Studio 2015 with .NET Framework 4.0 and 4.8 targeting packs.

The authoritative toolchain for future development is **Visual Studio 2017**, **2022**, and **2026** with **.NET Framework 4.0**, **4.8**, and **4.8.1** targeting packs respectively.

From Windows PowerShell:

```powershell
.\build\verify.ps1 -Rebuild
.\build\package.ps1 -SkipBuild
```

The first command runs metadata, dependency, source-parity, regression, build, and executable-architecture checks.
*See [continuous integration](docs/development/continuous-integration.md) for why the full dual-lane build requires a Visual Studio 2019 self-hosted runner.*

## Help, security, and project status

- [Support](SUPPORT.md)
- [Security policy](SECURITY.md)
- [Changelog](CHANGELOG.md)
- [Current release notes](RELEASE_NOTES.md)
- [Roadmap](ROADMAP.md)

Copyright © 2019–2026 Jules Carboni.
