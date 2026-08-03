# Repository layout and ownership

The repository is shallow at the root and organized by ownership. A project
enforces a dependency boundary; a feature folder keeps files that change together
adjacent. Target-state paths are not added until code actually needs them.

```text
/
|-- C3.sln
|-- README.md
|-- CHANGELOG.md
|-- RELEASE_NOTES.md
|-- ROADMAP.md
|-- TODO.md                         # pointer, not a second backlog
|-- CONTRIBUTING.md
|-- SECURITY.md
|-- SUPPORT.md
|-- CODE_OF_CONDUCT.md
|-- VERSION                         # legacy 1.x update compatibility feed
|-- .github/
|   |-- workflows/
|   `-- ISSUE_TEMPLATE/
|-- src/
|   |-- C3.Catalogue/
|   |   |-- Catalogues/
|   |   |-- Brands/
|   |   |-- CassetteModels/
|   |   |-- Decks/
|   |   `-- Tapes/
|   |-- C3.Infrastructure/
|   |   |-- CatalogueFiles/Xml/V1_1/
|   |   |-- Diagnostics/
|   |   |-- Preferences/
|   |   `-- Updates/
|   |-- C3.WinForms/
|   |   |-- C3.WinForms.Net40.vbproj
|   |   |-- C3.WinForms.Net48.vbproj
|   |   |-- Bootstrap/
|   |   |-- Configuration/
|   |   |-- Features/
|   |   |-- Generated/
|   |   |-- Runtime/
|   |   |-- Shell/
|   |   |-- State/
|   |   `-- My Project/
|   `-- Shared/Generated/            # linked generated assembly identity
|-- tests/C3.Characterization/
|-- fixtures/
|   |-- catalogues/v1.1.0/
|   |   |-- valid/
|   |   |-- invalid/
|   |   |-- security/
|   |   `-- cultures/
|   `-- settings/legacy/
|-- spec/catalogue/
|   |-- v1.1.0/                      # implemented public contract
|   `-- v2.0.0/                      # explicitly unimplemented design draft
|-- spec/preferences/v1/             # implemented shared profile contract
|-- build/
|-- release/
|   |-- catalog.v1.json              # machine lifecycle and artifact identity
|   |-- feeds/
|   `-- validation/
|-- docs/
|   |-- product/
|   |-- governance/
|   |-- compatibility/
|   |-- architecture/
|   |-- migration/
|   |-- ui/
|   |-- integrations/
|   |-- planning/
|   |-- development/
|   `-- user/
|-- assets/
|   |-- README.md
|   |-- branding/
|   |-- design/
|   |-- reference/
|   `-- screenshots/
`-- artifacts/                       # ignored generated output
```

`bin` and `obj` directories may appear below projects but are ignored and are
never architectural owners. Empty speculative `future`, language, platform, or
plugin directories are not committed.

## Projects by dependency, folders by feature

`C3.Catalogue` contains framework-4.0-compatible product concepts, commands,
rules, and results. It cannot reference files, XML, `DataSet`, WinForms, concrete
settings, networking, or OS APIs.

`C3.Infrastructure` implements external mechanisms. Its versioned legacy XML
directory is the only production code allowed to know v1.1 table/column names.
Preference persistence/import, update policy, and diagnostics live beside their
mechanism rather than in a catch-all platform module.

`C3.WinForms` composes services and owns interaction and presentation. Its two
project files compile the same physical feature sources. The Net40 project is the
authoritative current designer owner. Generated shared version attributes are
linked into every managed project from `src/Shared`; no hand-written business
code belongs there.

Avoid global `Entities`, `Services`, `Validators`, `Helpers`, and `Managers`
directories. Put a small validator/result/interface beside the feature it serves;
extract a shared abstraction only after more than one real owner needs it.

## Source-of-truth map

| Fact or behavior | Canonical owner | Projection or evidence |
| --- | --- | --- |
| Current development product/stage/channel/feed/assembly identity | `build/Version.props` | generated BuildInfo, shared assembly attributes, alpha feed, client endpoint, binary/package names |
| Published legacy updater value | `release/feeds/legacy-1x/VERSION` | root `VERSION`; deliberately independent from current build |
| Update-channel policy and branch contract | `docs/governance/versioning-and-channels.md` | release scripts and feed directories |
| Checkpoint lifecycle and artifact identity | `release/catalog.v1.json` | validation records and promotion/tag checks |
| Active build lanes | `build/lanes.json` | projects, scripts, CI, package names |
| Catalogue format 1.1.0 | `spec/catalogue/v1.1.0` | legacy XML adapter and fixtures |
| Native-v2 format status | `spec/catalogue/v2.0.0/README.md` and ADR 0005 | no production projection while draft |
| 1.x compatibility promise | `docs/compatibility/1x-to-2x-charter.md` | corpus, tests, candidate validation |
| XML table/column mapping | `C3.Infrastructure/CatalogueFiles/Xml/V1_1` | characterization tests |
| Document path/revision/dirty state | `CatalogueSession` | forms observe session state |
| Brand/model/deck/tape rules | matching `C3.Catalogue` feature | typed results and adapter tests |
| Native preference lifecycle and dirty state | `UserPreferencesService` | store/importer characterization |
| Preference format v1 | `spec/preferences/v1` | `XmlUserPreferencesStore`, canonical example test |
| C3 1.x preference discovery/import | `LegacyUserSettingsImporter` | `fixtures/settings`, path/reader tests |
| UI mutation refresh | `CatalogueUiCoordinator` | owning main window |
| Runtime capability | `Runtime/RuntimeInfo.vb` | About and diagnostics |
| Durable product doctrine/scope | product vision and 2.0 scope docs | README summary |
| UI design contract | `docs/ui/oem-plus-design.md` | forms and manual evidence |
| Programme dependency order | `docs/planning/2.0-execution-plan.md` | ROADMAP summary; issues/AIDE assignments |
| Current candidate narrative | `RELEASE_NOTES.md` | local alpha notes or stage-dependent GitHub release draft |
| Historical product change | `CHANGELOG.md` | tagged history |
| Exact candidate proof | `release/validation/<version>.md` | commands, CI/VM runs, hashes |
| AIDE/Universal Setup boundaries | their integration documents | future pinned bindings/evidence |
| Asset purpose and provenance | `assets/README.md` | files below `assets/` |

If two files appear to own the same fact, one becomes a generated projection or
validator, or is removed. README files summarize and link; they do not redefine
the underlying contract.

## Allowed build-lane differences

The two WinForms projects may differ only in target framework/CPU, conditional
constants, app configuration, manifest, target-specific runtime behavior, output
paths, and genuinely framework-specific references. Forms, designer files,
feature logic, settings schema/migration, catalogue behavior, generated assembly
identity, and user documentation remain shared.
