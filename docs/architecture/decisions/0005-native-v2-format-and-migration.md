# ADR 0005: Design a native v2 format behind explicit migration

- Status: Proposed
- Date: 2026-08-04

## Context

Catalogue format 1.1.0 exposes historical `DataSet` tables, display-name
relationships, and mutable human-readable identifiers. It cannot safely support
all desired 2.x capabilities. Changing that writer before proving a replacement
model would break C3 1.x interoperability and encode guesses permanently.

## Proposed decision

Keep format 1.1.0 as a named legacy profile. Design a deterministic native v2
XML profile after stable identity and typed relationship prototypes pass.

The proposed native file is a plain UTF-8 XML document with the `.c3catalogue`
extension. C3 also accepts it when named `.xml`; the extension is a user-facing
association, not an opaque container. Media remains externally referenced in
2.0. A future archive/container uses a different explicitly specified profile.

Opening a legacy file never changes it. Users choose legacy mode, convert a copy,
or inspect read-only. Export to v1.1 is a separate, loss-aware operation.

## Acceptance gate

This ADR becomes Accepted only when:

- the typed domain and stable-ID rules are accepted;
- the public 1.x corpus and baseline reader are inventoried;
- the v2 specification and invariants are complete enough to implement twice;
- deterministic reader/writer and migration fixtures pass;
- repeated migration produces stable mappings;
- legacy export reports every unrepresentable value;
- failure/recovery and size/security limits are specified; and
- both build lanes and the headless validator agree.

Until then, `spec/catalogue/v2.0.0` is a draft and no release claims native-v2
write support.

## Alternatives under evaluation

- Keep v1.1 forever and add sidecar data: maximizes old-reader access but splits
  catalogue truth and complicates atomicity.
- Use a ZIP package immediately: convenient for media, but weakens inspectability
  and expands recovery/security scope before media is a 2.0 requirement.
- Use a database: strong query behavior, but reduces transparent portability and
  introduces migration/repair tooling disproportionate to current scale.
