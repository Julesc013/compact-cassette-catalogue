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
|   |-- C3.Domain/
|   |   |-- Commands/
|   |   |-- Identity/
|   |   |-- Time/
|   |   |-- Validation/
|   |   `-- Values/
|   |-- C3.Catalogue/
|   |   |-- C3.Catalogue.csproj
|   |   |-- Catalogues/
|   |   |-- Brands/
|   |   |-- CassetteModels/
|   |   |-- Decks/
|   |   `-- Tapes/
|   |-- C3.Infrastructure/
|   |   |-- C3.Infrastructure.csproj
|   |   |-- CatalogueFiles/Xml/V1_1/
|   |   |-- Diagnostics/
|   |   |-- FileOperations/
|   |   |-- Preferences/
|   |   `-- Updates/
|   |-- C3.Cli/
|   |   |-- C3.Cli.csproj
|   |   `-- Program.cs
|   |-- C3.Presentation.WinForms/
|   |   |-- C3.Presentation.WinForms.csproj
|   |   |-- Workspace/
|   |   |-- Interaction/
|   |   `-- Features/
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
|   |-- compatibility/1x/             # canonical 1.x producer/artifact corpus
|   |-- catalogues/v1.1.0/
|   |   |-- valid/
|   |   |-- invalid/
|   |   |-- security/
|   |   |-- cultures/
|   |   `-- historical/                # normalized official-writer fixtures
|   `-- settings/legacy/
|-- spec/catalogue/
|   |-- v1.1.0/                      # implemented public contract
|   `-- v2.0.0/                      # implemented Alpha 4 candidate profile
|-- spec/catalogue-api/v1/           # frozen compiled catalogue surface
|-- spec/distribution/v1/            # portable profile/payload schemas
|-- spec/preferences/v1/             # implemented shared profile contract
|-- spec/release-catalog/v1/          # checkpoint lifecycle/artifact schema
|-- spec/release-train/
|   |-- v1/                           # preserved Alpha 1-6 controller schema
|   `-- v2/                           # active Alpha 1-12 controller schema
|-- spec/update-feed/v1/              # bounded 2.x release-manifest contract
|-- build/
|-- release/
|   |-- catalog.v1.json              # machine lifecycle and artifact identity
|   |-- feeds/
|   |-- profiles/                    # implemented lane bindings + one payload list
|   |-- train/                        # current programme pointer and order
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

`C3.Domain` is the dependency-free C# 7.3 target for native 2.0 identity,
commands, results, change sets, undo, and migrated aggregates. Shared concepts
are introduced there once; its directories correspond to real semantic owners,
not generic convenience code.

`C3.Catalogue` contains framework-4.0-compatible legacy product concepts, commands,
rules, and results. It cannot reference files, XML, `DataSet`, WinForms, concrete
settings, networking, or OS APIs. It may reference only `C3.Domain`. During
Alpha 3 its unmigrated features remain production owners and differential
oracles; a frozen public facade may delegate to one proven C# owner without
retaining duplicate behavior.

`C3.Catalogue` is explicit C# 7.3. Its completed Alpha 3 port preserved the
original project GUID, assembly identity, and frozen public surface; the
side-by-side candidate harness and superseded VB implementations no longer exist.

`C3.Infrastructure` implements external mechanisms. Its versioned legacy XML
directory is the only production code allowed to know v1.1 table/column names.
Preference persistence/import, update policy, and diagnostics live beside their
mechanism rather than in a catch-all platform module. `FileOperations` contains
only reusable, same-volume transaction primitives required by more than one
persistence adapter; it does not own catalogue or preference policy. The
completed C# 7.3 port preserved the original project GUID, binary identity,
net40 contract, and all 312 frozen public signatures; the candidate and VB
implementations no longer exist.

`C3.Cli` is the canonical net40/AnyCPU headless component. Its only production
source owns argument parsing, help text, exit-code projection, and composition.
All catalogue reading, validation, migration, recovery, and legacy export remain
owned by the same Infrastructure services used by other product surfaces.

`C3.Presentation.WinForms` owns shared desktop workspace state, reusable
interaction presentations, semantic UI commands, presenters, and WinForms
controls. `Interaction` contains the deliberately small field, validation,
list, inspector, empty-state, feedback, and progress vocabulary. Feature folders
compose those patterns around `C3.Catalogue` services; they cannot create another
catalogue rule or persistence path.

`C3.Presentation.WinForms` is the shared C# 7.3, net40-compatible presentation
boundary accepted in ADR 0011. It owns workspace projections, presenter-level
state, provisional Alpha 5 command-history coordination, and reusable WinForms patterns for both
lane hosts. It may depend on Catalogue and Domain, but never on Infrastructure,
XML, files, `DataSet`, concrete settings, or update transport. Feature folders
are created only as workflows enter the production path.

After Alpha 5, ADR 0012 first converges the complete logical catalogue inside
`C3.Catalogue`. `src/C3.Application/` is added only when Alpha 7 needs the
enforceable lifecycle boundary; it is not created early as an empty architecture
placeholder. Once present, presentation and CLI project operations through
Application, while Infrastructure implements inward ports. Presentation retains
selection, focus, draft controls, and layout state but no longer owns semantic
catalogue history or calls feature services directly.

`C3.WinForms` owns the lane executables, startup composition, runtime-edge policy,
and the remaining legacy forms until their replacement gates pass. Brands is
already owned by the shared C# presentation boundary. Its two
project files compile the same physical host sources. The Net40 project is the
authoritative current designer owner. Generated shared version attributes are
linked into every managed project from `src/Shared`; no hand-written business
code belongs there.

Avoid global `Entities`, `Services`, `Validators`, `Helpers`, and `Managers`
directories. Put a small validator/result/interface beside the feature it serves;
extract a shared abstraction only after more than one real owner needs it.

## Source-of-truth map

| Fact or behavior | Canonical owner | Projection or evidence |
| --- | --- | --- |
| Current development product/stage/channel/assembly identity | `build/Version.props` | generated BuildInfo, shared assembly attributes, unpublished alpha manifest/client endpoint, binary/package names |
| Published legacy updater value | `release/feeds/legacy-1x/VERSION` | root `VERSION`; deliberately independent from current build |
| Permanent branch identities | `build/branches.json` | schema, validator, release scripts, CI, generated updater allow-list, and governance prose |
| Supported 1.x producers and historical artifact/format provenance | `fixtures/compatibility/1x/corpus.v1.json` | schema, corpus validator, exact-binary harness, normalized fixtures, and compatibility documentation |
| Update-channel and promotion policy | `docs/governance/versioning-and-channels.md` | release scripts and feed directories |
| Checkpoint lifecycle and artifact identity | `release/catalog.v1.json` | validation records and C/E/P promotion checks |
| Current programme milestone and order | `release/train/2.0.0.json` | train validator and transition scripts |
| Active train schema | `spec/release-train/v2` | preserves v1 unchanged; validator and tests enforce Alpha 1-12 then Beta 1 |
| 2.x update manifest syntax and publication shape | `spec/update-feed/v1` | generated candidates, promoted channel feeds, and bounded runtime reader |
| Published 2.x beta/stable availability | matching channel `release.json`, changed only by successful public `P` | catalogue and validation evidence; release candidates use beta |
| Active build lanes | `build/lanes.json` | projects, scripts, CI, package names |
| Implemented distribution profiles and exact portable file list | `release/profiles` | `spec/distribution/v1`, validator, staging, ZIP packaging, and future setup binding |
| Catalogue format 1.1.0 | `spec/catalogue/v1.1.0` | legacy XML adapter and fixtures |
| Native domain identity, time, validation, command/change-set, and undo semantics | `C3.Domain` and ADR 0009 | characterization/property tests; migrated aggregate slices |
| Current catalogue-library compiled public surface | `spec/catalogue-api/v1/public-api.txt` | reflection validator after every characterization build; semantics remain in source/tests |
| Current infrastructure-library compiled public surface | `spec/infrastructure-api/v1/public-api.txt` | reflection validator after every characterization build; external semantics remain in source/tests |
| Native-v2 format status | `spec/catalogue/v2.0.0/README.md` and ADR 0005 | implemented Alpha 4 candidate; qualification controls public support |
| Post-Alpha-5 canonical catalogue and Application boundary | `docs/architecture/catalogue-and-application.md` and ADR 0012 | Alpha 6-12 execution gates and future architecture fitness checks |
| 1.x compatibility policy | `docs/compatibility/1x-to-2x-charter.md` | machine corpus, tests, and candidate validation; prose cannot widen the corpus |
| XML table/column mapping | `C3.Infrastructure/CatalogueFiles/Xml/V1_1` | characterization tests |
| Persisted catalogue revision identity | `C3.Domain.Catalogues.CatalogueRevision` | frozen `C3.Catalogue` compatibility facade and differential characterization |
| Document path/dirty state | `C3.Domain.Catalogues.CatalogueSession` | frozen `C3.Catalogue` compatibility facade; forms observe session state |
| Process object graph and sole legacy `DataSet` adapter seam | `C3.WinForms/Bootstrap/ApplicationComposition.vb` | boundary validator; repositories receive the instance-owned document provider |
| Brand/model/deck/tape rules | matching `C3.Catalogue` feature | typed results and adapter tests |
| Native preference lifecycle and dirty state | `UserPreferencesService` | store/importer characterization |
| Preference format v1 | `spec/preferences/v1` | `XmlUserPreferencesStore`, canonical example test |
| C3 1.x preference discovery/import | `LegacyUserSettingsImporter` | `fixtures/settings`, path/reader tests |
| Legacy UI mutation refresh and history invalidation | `CatalogueUiCoordinator` | owning main window; removed as workflows adopt reversible commands |
| Workspace, selection, drafts, command history, and shared interaction patterns | `C3.Presentation.WinForms` and ADR 0011 | presenter/controller tests and both-lane workflow evidence |
| Permanent document lifecycle, history, recovery, operations, and allowed actions | future `C3.Application` after Alpha 7 gate | language-neutral lifecycle/operation fixtures and frontend conformance |
| Runtime capability | `Runtime/RuntimeInfo.vb` | About and diagnostics |
| Durable product doctrine/scope | product vision and 2.0 scope docs | README summary |
| UI design contract | `docs/ui/oem-plus-design.md` and shared interaction patterns | feature forms, presenter tests, and manual evidence |
| Brands interaction workflow | `C3.Presentation.WinForms/Features/Brands` | one shared form/presenter/command set, both host launch points, characterization, and Alpha 5 UI evidence |
| Programme dependency order | `docs/planning/2.0-execution-plan.md` | ROADMAP summary; issues/AIDE assignments |
| Current candidate narrative | `RELEASE_NOTES.md` | local alpha notes or stage-dependent GitHub release draft |
| Historical product change | `CHANGELOG.md` | tagged history |
| Exact candidate proof | `release/validation/<version>.md` | commands, CI/VM runs, hashes |
| AIDE/Universal Setup boundaries | their integration documents | future pinned bindings/evidence |
| Distribution vocabulary, artifact grammar, and delivery horizons | `docs/development/distribution.md` | README, packaging, roadmap, and profile status projections |
| Asset purpose and provenance | `assets/README.md` | files below `assets/` |

If two files appear to own the same fact, one becomes a generated projection or
validator, or is removed. README files summarize and link; they do not redefine
the underlying contract. Three-line `VERSION` exists only at the root and in the
legacy 1.x feed; no 2.x directory gains one.

## Allowed build-lane differences

The two WinForms projects may differ only in target framework/CPU, conditional
constants, app configuration, manifest, target-specific runtime behavior, output
paths, and genuinely framework-specific references. Forms, designer files,
feature logic, settings schema/migration, catalogue behavior, generated assembly
identity, and user documentation remain shared.
