# ADR 0013: Canonical value and evidence semantics

- Status: Accepted for C3 2.0 canonical convergence
- Date: 2026-08-06

## Context

Legacy C3 fields often use an empty string, zero, `false`, or `DateTime.MinValue`
for several different meanings. Those representations cannot distinguish an
absent field from an unknown value, a known zero, a value that does not apply,
or a value inferred during migration. The native 2.0.0 profile already has
qualified UTC audit instants but intentionally froze many legacy scalar shapes.
Reusing either persistence representation as the logical model would silently
turn storage accidents into permanent catalogue rules.

Canonical convergence also needs one rule for partial historical dates,
controlled vocabularies, units, text comparison, and migration provenance before
the whole-document graph or its fingerprint can be defined.

## Decision

### Presence and knowledge

Field presence and value knowledge are separate:

```text
Optional.None                           field absent
Optional.Some(QualifiedValue.Unknown)   field present, value unknown
QualifiedValue.NotApplicable            field has no meaningful value
QualifiedValue.Known(value)             directly known value
QualifiedValue.Estimated(value)         value estimated by a person or process
QualifiedValue.Inferred(value)          value derived from other evidence
```

Known zero and known `false` are ordinary known values. They never mean absent
or unknown. Unknown and not-applicable values carry no hidden/default payload.
Canonical code must not infer a knowledge state merely from a scalar default.

### Time

Audit and operation instants use `UtcTimestamp` and always identify an exact UTC
instant. Subject matter dates use `HistoricalDate`, which preserves year,
year-month, or full Gregorian-day precision. Approximate, estimated, or inferred
meaning is expressed by the qualified-value wrapper rather than by altering the
date. A local wall time that cannot be mapped to an instant is not silently
converted to UTC; its eventual type must carry the original zone/offset evidence.

### Provenance

Provenance is orthogonal to value knowledge. A canonical projection records the
source profile and revision, stable source locator or legacy alias where safe,
and every normalization/inference rule code that materially affected the value.
It must not store a private filesystem path, user name, or other unnecessary
machine identity. Direct native values may omit per-field provenance when the
document/profile provenance is sufficient. Inferred and normalized values must
name their deterministic rule.

### Units and vocabularies

Physical measurements carry an explicit unit in the canonical contract. Counts
and genuinely dimensionless ratios are the only unadorned numeric values.
Writers never guess a unit from magnitude. A legacy field with an implicit unit
is mapped by a profile-specific rule whose code is retained as provenance.

Controlled values use a stable, lowercase ASCII code independent of display
text. Unknown source spellings are retained as archival aliases and reported by
profile validation; they are not silently added to the vocabulary. UI labels are
resource projections and do not enter persistence or fingerprints.

### Stored text and comparison

Stored archival text is preserved exactly after field-specific size and safety
validation. Single-line fields reject line separators; all fields reject NUL,
unpaired surrogates, and disallowed controls. C3 does not rewrite stored spelling,
case, whitespace, or normalization merely to simplify searching.

Comparison/search keys are derived, discardable data. The first canonical key
scheme uses Unicode compatibility normalization, collapses Unicode whitespace,
trims, and applies invariant case folding. Its scheme identifier participates in
query/fingerprint contracts. Locale-aware display sorting is presentation state;
canonical serialization and machine output use explicit ordinal field/ID order.
Bidi controls are surfaced for safety review and never used as hidden identity.

### Profile succession

Legacy v1.1 entity identity remains session-scoped. Conversion to a native
profile establishes durable identity; C3 creates no sidecar and no private v1.1
extension. The accepted Alpha 4 `urn:c3:catalogue:2` / `2.0.0` profile remains
immutable. If canonical values cannot be represented by that profile, direct-save
capability reports the limitation and a distinct successor profile plus explicit
migration is required.

## Executable contract

`QualifiedValue<T>` and `ValueKnowledge` make absence, unknown,
not-applicable, known zero, estimated, and inferred states distinct.
`HistoricalDate` preserves partial precision and validates the Gregorian date.
Characterization covers default/invalid cases and invariant text. Later
canonical graph/profile adapters must use shared fixtures to prove that their
projection never collapses these states.

## Consequences

- Logical validity and profile representability remain separate.
- The canonical graph can represent more truth than either current file profile.
- Migration/export must report every collapsed knowledge, precision, vocabulary,
  unit, text, or provenance distinction.
- Search indexes and UI sorting cannot mutate archival text or canonical order.
- Fingerprint scheme v1 must encode presence, knowledge, units, vocabulary codes,
  partial precision, and material provenance explicitly.
- This foundation does not change the current production `DataSet`, native
  profile bytes, file writer, UI, or mutation owner.

## Alternatives rejected

- Continue using sentinel scalar values: preserves ambiguity and loses data.
- Put uncertainty inside strings: prevents typed validation and stable queries.
- Normalize stored text on load: changes archival content and makes round trips
  destructive.
- Expand the frozen Alpha 4 profile in place: invalidates its schema and evidence.
- Preserve legacy IDs in a sidecar: creates multi-file ownership and portability
  failure.
