# Implemented legacy catalogue persistence

`C3.Infrastructure.CatalogueFiles.Xml.V1_1.LegacyXmlCatalogueStore` is the only
component permitted to read or write the legacy v1.1 XML format. WinForms owns
file dialogs and messages; catalogue rules do not know about files or XML.

## Load transaction

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

## Save transaction

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

## Failure contract

The adapter returns typed failures for missing files, excessive size, invalid
XML, missing/unsupported versions, invalid structure, constraint violations,
external changes, access denial, I/O errors, and verification failures. Forms
present these results; they do not interpret exceptions or XML themselves.

## Transition boundary

The adapter currently consumes the existing `DataSet` schema because this is a
strangler migration, not a format rewrite. `LegacyGlobalState` is the single
WinForms composition seam that owns the active document and wires its typed
repositories. Forms never access tables or rows. `DataSet` must not cross into
`C3.Catalogue`, and new WinForms source must consume typed services instead.

This document describes only implemented catalogue 1.1.0 persistence. The
[native-v2 ADR](decisions/0005-native-v2-format-and-migration.md) and
[migration design](../migration/catalogue-1.1-to-2.0.md) are proposed contracts,
not current reader/writer claims. Legacy mode continues to use this adapter even
after a native profile exists.
