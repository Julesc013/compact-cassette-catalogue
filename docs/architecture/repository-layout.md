# Repository Layout

The target layout is intentionally compact. Directories are added when their
contents exist; empty placeholders for speculative ports, protocols, or format
versions are not committed.

```text
/
|-- C3.sln
|-- src/
|   |-- C3.Catalogue/
|   |   |-- C3.Catalogue.vbproj
|   |   |-- Catalogues/
|   |   |-- Brands/
|   |   |-- CassetteModels/
|   |   |-- Tapes/
|   |   |-- Decks/
|   |   |-- Search/
|   |   `-- Results/
|   |-- C3.Infrastructure/
|   |   |-- C3.Infrastructure.vbproj
|   |   |-- CatalogueFiles/
|   |   |   `-- Xml/V1_1/
|   |   |-- Diagnostics/
|   |   `-- Updates/
|   `-- C3.WinForms/
|       |-- C3.WinForms.Net40.vbproj
|       |-- C3.WinForms.Net48.vbproj
|       |-- Bootstrap/
|       |-- Shell/
|       |-- Features/
|       |-- State/
|       |-- Adapters/
|       |-- Runtime/
|       |-- Configuration/
|       |-- Resources/
|       `-- My Project/
|-- tests/
|   |-- C3.Tests/
|   `-- C3.Smoke/
|-- fixtures/
|   `-- catalogues/v1.1.0/
|       |-- valid/
|       |-- invalid/
|       |-- security/
|       |-- cultures/
|       `-- expected/
|-- spec/
|   `-- catalogue/v1.1.0/
|-- build/
|-- packaging/
|-- release/
|-- docs/
|   |-- architecture/
|   |-- development/
|   `-- user/
|-- assets/
|   |-- branding/
|   `-- design/
`-- artifacts/                 # ignored build output
```

## Projects by dependency, folders by feature

A project exists to enforce a compile-time dependency boundary. A folder exists
to keep the files required to understand one feature next to each other.

For example, `Features/Tapes` may contain `TapeListForm`, `TapeEditorControl`, and
`TapeEditorDialog` because those files change together. Domain types for tapes
remain in `C3.Catalogue/Tapes` because the UI depends on them, not vice versa.

Avoid separating every feature into global `Entities`, `Services`, `Validators`,
and `Presenters` directories. That layout makes one change require navigation
across the entire tree and encourages duplicate orchestration.

## Source-of-truth map

| Fact or behavior | Canonical owner | Projections or evidence |
| --- | --- | --- |
| Product/build version | `build/Version.props` | assembly metadata, `VERSION`, artifact names |
| Active build lanes | `build/lanes.json` | projects, CI matrix, package names |
| Catalogue format v1.1 | `spec/catalogue/v1.1.0` | XML adapter and golden fixtures |
| Document path and dirty state | `CatalogueSession` | title bar and commands observe it |
| View selections and filters | `WorkspaceState` | forms and persisted UI settings |
| Application settings | typed settings interface | `MySettingsStore` adapter |
| Brand/model/tape/deck rules | matching feature in `C3.Catalogue` | forms display typed failures |
| Runtime capability | lane-specific runtime adapter | About/Diagnostics display it |
| Release narrative | versioned release notes | GitHub release text is generated/copied |
| Release proof | versioned validation record | CI and local verification outputs |
| Branding source | `assets/branding` | linked/copied runtime resources |

If two files appear to own the same fact, one must become generated, validated,
or removed.

## Allowed lane differences

The two WinForms projects may differ only in:

- target framework and CPU architecture;
- conditional constants;
- app configuration and manifest;
- target-specific runtime implementation;
- output and intermediate paths; and
- references required exclusively by the target framework.

Form code, designer files, resources, feature logic, settings schema, and
catalogue behavior are shared. The Net40 project remains the authoritative
WinForms designer owner while 1.x supports .NET Framework 4.0. Conditional
compilation is forbidden in `*.Designer.vb`.

