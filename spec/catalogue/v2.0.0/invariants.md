# Native-v2 semantic and canonical invariants

## Identity and relationships

1. Every catalogue, brand, cassette model, deck model, deck unit, tape, and
   recording has one non-empty 32-character lowercase hexadecimal ID.
2. IDs are unique within their entity kind. A reference names the exact target
   kind and must resolve in the same document.
3. A cassette model references one brand. A deck unit references one deck model.
   A tape references one cassette model. A recording may reference one deck unit.
4. A tape owns exactly sides A and B. Each side owns zero or one recording; a
   recording is never shared or independently deleted.
5. Legacy codes and identifiers are preserved as import/export keys. They do not
   become native primary identity.

## Scalar meaning

- Native timestamps use UTC and the canonical `yyyy-MM-ddTHH:mm:ss.fffffffZ`
  representation. Offsets, unspecified values, leap-second text, and local time
  are rejected by the native reader.
- Legacy `DateTime` fields lack a durable zone contract. Migration preserves
  their wall-clock components, labels that value UTC, and records
  `legacy-local-wall-clock-assumed-utc` for each populated field. This is
  deterministic across machines and does not claim to recover an unknown offset.
- Decimals use XML invariant lexical form without exponent notation or trailing
  insignificant zeroes. Negative tape lengths are invalid.
- Text is preserved exactly after XML newline normalization. Required names and
  keys must contain at least one non-whitespace character. No hidden trimming or
  Unicode normalization occurs in profile 2.0.0.
- Derived counts are recomputed from collections and never persisted.

## Canonical document order

The writer emits declaration, root, metadata, brands, cassette models, deck
models, deck units, and tapes in that order. Entities are sorted by canonical ID.
Tape sides are always A then B. Child elements follow XML Schema sequence order.
Attributes are written in the order shown in canonical fixtures. Empty optional
text is emitted as an empty element; absent references are omitted.

The writer uses UTF-8 without BOM, LF line endings, two-space indentation, double
quotes, and one final LF. It emits no comments, processing instructions, CDATA,
DTD, entity reference, schema-location hint, or ignorable extension content.
Reading and rewriting a canonical document must produce byte-identical output.

## Extensions and future profiles

Profile 2.0.0 has no extension envelope. Unknown attributes, elements, namespace
content, and mixed markup are rejected instead of ignored. A future compatible
profile must define an explicit criticality/preservation contract before C3 can
retain foreign content safely.
