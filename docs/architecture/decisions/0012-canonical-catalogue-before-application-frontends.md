# ADR 0012: Converge the canonical catalogue before Application and frontends

- Status: Accepted for the post-Alpha-5 C3 2.0 programme
- Date: 2026-08-06

## Context

Alpha 5 proves a shared C# WinForms presentation path over the existing legacy
editable `DataSet`. Alpha 4 separately proves a deterministic native object
graph, storage profile, migration, recovery, export, and CLI. The repository also
contains generic Domain command types and presentation-owned reversible command
history.

Adding `C3.Application` or more frontends immediately would place another layer
over competing editable, persistence, command, session, and history models. A
feature-by-feature transition could make Brands canonical while Models, Decks,
and Tapes remained live in a separately mutable `DataSet`, requiring fragile
synchronization among multiple truths.

## Decision

C3 will converge the complete supported catalogue into one native-superset
logical authority before canonical mutation enters production.

Alpha 6 proves whole-document shadow projection and round-trip equivalence for
every entity and relationship through explicit legacy and native profile
adapters. No new production frontend or canonical production mutation enters
that milestone.

Alpha 7 may introduce `C3.Application` and the first production canonical
mutations only after the whole-document boundary passes. At promotion,
`CatalogueDocument` is the sole active logical state; `DataSet` is confined to
the v1.1 adapter/characterization and `Native*` objects are confined to native
profile adaptation.

The logical boundary exposes immutable snapshots but does not mandate deep-copy
storage. Owned indexes, copy-on-write, or structural sharing are acceptable when
transactions are atomic, callers cannot bypass validation, snapshots are
isolated, incremental fingerprints are fully verifiable, and resource budgets
pass.

The logical catalogue is not limited to legacy representability. Legacy v1.1 is
a constrained profile whose entity identities are session-scoped; C3 will not
add an identity sidecar or private legacy XML fields. Native conversion creates
durable identity. Logical validity, current-profile direct-save validity, and
requested-export representability remain separate results.

Document session, optional durable catalogue, content version, semantic state,
disk revision, and destination lease identities remain distinct. Application
owns lifecycle, clocks/IDs, operations, history/savepoints, recovery, allowed
actions, events, progress/cancellation, and external ports. Catalogue owns
logical rules/transactions/queries. Infrastructure implements mechanisms.
Frontends project interaction only.

The detailed contract is [Canonical catalogue and Application architecture](../catalogue-and-application.md).

## Consequences

- The release train expands from Alpha 6 through Alpha 12 so semantic
  convergence, Application lifecycle, Windows shell, workflow replacement, CLI,
  optional TUI disposition, and final hardening have independent gates.
- Alpha 5 behavior and evidence remain historical input, not the permanent
  location of catalogue history or commands.
- The Alpha 4 native profile is immutable. A semantic change requires an
  explicitly versioned successor and migration rather than reinterpretation.
- The permanent CLR projects are internal implementation APIs for 2.0; public
  longevity is carried by language-neutral profiles, process schemas, codes,
  and conformance fixtures unless a later ADR accepts an SDK.
- TUI, rich CLI mutation, alternate native shells, automatic merge, and other
  additive capabilities cannot delay a complete safe Windows product unless
  scope is explicitly changed.

## Alternatives rejected

- Add Application above the current models: preserves competing truths and moves
  ambiguity rather than resolving it.
- Migrate live state feature by feature: divides one document between canonical
  and `DataSet` authorities.
- Use the immutable native file graph directly as the editing engine: risks
  whole-graph copying and confuses persistence DTOs with logical transactions.
- Make v1.1 the canonical lowest common denominator: permanently imports legacy
  key, identity, time, and representability limits into native semantics.
- Preserve legacy identity through sidecars or private XML: creates multi-file
  ownership or breaks the strict compatibility contract.
