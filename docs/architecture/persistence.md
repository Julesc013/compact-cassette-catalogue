# Implemented catalogue persistence boundaries

Two format owners coexist without sharing syntax logic:

| Profile | Sole owner | Model boundary |
| --- | --- | --- |
| Legacy 1.1.0 | `CatalogueFiles.Xml.V1_1.LegacyXmlCatalogueStore` | Compatibility `DataSet` below the composition seam |
| Native 2.0.0 candidate | `CatalogueFiles.Xml.V2_0.NativeXmlCatalogueStore` | `C3.Catalogue.Native.NativeCatalogue` |

## Legacy 1.1.0

`C3.Infrastructure.CatalogueFiles.Xml.V1_1.LegacyXmlCatalogueStore` is the only
component permitted to read or write the legacy v1.1 XML format. WinForms owns
file dialogs and messages; catalogue rules do not know about files or XML.

### Load transaction

1. Check that the selected file exists and is at most 64 MiB.
2. Parse with DTD processing prohibited, external resolution disabled, and XML
   character limits applied.
3. Require an unqualified `Catalogue` root and reject unknown table/field names.
4. Read and normalize the file-format version, then reject missing or unsupported
   versions.
5. Clone the known in-memory schema and read into that temporary `DataSet` with
   constraints initially disabled.
6. Derive counters from actual rows and re-enable constraints.
7. Calculate a SHA-256 revision token.
8. Return the complete temporary document. Only the WinForms composition layer
   may then swap it into the active session.

Any failure leaves the existing global compatibility references and active
`CatalogueSession` untouched.

### Save transaction

1. Resolve the destination without changing the active session path.
2. If overwriting the current document, compare its on-disk SHA-256 revision with
   the revision captured at load/save time.
3. Copy the `DataSet` to a stable snapshot and normalize derived counters.
4. Write the snapshot to a uniquely named temporary file in the destination
   directory.
5. Reopen that temporary file through the same secure load path.
6. Compare every table, row, column, and value with the intended snapshot.
7. For a new file, move the verified temporary file into place. For an existing
   file, use same-volume replacement and retain the previous destination as
   `<catalogue>.bak`.
8. Recalculate the destination revision, update `CatalogueSession`, and clear
   dirty state only after replacement succeeds.

Cancellation, validation failure, I/O failure, and external modification all
return before session identity or dirty state changes. A best-effort cleanup
removes only the uniquely named temporary file created by the failed attempt.

### Failure contract

The adapter returns typed failures for missing files, excessive size, invalid
XML, missing/unsupported versions, invalid structure, constraint violations,
external changes, access denial, I/O errors, and verification failures. Forms
present these results; they do not interpret exceptions or XML themselves.

### Transition boundary

The adapter currently consumes the existing `DataSet` schema because this is a
strangler migration, not a format rewrite. The instance-owned
`ApplicationComposition` is the single WinForms adapter seam that owns the
active document and wires its typed repositories to the shared workspace. Forms
never access tables or rows. `DataSet` must not cross into `C3.Catalogue` or
`C3.Presentation.WinForms`; presentation consumes typed services instead.

Legacy mode continues to use this adapter after native-v2 exists. No native
element, ID, namespace, or extension is ever injected into a legacy Save.

## Native 2.0.0 candidate

`NativeXmlCatalogueReader` preflights the 64 MiB input with DTD/entity expansion
disabled and exact depth, element, attribute, and scalar limits. It then requires
the immutable namespace/profile, canonical element sequence, known attributes,
typed scalar lexemes, unique IDs, and a fully resolved in-document graph.
Unknown core or extension content is rejected rather than ignored.

`NativeXmlCatalogueWriter` is the only byte projection. It sorts entities by ID
through the model, emits fixed schema order, canonical UTC/decimal/Boolean text,
UTF-8 without BOM, LF, and no foreign content. Rewriting a canonical file is
byte-identical.

`NativeXmlCatalogueStore` owns path and revision behavior. It writes a unique
sibling temporary file, flushes it, reopens it through the same reader, rewrites
the reopened model and requires byte identity, then performs same-volume move or
replacement with a byte-exact backup. Expected-revision mismatch leaves the
destination untouched. Migration and UI call this store; they do not reproduce
its XML or transaction rules.

The [native-v2 ADR](decisions/0005-native-v2-format-and-migration.md) remains at
its candidate gate until migration, loss-aware export, CLI, recovery, both lanes,
and exact packages pass together.

## Format transitions and headless tooling

`LegacyToNativeMigrator` owns deterministic, read-only interpretation and stable
ID mapping. `LegacyToNativeConversionService` adds verified convert-copy,
deterministic reports, and a resumable journal; recovery revalidates both source
and destination revisions before proceeding. `NativeToLegacyExporter` owns the
reverse preview/export and reports native identity, provenance, timestamp, and
flattening losses before writing through the create-only legacy store.

`c3.exe` is intentionally a thin command shell over those services.
This lets tests, later WinForms workflows, and automation use one behavior owner
instead of carrying separate parser, migration, or export implementations.

## Post-Alpha-5 canonical adapter transition

The two implemented stores are mechanism owners, not two editable catalogue
truths. Under [ADR 0012](decisions/0012-canonical-catalogue-before-application-frontends.md),
Alpha 6 must adapt the complete legacy and native documents to one logical
catalogue in shadow/round-trip mode before canonical mutation becomes active.

The permanent direction is:

```text
legacy XML <-> v1.1 DataSet adapter ---+
                                       +-- logical CatalogueDocument
native XML <-> native profile adapter -+
```

`DataSet` remains legal only inside the v1.1 adapter and legacy
characterization. `Native*` types remain legal only as native profile
DTOs/projections. Neither may cross into Application or ordinary frontends.

The Alpha 4 native profile is immutable. If the canonical semantic audit needs
different aliases, temporal values, uncertainty, provenance, units, or
extensions, a distinct profile and explicit deterministic transition supersede
it. Existing schema, fixtures, reader/writer evidence, and migration vectors are
preserved.

Application save policy captures a versioned logical snapshot and destination
lease before invoking a profile store. Stores continue to own byte parsing,
temporary output, flush, verification, backup, replacement, and exact
`DiskRevision`; Application alone decides whether the committed snapshot is
still the current clean state. File-dialog confirmation is not a replacement
lease, and a failed store operation must not mutate active logical state.
