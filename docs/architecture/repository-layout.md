# Repository Layout

The repository is intentionally shallow at the root and organized by ownership.
A project enforces a dependency boundary; a feature folder keeps files that
change together adjacent. Empty directories for speculative ports or protocols
are not committed.

```text
/
|-- C3.sln
|-- README.md
|-- CHANGELOG.md
|-- RELEASE_NOTES.md
|-- ROADMAP.md
|-- CONTRIBUTING.md
|-- SECURITY.md
|-- SUPPORT.md
|-- CODE_OF_CONDUCT.md
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
|   |   `-- Diagnostics/
|   `-- C3.WinForms/
|       |-- C3.WinForms.Net40.vbproj
|       |-- C3.WinForms.Net48.vbproj
|       |-- Bootstrap/
|       |-- Configuration/
|       |-- Features/
|       |-- Runtime/
|       |-- Shell/
|       |-- State/
|       `-- My Project/
|-- tests/C3.Characterization/
|-- fixtures/catalogues/v1.1.0/
|   |-- valid/
|   |-- invalid/
|   |-- security/
|   `-- cultures/
|-- spec/catalogue/v1.1.0/
|-- build/
|-- release/validation/
|-- docs/
|   |-- architecture/
|   |-- development/
|   `-- user/
|-- assets/
|   |-- README.md
|   |-- branding/
|   |-- design/
|   |-- reference/
|   `-- screenshots/
`-- artifacts/                    # ignored generated output
```

`bin` and `obj` directories may appear locally below projects but are ignored and
never architectural owners.

## Projects by dependency, folders by feature

`C3.Catalogue` contains framework-neutral product concepts and rules. It cannot
reference files, XML, `DataSet`, WinForms, settings, or OS APIs.

`C3.Infrastructure` implements external mechanisms. The v1.1 XML directory is
the only production code allowed to know legacy table and column names.

`C3.WinForms` composes services and owns interaction and presentation. Its two
project files compile the same physical UI sources. The Net40 project is the
authoritative Visual Studio designer owner.

Avoid global `Entities`, `Services`, `Validators`, `Helpers`, and `Managers`
directories. A type is placed with the capability whose rule or boundary it
owns. Cross-feature behavior moves down to the catalogue module only when it is
a genuine product rule.

## Source-of-truth map

| Fact or behavior | Canonical owner | Projections or evidence |
| --- | --- | --- |
| Product/build version | `build/Version.props` | assembly metadata, `VERSION`, package names |
| Active build lanes | `build/lanes.json` | project files, scripts, CI, package names |
| Catalogue format 1.1.0 | `spec/catalogue/v1.1.0` | XML adapter and fixtures |
| XML table/column mapping | `C3.Infrastructure/CatalogueFiles/Xml/V1_1` | characterization tests |
| Document path/revision/dirty state | `CatalogueSession` | forms observe session state |
| Brand/model/deck/tape rules | matching `C3.Catalogue` feature | typed results shown by forms |
| UI mutation refresh | `CatalogueUiCoordinator` | owning main window |
| Runtime capability | `Runtime/RuntimeInfo.vb` | About and diagnostics |
| Release narrative | `RELEASE_NOTES.md` and `CHANGELOG.md` | GitHub release text |
| Release proof | `release/validation/<version>.md` | commands, CI, OS runs, hashes |
| Asset purpose and provenance | `assets/README.md` | branding, design sources, screenshots, references |

If two files appear to own the same fact, one must become a generated projection,
a validator, or be removed.

## Allowed build-lane differences

The two WinForms projects may differ only in target framework/CPU, conditional
constants, app configuration, manifest, target-specific runtime behavior, output
paths, and truly framework-specific references. Forms, designer files, feature
logic, settings schema, catalogue behavior, and user documentation remain shared.
