# C3 Catalogue Format 1.1.0

This directory defines the compatibility contract for catalogue files identified
by `Information[Information = 'File Version']/Value = 1.1.0`.

The contract has three parts:

- `catalogue.xsd` describes the XML structure and primitive value types.
- This document defines semantic invariants that XSD cannot express clearly.
- `fixtures/catalogues/v1.1.0` contains executable examples and regression input.

The specification and fixtures are language-neutral. C3 1.3.0 must preserve
this contract through its original DataSet implementation.

## Document shape

The document element is `Catalogue`. Its children are repeated row elements from
six logical tables, in this order:

1. `Information`
2. `Counters`
3. `Decks`
4. `Brands`
5. `Models`
6. `Tapes`

This is the data-only XML emitted by the historic .NET `DataSet.WriteXml`
implementation. Empty tables may have no row element. Element names and casing
are compatibility-sensitive.

## Required semantic records

`Information` contains exactly one `File Version` row. Compatibility checks trim
its value and read the leading three numeric components, so a historical value
such as `1.1.0b1` identifies format 1.1.0.

`Counters` rows are historical serialized data, not authoritative totals.
Readers calculate Deck, Brand, Model, and Tape counts from actual rows after
loading. Writers may preserve and normalize counter rows for old clients.

## Keys in format 1.1.0

- `Brands.Code` is unique and non-empty.
- `Models.Identifier` is unique and non-empty.
- `Decks.Name` is unique and non-empty.
- `Tapes.IdentifierShort` is unique and non-empty.

These are legacy user-visible keys. C3 1.3.0 cannot silently rewrite them.
Stable opaque IDs require a future versioned catalogue format and migration.

## Compatibility behaviour

Readers:

- parse securely with DTD processing prohibited and external resolution disabled;
- reject malformed XML, missing file versions, and unsupported formats;
- load into temporary state and validate before replacing the active catalogue;
- accept omitted optional values and empty tables produced by historical C3;
- parse XML numbers and dates using XML/invariant rules; and
- report an actionable failure without exposing active state to partial input.

Writers:

- preserve v1.1.0 element names and primitive types;
- emit deterministic table and column order;
- use XML/invariant representations for dates, numbers, and Boolean values;
- write to a same-directory temporary file, flush, reopen, and validate it before
  replacement; and
- never clear dirty state until the destination contains the intended snapshot.

## Limits

The application may impose documented size, depth, and text-length limits to
protect memory on legacy systems. A limit must fail explicitly without changing
the active session. Unknown elements are compatibility errors unless a future
format explicitly defines an extension mechanism.
