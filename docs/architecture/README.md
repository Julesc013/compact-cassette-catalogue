# C3 Architecture

C3 is one product with one catalogue contract, one shared source tree, and two
Windows build lanes. The repository is a modular monolith: dependency boundaries
are represented by projects, while related behavior is kept together in feature
folders.

The architecture optimizes for four things, in this order:

1. Never corrupt or silently discard a user's catalogue.
2. Preserve the supported catalogue format and the Windows XP compatibility lane.
3. Keep each behavior in one physical source file wherever possible.
4. Make future capabilities additive instead of forcing another rewrite.

## Production modules

| Module | Owns | Must not own |
| --- | --- | --- |
| `C3.Catalogue` | Catalogue concepts, commands, rules, results, session semantics, and store interfaces | Files, XML, `DataSet`, WinForms, settings, networking, or OS APIs |
| `C3.Infrastructure` | XML format adapters, atomic file I/O, diagnostics, update feeds, and other external-system adapters | Form behavior or business rules |
| `C3.WinForms` | Forms, controls, user interaction, workspace state, settings adapters, composition, and runtime-lane policy | XML parsing or duplicated domain rules |

`C3.WinForms` has two project files over the same physical UI sources:

- `C3.WinForms.Net40.vbproj`: x86, .NET Framework 4.0, Windows XP SP3+
- `C3.WinForms.Net48.vbproj`: x64, .NET Framework 4.8, Windows 7 SP1+

Only target-specific configuration, manifests, output paths, constants, and
small runtime adapters may differ between those project files.

## Dependency rule

```text
C3.WinForms.Net40 ----+--> C3.Infrastructure --> C3.Catalogue
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
- `WorkspaceState` owns view-only state such as selected IDs, filters, columns,
  and open-window preferences.
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
are the public compatibility contract; implementation details are not.

## Extensibility policy

Prefer declarative extensions: data packs, import/export profiles, column
presets, validation profiles, and localization. C3 1.x does not load arbitrary
in-process plugin assemblies. A future executable extension system must use a
versioned contract and should prefer process isolation.

Future native applications reuse language-neutral specifications, fixtures, and
behavior contracts. They do not consume the .NET 4.0 implementation as their API.

## Naming rules

- Use product language: `CatalogueSession`, `TapeDraft`, `BrandRules`.
- Name adapters after their mechanism: `XmlCatalogueStore`, `MySettingsStore`.
- Name forms by feature and purpose: `TapeListForm`, `BrandEditorForm`.
- Do not introduce `Utils`, `Helpers`, `Common`, `Managers`, or `Platform`
  catch-all folders or types.
- Do not create a service, validator, presenter, or interface merely for symmetry.
  Introduce a type when it owns a real rule, boundary, or replaceable behavior.

## Further reading

- [Repository layout](repository-layout.md)
- [Catalogue persistence](persistence.md)
- [ADR 0001: Modular monolith](decisions/0001-modular-monolith.md)
- [ADR 0002: Compatibility build lanes](decisions/0002-compatibility-build-lanes.md)
