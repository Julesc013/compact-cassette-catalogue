# Compact Cassette Catalogue 2.0.0 Alpha 1

C3 2.0.0 Alpha 1 establishes the safe product and engineering boundary for the
long-term overhaul. It reclassifies the unpublished 1.2.1 candidate; no historical
release, package, hash, or validation record has been relabelled.

This alpha is currently a development candidate, not an advertised download.
Its [validation record](release/validation/2.0.0-alpha.1.md) remains blocked until
the refreshed automated, manual, settings-profile, and minimum-OS evidence is
complete.

## Implemented foundation

- Preserves one product and one shared source tree across x86/.NET Framework 4.0
  and x64/.NET Framework 4.8 lanes.
- Retains the legacy catalogue 1.1.0 writer; C3 product version 2.0 does not imply
  a native-v2 catalogue file.
- Separates the root 1.x update feed from the opt-in 2.x alpha feed so existing
  users cannot be offered an unavailable preview.
- Gives every shipped EXE and DLL consistent assembly, file, and informational
  identity and verifies it before packaging.
- Imports supported previous-version user preferences once, normalizes them,
  persists a retry-safe completion marker, and characterizes repeat/failure paths.
- Securely loads catalogue XML into temporary state and rejects DTD/external
  entity input, malformed structure, unsupported versions, and oversized files.
- Saves through verified temporary output, external-edit detection, atomic
  replacement, and recoverable backup behavior.
- Centralizes typed brand, model, deck, and tape rules outside forms while keeping
  the historical `DataSet` behind one versioned infrastructure seam.
- Provides bounded diagnostics, crash reports, deterministic portable packaging,
  SHA-256 manifests, PE checks, dependency/UI boundaries, and shared-lane parity.
- Publishes the 2.0 product vision, compatibility charter, staged C# strategy,
  OEM+ UI contract, migration design, and AIDE/Universal Setup boundaries.

## Deliberately not implemented yet

- The complete public C3 1.x compatibility corpus and baseline-reader matrix.
- Stable opaque native identities and the native-v2 reader/writer.
- Convert-copy migration, legacy-mode UX, and loss-aware v1.1 export.
- The headless validator/migrator CLI.
- Command history, undo/redo, multi-document-ready workspace state, or the OEM+
  replacement shell.
- A Universal Setup binding or operational AIDE integration.
- Full conversion of managed production source from VB to C# 7.3.

These are ordered programme milestones, not hidden Alpha 1 claims. See the
[execution plan](docs/planning/2.0-execution-plan.md).

## Candidate packages

When the candidate is frozen, the packaging gate produces:

- `C3-v2.0.0-alpha.1-win-x86-net40-portable.zip`
- `C3-v2.0.0-alpha.1-win-x64-net48-portable.zip`
- `SHA256SUMS.txt`

Do not publish or mirror locally generated files until their source commit,
sizes, hashes, reproducibility comparison, and post-download verification are in
the validation record.

## Runtime contract

| Build | Framework | Intended compatibility lane |
| --- | --- | --- |
| x86 | .NET Framework 4.0 | Windows XP SP3 and later |
| x64 | .NET Framework 4.8 | Windows 7 SP1 and later, 64-bit |

These are target boundaries, not current Microsoft support claims. The x64 build
does not claim Windows XP x64 support. Enhanced DPI behavior depends on operating
system capabilities and must not be generalized from a modern development host.

## Alpha precautions

Keep independent backups, preserve original 1.x catalogues, and use an explicit
copy for preview testing. Alpha 1 continues to write legacy format 1.1.0, but the
complete old-reader matrix and real historical settings-profile migration remain
release gates. Report any cross-lane difference, data loss, silent normalization,
or incorrect update-channel behavior as release blocking.
