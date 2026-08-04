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

| Module | Owns | Must not own |
| --- | --- | --- |
| `C3.Domain` | Native-2.0 opaque identity, UTC/optional values, validation, commands, change sets, undo contracts, and migrated behavior such as persisted revision identity | Legacy keys, files, XML, `DataSet`, WinForms, settings, networking, or OS APIs |
| `C3.Catalogue` | Compatibility-facing catalogue concepts, commands, rules, results, session semantics, store interfaces, and narrow facades over migrated behavior | Files, XML, `DataSet`, WinForms, settings, networking, OS APIs, or a second implementation of migrated rules |
| `C3.Infrastructure` | XML format adapters, atomic file I/O, C3-owned preferences and legacy import, diagnostics, update feeds, and other external-system adapters | Form behavior or business rules |
| `C3.WinForms` | Forms, controls, user interaction, workspace state, preference composition, and runtime-lane policy | Persistence parsing or duplicated domain rules |

During Alpha 3, unmigrated `C3.Catalogue` features remain the sole production
behavior owners and VB differential oracles while named slices move to
`C3.Domain`. A frozen VB public facade may delegate to the migrated owner; it may
not retain a second implementation of the rule. Its compiled public
surface is frozen in [`spec/catalogue-api/v1`](../../spec/catalogue-api/v1/README.md)
and checked after every characterization build; behavior remains protected by
the executable characterization suite.

The complete `C3.Catalogue` production assembly is explicit C# 7.3. Its
mechanical translation was staged in an unpackaged candidate, matched against
all 269 frozen VB signatures, then promoted atomically through the behavior
suite. The candidate project and superseded VB implementations were removed so
there is one production owner.

`C3.WinForms` has two project files over the same physical UI sources:

- `C3.WinForms.Net40.vbproj`: x86, .NET Framework 4.0, Windows XP SP3+
- `C3.WinForms.Net48.vbproj`: x64, .NET Framework 4.8, Windows 7 SP1+

Only target-specific configuration, manifests, output paths, constants, and
small runtime adapters may differ between those project files.

## Dependency rule

```text
C3.WinForms.Net40 ----+--> C3.Infrastructure --> C3.Catalogue --> C3.Domain
                      |                         ^
C3.WinForms.Net48 ----+-------------------------+
```

Dependencies never point toward WinForms. `C3.Catalogue` depends only on APIs
available in .NET Framework 4.0. `C3.Infrastructure` may depend on
`C3.Catalogue`; the reverse reference is forbidden.

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

The v1.1 catalogue reader is tolerant of compatible historical input. The writer
is deterministic. The versioned specification, invariants, and golden fixtures
are the current implemented compatibility contract; the complete public 1.x
corpus remains a C3 2.0 programme gate.

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
- [Preference ownership and recovery](preferences.md)
- [ADR 0001: Modular monolith](decisions/0001-modular-monolith.md)
- [ADR 0002: Compatibility build lanes](decisions/0002-compatibility-build-lanes.md)
- [ADR 0003: C3 2.0 product boundary](decisions/0003-c3-2-product-boundary.md)
- [ADR 0004: Managed language strategy](decisions/0004-managed-language-strategy.md)
- [ADR 0005: Proposed native-v2 format and migration](decisions/0005-native-v2-format-and-migration.md)
- [ADR 0006: Separate update channels](decisions/0006-separate-update-channels.md)
- [ADR 0007: C3-owned shared preferences](decisions/0007-c3-owned-shared-preferences.md)
- [ADR 0008: Qualified checkpoint ledger](decisions/0008-qualified-checkpoint-ledger.md)
- [ADR 0009: Typed domain identity and command contracts](decisions/0009-typed-domain-contracts.md)
- [Release catalogue v1 contract](../../spec/release-catalog/v1/README.md)
- [Update release manifest v1 contract](../../spec/update-feed/v1/README.md)
