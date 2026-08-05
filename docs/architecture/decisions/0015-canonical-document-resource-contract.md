# ADR 0015: Canonical document resource contract

- Status: Accepted for C3 2.0 canonical convergence
- Date: 2026-08-06

## Context

The canonical catalogue needs one transaction boundary and immutable,
version-bound reads without assuming that every operation can copy, sort,
serialize, or hash an entire future million-entity catalogue. A loose `GetAll()`
API and an opaque continuation integer would allow stale pages, unbounded
filters, and silent mixing of results from different document states.

## Decision

The initial contract introduces:

- `CatalogueDocument`, whose current immutable snapshot and resource budget are
  explicit;
- `CatalogueTransaction`, which binds a bounded set of intents to an expected
  monotonic `ContentVersion`;
- `CatalogueSnapshot`, which binds session, version, semantic fingerprint, and
  entity counts;
- `CatalogueChangeSet`, which records before/after versions, fingerprints, and
  semantic changes;
- typed field identifiers and a closed filter grammar;
- explicitly ordered, page-sized queries;
- projections and continuation cursors bound to the exact document session,
  content version, query fingerprint, stable last sort key, and entity-ID
  tie-breaker; and
- a supplied `CatalogueResourceBudget` for entity, transaction, query, page,
  and snapshot ceilings.

Fingerprint state uses sorted typed entity keys and SHA-256 entity digests. The
aggregate root hashes a versioned, UTF-8, ordinal entry stream. The engine
supports immutable delta application and independent full recomputation; both
paths must produce the same scheme-bound `StateFingerprint`. This reference
implementation favors obvious verifiability. A measured tree/index
implementation may later reduce aggregate-update cost without changing the
root contract.

The contract deliberately supplies no universal numeric defaults. Alpha 12 must
choose supported/recommended/stress values from measured hosts; callers cannot
mistake a defensive schema ceiling for a performance promise.

The query grammar admits `Equals`, `Contains`, `StartsWith`, `IsKnown`,
`IsUnknown`, `Range`, `And`, `Or`, and `Not`. Construction enforces structural
validity; the query boundary enforces depth, term, and page budgets. A changed
session or content version invalidates a cursor rather than mixing pages.

## Transition status

This is a non-production contract slice. `CatalogueDocument` can expose and
budget an immutable snapshot and can form an expected-version transaction, but
it does not yet commit mutations or contain the complete canonical entity graph.
The legacy `DataSet` remains the production state owner until the complete
Alpha 6 projection/round-trip/differential gate passes. The new types therefore
expand C3's internal cross-project Catalogue API oracle deliberately without
creating a second production behavior owner.

## Consequences

- A stale transaction is refused before any operation can run.
- Bulk operations have an explicit maximum intent count.
- Snapshot and query results always name their semantic state.
- Cursor tokens cannot be reused across document versions.
- Presentation and CLI code need not invent pagination or staleness policy.
- Per-entity fingerprint production remains with feature projections; aggregate
  root computation, delta application, and full verification are deterministic.
- Snapshot lease enforcement and transaction commit remain later work; their
  resource concepts are fixed here.

## Alternatives rejected

- Unbounded `GetAll()`: creates avoidable memory and latency cliffs.
- Offset-only pagination: silently skips or duplicates entries after mutation.
- Serialize the entire document for every fingerprint: makes small edits scale
  with unrelated data.
- Put query/filter rules in WinForms: duplicates policy across frontends.
- Select permanent numeric limits before measurement: turns guesses into product
  claims.
