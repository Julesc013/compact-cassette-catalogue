# C3 1.x to 2.x compatibility charter

Status: **Contract accepted; complete compatibility evidence not yet established**

“Fully backwards compatible with C3 1.x” means preserving the documented user
workflows and every file format actually emitted by the supported public 1.x
releases. It does not mean preserving internal types, form layouts, accidental
implementation details, or known defects.

The release-validation record must name the exact releases and artifacts tested.
Until that inventory and corpus exist, C3 describes compatibility as a target,
not a completed claim.

## Compatibility surfaces

| Surface | 2.0 obligation |
| --- | --- |
| Catalogue input | Open untouched supported 1.x catalogues into temporary state; failure leaves the active session and original file unchanged. |
| Legacy editing | Offer a clearly labelled v1.1 compatibility mode whose saves remain readable by the chosen 1.x baseline. |
| Native conversion | Convert to a new destination, retain the original, generate deterministic stable identities, and report every normalization. |
| Legacy export | Preview information that cannot be represented, require an explicit destination, and emit a loss report. |
| Preferences | Import supported 1.x values read-only into one shared 2.x profile, checkpoint a versioned outcome atomically, preserve sources, and keep transient failures retryable. |
| Updates | Never direct stable 1.x users to an unavailable 2.x preview; channel promotion occurs only after matching assets exist. |
| Distribution | Keep portable ZIPs authoritative; a setup product consumes the same payload and preserves side-by-side and rollback semantics. |
| Runtime lanes | Preserve identical catalogue behavior in x86/net40 and x64/net48; validate each minimum-OS claim independently. |
| Diagnostics | Preserve privacy, make collection user-visible, and distinguish format, migration, I/O, and application failures. |

## File modes

Opening a supported 1.x catalogue presents an explicit choice whenever native v2
behavior would change its representation:

1. **Legacy mode** keeps the catalogue on the v1.1 profile. Saving must not add
   native-v2 fields, identifiers, extension elements, or encodings.
2. **Convert a copy** creates a new native-v2 destination, leaves the original
   byte-for-byte untouched, and writes a migration report.
3. **Read-only inspection** remains available when C3 can parse the source but
   cannot safely preserve or convert all required information.

An ordinary Save never changes modes. “Save As” changes a path, not a format,
unless the user selects a separately named conversion/export command.

## Migration identity

Native v2 entities use stable opaque identifiers. Migration must be repeatable:
the same source identity and canonical source content produce the same mapping,
or a durable mapping artifact is retained and reused. Calling `Guid.NewGuid()`
independently on every conversion is insufficient because repeated migrations
would create unrelated catalogues.

Human-readable legacy codes remain data, aliases, or import keys; they do not
become mutable native primary keys. Ambiguous references stop conversion with a
specific issue rather than guessing.

## Legacy output profile

The initial 2.0 alpha continues to write catalogue format 1.1.0. Product producer
metadata such as `2.0.0 Alpha 1` is informational and must be tested against the
supported 1.x reader. Native v2 output is not claimed until its specification,
reader, writer, fixtures, migration engine, and recovery behavior pass together.

Once native v2 exists, v1.1 output has a named compatibility profile. The profile
defines permitted fields, ordering, encodings, enum values, timestamps, decimal
culture, relationship projections, and behavior for information that v1.1
cannot represent.

## Settings and side-by-side behavior

Product-version, executable-location, and CPU-lane changes can change the .NET
per-user configuration path. C3 2 therefore does not use that provider as its
live owner. Both lanes share the versioned product path
`%LOCALAPPDATA%\Jules Carboni\C3\2\preferences.xml`.

On first successful initialization, a bounded importer examines only known C3
1.x profile shapes and reads message preference, default directory, update
policy, and last update-check time. The values and a versioned
`imported`/`not-found`/`invalid` outcome are checkpointed together. Discovery,
access, lock, or save failures do not mark completion and remain retryable. C3
does not change or delete a 1.x `user.config`; an older 2.x executable likewise
does not quarantine or overwrite a future preference schema.

Future setup integration uses distinct 1.x and 2.x product identities so either
can be kept for rollback. File association takeover is opt-in. Uninstalling one
major line must not remove the other's binaries, settings, catalogues, backups,
or recovery data.

## Corpus and matrix

The compatibility corpus is organized by producer release and observed variant,
not only by nominal schema number. Each artifact records provenance without
containing private user data.

For each supported 1.x baseline, automation and manual evidence cover:

- untouched open and visible-value comparison;
- load failure isolation;
- legacy-mode save and reopen in 2.0;
- legacy-mode save and reopen in the original/baseline 1.x executable;
- deterministic convert-copy and repeated migration identity;
- v2 export preview, v1.1 reopen, and loss-report accuracy;
- x86/net40 and x64/net48 parity; and
- preference migration on first/repeated startup, transient failure retry,
  cross-lane sharing, and source-byte preservation.

Characterized behavior is classified as **required behavior**, **tolerated
legacy quirk**, or **defect to correct**. A characterization test does not turn a
historical bug into a permanent product requirement by itself.

## Compatibility change control

Any change to this charter, a supported baseline, the legacy writer, migration
identity, or a loss policy requires an ADR, fixtures, user-facing notes, and a
new release-validation result. Compatibility exceptions are explicit and narrow;
they are never hidden behind “best effort.”
