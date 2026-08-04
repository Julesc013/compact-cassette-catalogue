# C3 catalogue format 2.0.0 candidate profile

Status: **Frozen Alpha 4 candidate contract**

This directory is the language-neutral contract for C3's native, inspectable
catalogue format. The profile is not a ZIP container or database: one
`.c3catalogue` file is deterministic UTF-8 XML without a byte-order mark. C3
also accepts the profile when it is named `.xml`; extensions are file-association
hints, not format detection.

The namespace and format identifier are immutable:

```text
namespace  urn:c3:catalogue:2
format     2.0.0
```

The [XML Schema](catalogue.xsd) defines syntax. [Invariants](invariants.md)
define meaning and canonical writer order. [Security limits](security-limits.md)
define mandatory rejection boundaries. `support-matrix.v1.json` and
`normalization-vectors.v1.json` are machine-readable compatibility inputs.

## Ownership

- `C3.Catalogue.Native` owns the typed native model and referential invariants.
- `C3.Infrastructure.CatalogueFiles.Xml.V2_0` exclusively owns v2 XML parsing,
  canonical writing, revisions, and transactional file replacement.
- `C3.Infrastructure.Migrations.V1_1ToV2_0` exclusively owns legacy mapping,
  dry runs, reports, recovery journals, and convert-copy orchestration.
- `C3.Infrastructure.CatalogueFiles.Xml.V1_1` remains the only legacy-format
  reader/writer and is reused by loss-aware export.
- WinForms and the CLI call those owners; neither duplicates XML or migration
  logic.

## Compatibility posture

Native identifiers are stable opaque 128-bit values. Editable names and legacy
codes are attributes, never relationship identity. Relationships use IDs and
must resolve within the same catalogue. Derived counters are never serialized.

Opening or saving a 1.1 catalogue never converts it. Conversion is a separately
named operation that writes a new destination, verifies it through the native
reader, preserves the source bytes, and emits an auditable mapping report.
Export to 1.1 is also separately named and refuses unreported loss.

ADR 0005 becomes Accepted only after the candidate contract, both build lanes,
the independent CLI, migration/export fixtures, recovery tests, and exact
package gates pass together. Until then no published release claims native-v2
support.
