# ADR 0009: Typed domain identity and command contracts

Status: **Accepted for C3 2.0 Alpha 3**

Date: 2026-08-04

## Context

C3 1.x persisted relationships through mutable display names and compact codes.
Those values remain essential legacy-format keys, but they cannot be the identity
of a long-lived 2.0 object: users may correct a name, two physical decks may share
a model name, and migration must report mappings without silently changing what a
reference means.

The new model must remain usable from .NET Framework 4.0, both build lanes, a
future CLI, and later native implementations. It must not import WinForms,
`DataSet`, XML, file-system, update, or operating-system concerns.

## Decision

`C3.Domain` is the C# 7.3, .NET Framework 4.0-compatible substrate for the native
2.0 domain. It owns these language-neutral semantics:

- `EntityId<TAggregate>` is an opaque non-empty 128-bit value rendered as exactly
  32 lowercase hexadecimal characters. The aggregate type participates in the
  compile-time type, never in the stored bytes. Display names and legacy codes
  are attributes or adapter keys, not identity.
- Equality compares identifier bytes. Ordering compares the canonical lowercase
  representation ordinally so every runtime produces the same order.
- Production creation uses cryptographically strong random GUIDs. Fixture and
  migration code may inject a deterministic SHA-256-based generator whose seed,
  aggregate type, and sequence completely determine output.
- Domain timestamps are explicit UTC instants. Local or unspecified `DateTime`
  values are rejected at the boundary.
- Absence uses `Optional<T>`; `null`, empty text, zero, unknown, and not-applicable
  are never silently conflated.
- Validation failures carry stable machine codes, field paths, and user-facing
  messages. A rejected command has issues and no change set. A successful command
  has a value and a non-empty versioned change set.
- Commands carry an opaque command ID, UTC issue time, and optional expected
  aggregate version. Undo is another explicit command that names the command it
  reverses; a UI does not mutate objects backwards by reflection.

## Aggregate vocabulary

The native model uses these boundaries:

| Aggregate/entity | Identity and ownership |
| --- | --- |
| `Catalogue` | Root transaction boundary; owns catalogue version and membership, not UI state |
| `Brand` | Independent reference aggregate identified by `EntityId<Brand>` |
| `CassetteModel` | Independent aggregate referencing `Brand` by ID |
| `Tape` | Independent aggregate referencing `CassetteModel`; owns exactly sides A and B and their recordings |
| `Recording` | Entity owned by one `Tape`; never independently persisted or deleted |
| `DeckModel` | Optional reusable manufacturer/model specification, independent of a user's physical equipment |
| `DeckUnit` | A physical owned deck with its own identity; may reference `DeckModel` and may carry per-unit overrides |

Recordings reference `DeckUnit`, not a mutable manufacturer/model display name.
Legacy 1.1 adapters may derive deterministic IDs from provenance plus legacy keys
during import, but that mapping belongs to migration infrastructure and must be
reported. A native-v2 writer persists the resulting IDs directly.

## Migration rule

Alpha 3 introduces and proves contracts before replacing behavior. The existing
VB catalogue library remains the differential oracle and sole production owner
until a named slice passes its API/behavior comparison. A port commit is
mechanical. Redesign, persistence changes, and cleanup are separate commits.
Once a slice is accepted, only the C# implementation remains on the production
path; any retained VB source is test-only oracle material with an explicit exit
gate or a logic-free compatibility facade whose compiled surface is frozen.

The first accepted slice is persisted catalogue revision identity. The C# type
owns validation, ordinal equality, hashing, and text projection. The public VB
type delegates to it solely to preserve the frozen `C3.Catalogue` binary/source
surface while existing callers migrate.

The second accepted slice is catalogue document/session state. The C# type owns
path, display name, revision, dirty state, change sequence, transition ordering,
and change notification. The VB facade retains only the legacy revision wrapper
projection and re-emits notifications with the compatibility facade as sender.

## Consequences

- Renames no longer break identity in native 2.0.
- Deck model data and physical recording equipment are no longer conflated.
- Deterministic migration and property tests do not require production-random
  values.
- The legacy 1.1 format remains unchanged and requires an explicit mapping layer.
- `C3.Domain` is an additional transition module, not permission to duplicate
  brand/model/deck/tape rules. Those rules move once, slice by slice.
