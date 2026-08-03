# Catalogue 1.1 to native v2 migration design

Status: **Design contract; native v2 migration is not implemented**

This document owns migration behavior. The
[compatibility charter](../compatibility/1x-to-2x-charter.md) owns the user
promise, and the versioned specifications own their respective file syntax.

## User workflow

1. C3 securely loads the source into temporary legacy state.
2. It identifies the producer/version variant and validates all relationships.
3. A dry run constructs the native model and an issue/mapping report.
4. The user resolves blocking ambiguity or cancels without any changed file.
5. C3 writes a new destination transactionally and reopens it through the native
   reader.
6. It compares the intended model with the reopened model.
7. C3 records the source revision, migration profile, mappings, normalizations,
   warnings, and destination revision.
8. Only then may the new catalogue become the active document.

The original remains untouched. Migration is never hidden inside Open, Save, or
Save As.

## Mapping rules

- Stable IDs are deterministic from a versioned migration namespace plus
  canonical source identity, or are persisted in a mapping artifact reused by
  subsequent runs.
- A legacy tape code, brand code, model identifier, or deck code becomes a legacy
  alias/import key, not the native primary identity.
- Display-name relationships are resolved only when unique. Ambiguity is a
  blocking issue with candidate details.
- Dates, decimals, enum values, empty strings, and absent values normalize under
  versioned invariant rules.
- Derived counters are discarded and recomputed from accepted entities.
- Unknown or malformed content is rejected or explicitly preserved in a declared
  extension envelope; it is never silently ignored.

## Migration report

Reports have a stable machine-readable form and an accompanying readable summary.
They contain no catalogue content beyond what is needed to identify an issue.
At minimum they record:

- source/destination paths in the local report only;
- source and destination SHA-256 revisions;
- reader, migration, and writer profile versions;
- entity counts and stable-ID mappings;
- applied normalizations;
- warnings, blocking errors, and user decisions; and
- information that would be lost by legacy export.

## Failure and restart

Temporary outputs use unique names below the chosen destination directory and
are cleaned only when their ownership is proven. A process interruption leaves
the source intact and a recovery record that distinguishes safe cleanup from a
candidate destination. Re-running the same migration must not create unrelated
identities.

## Export back to 1.1

Export performs a dry run first. It classifies each difference as exactly
representable, normalized, omitted with user approval, or blocking. The user
chooses a new output path. C3 writes and verifies the v1.1 file, opens it with the
baseline reader harness, and retains the loss report beside the export when the
user requests it.
