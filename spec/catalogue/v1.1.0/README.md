# C3 Catalogue Format 1.1.0

This directory defines the compatibility contract for catalogue files identified
by `Information[Information = 'File Version']/Value = 1.1.0`.

The contract has three parts:

- `catalogue.xsd` describes the XML structure and primitive value types.
- This document defines semantic invariants that XSD cannot express clearly.
- `fixtures/catalogues/v1.1.0` contains executable examples and regression input.

The specification and fixtures are language-neutral. A future C3 implementation
must pass the same contract; it is not required to reuse the VB.NET code.

## Document shape

The document element is `Catalogue`. Its children are repeated row elements from
six logical tables:

1. `Information`
2. `Counters`
3. `Decks`
4. `Brands`
5. `Models`
6. `Tapes`

This shape is the data-only XML emitted by the historic .NET `DataSet.WriteXml`
implementation. Empty tables may have no row element. Element names and casing
are compatibility-sensitive.

## Required semantic records

The `Information` table must contain exactly one row whose `Information` value is
`File Version`. Its `Value` is normalized by trimming surrounding whitespace and
reading the leading three numeric components. Thus a historical value such as
`1.1.0b1` identifies format 1.1.0 for compatibility checking.

The `Counters` rows are historical serialized data, not authoritative totals.
Readers calculate Deck, Brand, Model, and Tape counts from their corresponding
rows after loading. Writers may preserve and normalize the counter rows for old
clients.

## Keys in format 1.1.0

- `Brands.Code` is unique and non-empty.
- `Models.Identifier` is unique and non-empty.
- `Decks.Name` is unique and non-empty.
- `Tapes.IdentifierShort` is unique and non-empty.

These are legacy user-visible keys. They cannot be silently rewritten in v1.1.0.
Stable opaque IDs require a future versioned catalogue format and migration.

## Compatibility behavior

Readers:

- securely parse with DTD processing prohibited and external resolution disabled;
- reject malformed XML, a missing file version, and unsupported format versions;
- load into temporary state and validate before replacing the active catalogue;
- accept omitted optional values and empty tables produced by historical C3;
- parse XML numeric/date values using XML/invariant rules; and
- report a typed, actionable failure without exposing the old catalogue to
  partial input.

Writers:

- preserve the v1.1.0 element names and primitive types;
- emit deterministic table/column ordering;
- use XML/invariant representations for dates, numbers, and booleans;
- write to a temporary file, flush, reopen, and validate before replacement; and
- never clear dirty state until the destination is known to contain the intended
  snapshot.

## Limits

The application may impose documented size, depth, and text-length limits to
protect memory on legacy systems. Such limits must fail explicitly and must not
modify the active session. Unknown elements are compatibility errors unless a
future specification explicitly defines an extension mechanism.

