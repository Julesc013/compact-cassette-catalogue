# C3 Architecture

C3 is one product with one catalogue contract, one shared source tree, and two
Windows build lanes. The repository is a modular monolith: dependency boundaries
are represented by projects, while related behavior is kept together in feature
folders.

The architecture optimizes for four things, in this order:

1. Never corrupt or silently discard a user's catalogue.
2. Preserve the supported catalogue format and the Windows XP compatibility lane.
3. Keep each behavior in one physical source file wherever possible.
4. Make future capabilities additive and replace implementations by proven slices.

## Production modules

This table describes the implemented Alpha 5 graph. It is not permission to
preserve provisional presentation-owned history or the legacy active `DataSet`
after their replacement gates pass.

| Module | Owns | Must not own |
| --- | --- | --- |
| `C3.Domain` | Native-2.0 opaque identity, UTC/optional values, validation, commands, change sets, undo contracts, and migrated behavior such as persisted revision identity | Legacy keys, files, XML, `DataSet`, WinForms, settings, networking, or OS APIs |
| `C3.Catalogue` | Compatibility-facing catalogue concepts, commands, rules, results, session semantics, store interfaces, and narrow facades over migrated behavior | Files, XML, `DataSet`, WinForms, settings, networking, OS APIs, or a second implementation of migrated rules |
| `C3.Infrastructure` | XML format adapters, atomic file I/O, C3-owned preferences and legacy import, diagnostics, update feeds, and other external-system adapters | Form behavior or business rules |
| `C3.Cli` | Thin argument parsing, exit-code projection, and headless composition over Infrastructure services | A second parser, mapper, validator, migration, or export implementation |
| `C3.Presentation.WinForms` | Shared C# workspace state, presenters, command-history coordination, and reusable WinForms interaction patterns | Infrastructure, XML, files, `DataSet`, concrete settings, update transport, or catalogue rules |
| `C3.WinForms` | Lane executables, startup composition, manifests/configuration, runtime-edge policy, and legacy forms pending proven replacement | Shared workspace rules, persistence parsing, or duplicated domain rules |

After exact Alpha 5 `P`, [ADR 0012](decisions/0012-canonical-catalogue-before-application-frontends.md)
requires whole-document canonical convergence before a new Application layer or
additional frontend becomes a production authority. The accepted target adds
`C3.Application` between frontends and Catalogue, moves lifecycle/history/save
coordination out of presentation, and confines `DataSet`/`Native*` objects to
profile adapters. The complete contract is
[Canonical catalogue and Application architecture](catalogue-and-application.md).

During Alpha 3, named behaviors moved to `C3.Domain` without retaining a second
implementation. Alpha 4 adds the stable-ID native catalogue graph to
`C3.Catalogue.Native`; format syntax and migration remain Infrastructure
concerns. The compiled Catalogue surface is versioned in
[`spec/catalogue-api/v1`](../../spec/catalogue-api/v1/README.md) and checked after
every characterization build; behavior remains protected by the executable
characterization suite.

The complete `C3.Catalogue` production assembly is explicit C# 7.3. Its
mechanical translation was staged in an unpackaged candidate, matched against
all 269 frozen VB signatures, then promoted atomically through the behavior
suite. The candidate project and superseded VB implementations were removed so
there is one production owner.

The complete `C3.Infrastructure` production assembly is also explicit C# 7.3.
Its staged translation matched all 312 signatures under
[`spec/infrastructure-api/v1`](../../spec/infrastructure-api/v1/README.md), then
passed the external-behavior suite through the real application graph. The
candidate and all superseded VB Infrastructure sources were removed so every
external mechanism has one production owner.

`C3.WinForms` has two project files over the same physical UI sources:

- `C3.WinForms.Net40.vbproj`: x86, .NET Framework 4.0, Windows XP SP3+
- `C3.WinForms.Net48.vbproj`: x64, .NET Framework 4.8, Windows 7 SP1+

Only target-specific configuration, manifests, output paths, constants, and
small runtime adapters may differ between those project files.

## Implemented Alpha 5 dependency rule

```text
C3.WinForms.Net40 ----+--> C3.Presentation.WinForms --> C3.Catalogue --> C3.Domain
                      |                                   ^
C3.WinForms.Net48 ----+-----------------------------------+
                      +--> C3.Infrastructure -------------+
C3.Cli -------------------> C3.Infrastructure
```

Dependencies never point toward WinForms. `C3.Catalogue` depends only on APIs
available in .NET Framework 4.0. `C3.Infrastructure` explicitly references both
`C3.Catalogue` and the Domain value types exposed by Catalogue's public native
contracts; neither reference points back toward Infrastructure.

