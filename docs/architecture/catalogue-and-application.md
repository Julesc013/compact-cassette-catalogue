# Canonical catalogue and Application architecture

Status: **Accepted direction; non-production foundation implementation underway**

This document owns the post-Alpha-5 in-memory catalogue, document-lifecycle, and
frontend boundary. The [execution plan](../planning/2.0-execution-plan.md) owns
milestone order and completion. Persistence syntax remains owned by
[catalogue persistence](persistence.md), and UI rendering remains owned by the
[OEM+ contract](../ui/oem-plus-design.md).

## Governing doctrine

C3 has one logical catalogue and one production mutation authority. That does
not require one physical representation or a deep copy of the complete
catalogue after every edit.

```text
one native-superset logical catalogue
        +
explicit constrained persistence profiles
        +
one document/application lifecycle
        +
toolkit-native frontend projections
```

Callers cannot mutate catalogue state except through a validated transaction.
Internally owned mutable indexes, copy-on-write aggregates, or structural
sharing are permitted when snapshot isolation, deterministic verification, and
measured memory budgets pass. A schema ceiling is never presented as a measured
interactive-capacity promise.

## Permanent dependency direction

The accepted target, introduced only as its milestone proves each boundary, is:

```text
WinForms presentation ----+
CLI process projection ---+----> C3.Application ----> C3.Catalogue ----> C3.Domain
TUI projection -----------+              ^                   ^                ^
                                          |                   |                |
composition roots --------+----> C3.Infrastructure -----------+----------------+
```

- `C3.Domain` owns foundational identities, temporal/value semantics,
  validation, and version/change primitives.
- `C3.Catalogue` owns the complete logical entity graph, relationships,
  invariants, transactions, queries, projections, and semantic change sets.
- `C3.Application` owns document sessions, lifecycle, operation coordination,
  savepoints/history, recovery policy, capabilities, allowed actions, progress,
  cancellation, and ports for external mechanisms.
- `C3.Infrastructure` implements profile adapters, transactional file storage,
  migration/export, preferences, recovery journals, diagnostics, reports, and
  update transport.
- Frontends own parsing and rendering for their interaction medium. They never
  own catalogue rules, clocks, IDs, history, persistence, or final save policy.

Infrastructure may reference inward contracts so it can implement Application
ports. Application never references Infrastructure, WinForms, Console, XML,
`System.IO`, or a specific UI threading primitive. Composition roots are the
only places allowed to assemble concrete Infrastructure adapters.

The CLR assemblies are internal cross-project APIs for C3 2.0, not a supported
third-party SDK. Long-lived public contracts are the catalogue profiles, CLI
process and machine-result schemas, stable operation/result codes, and
language-neutral conformance fixtures. A public binary SDK requires a separate
compatibility decision.

## Canonical catalogue boundary

`C3.Catalogue` will expose these logical roles without requiring these exact
physical data structures:

```text
CatalogueDocument
    sole authoritative logical state
    ID-indexed entity ownership
    relationship indexes
    current content version

CatalogueTransaction
    expected content version
    staged creates, updates, and deletes
    whole-operation validation
    atomic commit or refusal

CatalogueSnapshot
    immutable version-bound read facade
    safe for save staging, queries, reports, and background work

ChangeSet
    exact committed semantic changes
    before/after versions
    bounded inverse information where undoable

CatalogueProjection
    purpose-specific immutable read model
```

Feature handlers remain adjacent to their feature. `CatalogueDocument` must not
become a god object or a generic repository. Brands, models, decks, tapes, and
recordings share one transaction boundary while retaining feature-local rules.

### Whole-document transition rule

No canonical mutation becomes production authority while any supported entity
kind exists only in the active legacy `DataSet`.

The migration sequence is mandatory:

1. Project the complete legacy document into canonical read-only state.
2. Prove every entity, relationship, counter, alias, timestamp, and measurement.
3. Round-trip the complete canonical document through legacy and native profile
   adapters and their accepted equivalence rules.
4. Differentially compare legacy and canonical operations against equivalent
   documents.
5. Promote the canonical document as the sole active state.
6. Retain `DataSet` only inside the v1.1 adapter and legacy characterization;
   retain `Native*` objects only as native profile DTOs/projections.
7. Move feature mutation surfaces one proven slice at a time and remove the
   superseded owner promptly.

