# C3 catalogue format 2.0.0 design draft

Status: **Draft — not implemented, emitted, or supported by a release**

This directory is the language-neutral design space for a future native C3
catalogue profile. Catalogue format 1.1.0 remains the only implemented writer.
Do not add `2.0.0` to a supported-format list until ADR 0005's acceptance gate
and the complete release gate pass.

## Design goals

- Plain, deterministic UTF-8 XML that remains inspectable without C3.
- Stable opaque identity separated from editable names and legacy codes.
- Explicit references with referential-integrity validation.
- Lossless representation of C3 2.0's accepted domain.
- Stream-safe parsing with documented size and complexity limits.
- Canonical ordering, decimal, date/time, whitespace, and normalization rules.
- Forward-compatible extension envelopes without accepting unknown core meaning.
- Deterministic migration from supported legacy fixtures and loss-aware export.

## Provisional logical model

The model is expected to distinguish:

- catalogue identity and metadata;
- brands;
- cassette models;
- physical tape units;
- tape sides and recordings;
- deck models and owned deck units where the accepted domain requires both;
- user-defined views/tags only if they enter the 2.0 stable scope; and
- provenance/migration records needed to explain imported identity.

Names and human-readable codes are editable attributes or aliases. Relationships
use stable IDs. Timestamps have explicit meaning and offset policy. Derived
counts are not serialized as independent authority.

## Provisional XML rules

- XML declaration and UTF-8 without a byte-order mark.
- One namespace-qualified root identifying the format profile.
- DTDs and external entities prohibited.
- Unknown core elements rejected; documented extension elements retained or
  reported according to their declared criticality.
- Canonical element/attribute order defined by the future schema and writer
  profile, never by reflection or dictionary enumeration.
- Binary media excluded from the initial XML file; references are URI/path data
  with an explicit base and portability policy.

## Required artifacts before acceptance

This directory will eventually contain the normative overview, XML Schema,
canonical examples, invariants, compatibility/loss table, security limits, and
normalization vectors. Executable fixtures live under a matching versioned
fixture directory. Migration algorithms and reports are documented separately so
the format contract does not become implementation-specific.