## Interaction rule

```text
Form or control
  -> typed draft or command
  -> catalogue operation
  -> typed result
  -> UI feedback
```

Forms do not pass `DataRow` instances to each other, parse catalogue XML, compute
global counters, construct IDs, or invoke Visual Basic default form instances.

## State ownership

- The following describes the Alpha 5 proof boundary; ADR 0012 deliberately
  replaces its long-term document/history ownership after whole-document
  convergence.
- `CatalogueSession` owns the active catalogue, path, persisted revision,
  dirty state, and document lifecycle events.
- Each list/editor instance owns its temporary selection and filter state. State
  becomes a shared `WorkspaceState` only when more than one view must coordinate
  it or when the user explicitly chooses to persist it.
- An editor owns its incomplete draft until validation succeeds.
- Counts are derived from catalogue contents; they are not independently mutable.

There is one dirty-state authority. A successful mutation marks the session
dirty. Only a verified successful save clears it.

## Persistence safety

Loading occurs into temporary state. The active session is replaced only after
the input is securely parsed, validated, normalized, and mapped successfully.

Saving uses a snapshot and a temporary file in the destination directory. The
temporary output is flushed, reopened, and validated before replacement. A failed
save never clears dirty state and never destroys the last known-good file.

The v1.1 catalogue reader is tolerant of compatible historical input. Its writer
remains the sole legacy-format owner. The implemented native-v2 candidate profile uses
the typed graph, stable references, canonical ordering, and strict limits in
[`spec/catalogue/v2.0.0`](../../spec/catalogue/v2.0.0/README.md); no published
support claim exists until reader, writer, migration, export, CLI, and recovery
gates pass together.

## Extensibility policy

Prefer declarative extensions: data packs, import/export profiles, column
presets, validation profiles, and localization. C3 does not load arbitrary
in-process plugin assemblies. A future executable extension system requires a
versioned capability protocol and process isolation.

Future native applications reuse language-neutral specifications, fixtures, and
behavior contracts. They do not consume the .NET 4.0 implementation as their API.

## Naming rules

- Use product language: `CatalogueSession`, `TapeDraft`, `BrandRules`.
- Name adapters after their mechanism: `LegacyXmlCatalogueStore`,
  `LegacyTapeRepository`, or `XmlUserPreferencesStore`.
- Name forms by feature and purpose: `TapeListForm`, `BrandEditorForm`.
- Do not introduce `Utils`, `Helpers`, `Common`, `Managers`, or `Platform`
  catch-all folders or types.
- Do not create a service, validator, presenter, or interface merely for symmetry.
  Introduce a type when it owns a real rule, boundary, or replaceable behavior.

## Further reading

- [Repository layout](repository-layout.md)
- [Catalogue persistence](persistence.md)
- [Canonical catalogue and Application architecture](catalogue-and-application.md)
- [Preference ownership and recovery](preferences.md)
- [ADR 0001: Modular monolith](decisions/0001-modular-monolith.md)
- [ADR 0002: Compatibility build lanes](decisions/0002-compatibility-build-lanes.md)
- [ADR 0003: C3 2.0 product boundary](decisions/0003-c3-2-product-boundary.md)
- [ADR 0004: Managed language strategy](decisions/0004-managed-language-strategy.md)
- [ADR 0005: Native-v2 format and migration](decisions/0005-native-v2-format-and-migration.md)
- [ADR 0006: Separate update channels](decisions/0006-separate-update-channels.md)
- [ADR 0007: C3-owned shared preferences](decisions/0007-c3-owned-shared-preferences.md)
- [ADR 0008: Qualified checkpoint ledger](decisions/0008-qualified-checkpoint-ledger.md)
- [ADR 0009: Typed domain identity and command contracts](decisions/0009-typed-domain-contracts.md)
- [ADR 0010: Stable release identity](decisions/0010-stable-release-identity.md)
- [ADR 0011: Shared C# WinForms presentation boundary](decisions/0011-shared-winforms-presentation-boundary.md)
- [ADR 0012: Canonical catalogue before Application/frontends](decisions/0012-canonical-catalogue-before-application-frontends.md)
- [ADR 0013: Canonical value and evidence semantics](decisions/0013-canonical-value-semantics.md)
- [ADR 0014: Profile capability and representability](decisions/0014-profile-capability-and-representability.md)
- [Distribution doctrine](../development/distribution.md)
- [Release catalogue v1 contract](../../spec/release-catalog/v1/README.md)
- [Update release manifest v1 contract](../../spec/update-feed/v1/README.md)