Synchronizing partial live states is prohibited. Production must never divide
Brands into a canonical document while Models, Decks, or Tapes remain active in
a separately mutable `DataSet`.

## Logical semantics and profile capabilities

The logical catalogue is a native superset, not the intersection of v1.1 and a
native profile. Profile-specific limits are representability constraints, not
universal catalogue invariants.

Every proposed state answers three independent questions:

1. Is it logically valid C3 catalogue content?
2. Can the current profile save it directly without loss?
3. Can a requested export profile represent it, and with which losses or
   transformations?

Each profile exposes explicit capabilities, including durable identity,
native-only values, partial dates, custom fields, direct save, lossless export,
and relationship stability across reopen.

Legacy v1.1 identity is session-scoped and is never advertised as durable across
save/reopen. C3 2.0 will not add an identity sidecar or private XML extensions to
legacy files. Conversion to a native profile establishes durable native
identity. Legacy aliases are compatibility metadata, optional in native state,
and must not constrain native entity creation.

The Alpha 4 `urn:c3:catalogue:2` / `2.0.0` profile and its evidence are immutable.
If the semantic audit changes temporal, uncertainty, alias, unit, provenance,
text, or extension structure, C3 defines a distinct superseding profile and
explicit migration. It never silently reinterprets or edits the Alpha 4 profile
in place.

Before the stable writer is selected, the semantic audit must decide:

- audit instants versus local, partial, approximate, or historical dates;
- unknown, absent, not applicable, estimated, inferred, and known-zero values;
- tri-state historical capabilities where `false` is ambiguous;
- units, ranges, precision, measurement standards, and controlled vocabularies;
- stored archival text versus comparison/search keys, Unicode normalization,
  whitespace, case, culture, control characters, and bidi safety;
- provenance for inferred or normalized values; and
- the exact meaning of legacy behaviors such as a packaged tape clearing sides.

[ADR 0013](decisions/0013-canonical-value-semantics.md) accepts these baseline
semantics. Presence is represented independently from unknown,
not-applicable, known, estimated, and inferred knowledge; known zero is never a
sentinel. Audit instants remain UTC, historical subject dates preserve partial
precision, physical measurements name units, vocabularies use stable codes,
archival text remains distinct from derived comparison keys, and material
normalization/inference carries bounded provenance. Profile adapters report any
representation loss rather than weakening the logical model.

[ADR 0014](decisions/0014-profile-capability-and-representability.md) makes the
three questions executable. Logical `ValidationResult`, published
`CatalogueProfileCapabilities`, and purpose-bound `RepresentabilityResult` are
separate contracts. Normalization, information loss, and unsupported content
remain distinct; an unsupported result can never authorize a destination write.
The frozen native-v2 capability projection intentionally does not claim the
qualified-value or partial-date semantics that require a successor profile.

## Document and content identities

These concepts are separate and must not be collapsed into a dirty Boolean:

| Identity | Contract |
| --- | --- |
| `DocumentSessionId` | Always identifies this exact open application session. Mutations target it. |
| `CatalogueId` | Optional durable logical catalogue identity. Native profiles persist it; legacy mode may not. |
| `ContentVersion` | Monotonic in-session mutation, undo, redo, and concurrency version. It never moves backward. |
| `StateFingerprint` | Versioned semantic-content equality, independent of file syntax and history position. |
| `DiskRevision` | Exact externally observed file-byte identity and relevant file identity metadata. |
| `LocationLease` | Exact destination identity and revision authorized for create or replacement. |

`HistoryPositionId` may exist internally for efficient navigation, but it is not
logical equality. Entity projections also declare whether identity is durable,
session-scoped, or an imported alias.

The first foundation slice implements the nominal session/catalogue IDs,
monotonic content-version value, scheme-bound fingerprint value, opaque disk
revision, and entity-ID durability classification in `C3.Domain`. Fingerprint
calculation, destination leases, history positions, and the complete document
owner remain later dependency-ordered work.

`StateFingerprint` uses a documented scheme such as
`c3-logical-state-sha256-v1`. It declares field ordering, text/value rules,
entity ordering, and included metadata. Per-entity digests and ordered aggregate
roots may update incrementally; full deterministic recomputation verifies them
at save, qualification, and integrity checkpoints. One small edit must not
require reserializing or hashing a million unrelated tapes.

## Transactions, snapshots, queries, and plans

Only one catalogue mutation may commit against a document at a time. Refused,
invalid, stale, cancelled, or failed operations change nothing. Bulk catalogue
mutation is atomic by default and produces one change set and one history entry;
best-effort behavior must be explicit and is never the default.

Snapshot reads are version-bound. Snapshot leases have limits for concurrency,
lifetime, retained memory, cancellation, and disposal so long-running reports do
not pin obsolete large states indefinitely.

Queries use a bounded typed filter grammar (`Equals`, `Contains`, `StartsWith`,
`IsKnown`, `IsUnknown`, `Range`, `And`, `Or`, `Not`) with typed field IDs,
explicit comparison policy, maximum depth/terms, and a cost budget. Cursor pages
bind document session, content version, query/sort identity, stable last key, and
entity-ID tie-breaker. A changed version returns `QueryStale`; pages from
different states are never silently mixed. Human locale sorting never controls
canonical file or machine-output order.

Destructive, bulk, migration, import, export, and reassign previews return a
version-bound `OperationPlan`. A plan includes a plan ID/fingerprint, expected
document and disk state, affected entities, warnings, conflicts, profile losses,
resource estimate, and commit policy. Any relevant document, destination,
profile, or semantic selection change invalidates the plan before commit.

Derived indexes are version-labelled, discardable, deterministically rebuildable,
and never authoritative persisted meaning. They publish only when still valid
for the current state.

## Application lifecycle and operations

The Application layer makes legal document transitions executable:

```text
NoDocument -> Opening -> OpenClean <-> OpenDirty
                              |            |
                              +-> SavingSnapshot
                              +-> Conflict
                              +-> RecoveryRequired
                              +-> Closing -> NoDocument
```

Read-only/profile/background/draft conditions are orthogonal state, not an
unbounded cross-product enum. An `AllowedActionSet` is a pure projection of
lifecycle, profile capabilities, selection semantics, draft state, recovery,
background work, and content version. Frontends choose visual placement; they
do not independently invent whether an operation is legal.

Application supplies audit clocks, entity/operation/correlation IDs, transaction
context, and expected-version checks. Frontends submit user values and intent,
not timestamps or identifiers.

A toolkit-neutral immutable event stream reports document, history, save,
conflict, recovery, allowed-action, progress, and cancellation changes. Events
carry session/catalogue identity when available, operation/correlation identity,
before/after versions, affected entities, and changed capabilities. Each
frontend marshals events onto its own interaction thread; Application knows
nothing about WinForms handles, `Control.Invoke`, or terminal redraws.

Typed operation implementations remain authoritative. One deterministic
descriptor per operation supplies or verifies stable operation ID, request and
result type, mutability, undoability, preview/cancellation support, profile
requirements, resource/help keys, and diagnostic category. Build validation may
project CLI/TUI help, GUI metadata, machine registries, and conformance fixtures.
A runtime string command bus is prohibited.

Results distinguish `Success`, `SuccessWithWarnings`, `Refused`, `Invalid`,
`Conflict`, `Unsupported`, `Corrupt`, `Unavailable`, `Cancelled`, and unexpected
`Failed`. They contain stable codes, resource keys, structured parameters,
field/entity paths, state-changed/safe-state facts, next-action IDs, and a
technical diagnostic reference. Final English prose and raw exception messages
do not cross the Application boundary.

## Save, conflict, history, and recovery

Saving captures snapshot `S` at version `V`, creates a proposed persisted
snapshot with audit metadata without mutating active state, validates logical and
profile rules, acquires and revalidates a destination lease, writes/flushes/
reopens/verifies temporary output, and replaces the destination only after every
precondition holds. Application installs saved state only after success.

If version `V+1` exists when saving `V` completes, the file and saved fingerprint
represent `V` while the current document remains dirty. Save failure leaves the
active document unchanged. Save As uses `CreateNew` or an observed-revision
replacement lease; a file-dialog confirmation alone is not replacement authority.

External conflict preserves both states and offers truthful actions such as
Compare, Save Current Work as Copy, Reload External Version, and Cancel.
Automatic merge is deferred until three-way semantic merge evidence exists.

Five mechanisms remain distinct even when they share file primitives:

| Mechanism | Purpose |
| --- | --- |
| Undo history | Reverse recent committed semantic mutations in process. |
| Workspace recovery journal | Restore applied unsaved semantic work after process failure. |
| Migration/external-operation journal | Recover one multi-file externally visible operation. |
| File backup | Preserve the immediate previous verified saved bytes. |
| Conflict copy | Preserve local and external states until explicit resolution. |

History has entry and byte budgets, compound-operation policy, coalescing rules,
and invalidation rules. Recovery journals have schema, identity/revision binding,
integrity, size, retention, privacy, discovery, preview, discard, and cleanup
policies. A verified previous destination backup is mandatory where the profile
promises it; backup failure before replacement refuses save. Restore previews
and verifies a copy by default.

| Persisted state | Authority | Portability and retention rule |
| --- | --- | --- |
| Catalogue content and native metadata | Catalogue/profile file | User-controlled and portable with the catalogue. |
| Legacy aliases | Logical catalogue/profile mapping | Portable only where the selected profile represents them. |
| Preferences and view state | C3 profile | Machine/user scoped unless explicit portable-profile mode is active; bounded/resettable. |
| Recent files | C3 profile | Privacy-sensitive, bounded, optional, and never portable by implication. |
| Derived indexes | Cache | Discardable, version-labelled, and rebuildable. |
| Workspace recovery | Application recovery store | User data retained until save, explicit discard, or reviewed expiry. |
| Migration/operation journal | Owning Infrastructure operation | Adjacent or managed; retained only to a verified terminal/recovery state. |
| Diagnostics | Diagnostic store | Privacy-sensitive, redacted, bounded, and consent-gated for export. |
| Support bundle | User-selected output | User-reviewed and user-controlled. |
| Backup | Persistence transaction | Previous verified bytes under explicit rotation/cleanup policy. |
| Conflict copy | Document conflict workflow | Retained until explicit user resolution. |

Every persisted state category—catalogue, profile metadata, preferences, view
state, recent files, derived indexes, workspace recovery, operation journals,
diagnostics, support bundles, backups, and conflict copies—must declare location,
locking, schema, integrity, size, retention, cleanup, privacy, portable-profile,
and side-by-side ownership before Beta.

## Frontend scope

WinForms remains the required complete 2.0 interface and uses one shared C# 7.3,
net40-compatible presentation implementation for both executable lanes. The
permanent shell is selected through task/entity/hybrid information-architecture
prototypes and usability evidence, not merely from entity class names.

The required CLI scope is bounded to `inspect`, `validate`, migration dry-run and
commit, `recover`, and `export-legacy`, with schema-versioned machine results,
stable existing exit compatibility, strict stdout/stderr behavior, redirected
execution safety, cancellation, destination leases, and output neutralization.
A read-only query is optional proof; arbitrary batch mutation is deferred.

Application and terminal contracts are required. A line-mode TUI or optional
full-screen mode ships only if independently complete and qualified; otherwise
the executable/capability is explicitly deferred without blocking a safe Windows
2.0 release. Production AppKit, GTK, Qt, ARM64, cloud, accounts, automatic merge,
multi-document windows, in-process plugins, and a general query/mutation language
remain outside compulsory 2.0 scope.

## Architecture fitness gates

The build will progressively enforce that:

- `DataSet` exists only in the legacy adapter and legacy characterization;
- native profile DTOs never cross into ordinary frontends;
- no frontend holds feature services, file stores, clocks, or ID generators;
- no profile key limit appears in format-neutral mutation rules;
- no large-list production path uses unbounded `GetAll()`;
- no mutation omits expected session and content version;
- no save omits snapshot fingerprint and destination lease;
- every query has stable ordering and an ID tie-breaker;
- every bulk operation declares atomic or explicit best-effort policy;
- every temporary, recovery, backup, and conflict file has one owner and cleanup
  rule; and
- every capacity/support claim has a named fixture, host, method, and budget.

Implementation may revise type names when tests reveal a better boundary. It may
not weaken the ownership, transition, identity, data-safety, or evidence rules
without a superseding ADR.

Foundation value types, fixtures, and read-only shadow infrastructure may land
on `dev/2.x` before Alpha 5 promotion when explicitly directed by the owner,
provided they do not change the production mutation authority or claim Alpha 6
qualification. Any such tracked change supersedes the current Alpha 5 candidate
and requires its complete automated and owner qualification to restart.
