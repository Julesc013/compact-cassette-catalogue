# Compact Cassette Catalogue 1.2.1 Beta 1

C3 1.2.1 Beta 1 is the compatibility, persistence, and maintainability overhaul
of the existing C3 product. It keeps catalogue format 1.1.0 and ships the same
feature behavior through two Windows build lanes.

## Highlights

- Added a real x64/.NET Framework 4.8 lane while retaining the x86/.NET
  Framework 4.0 compatibility lane.
- Made catalogue loading temporary and secure: malformed, unsupported, or unsafe
  XML cannot partially replace the active catalogue.
- Made saving transactional with temporary-file verification, atomic replacement,
  backup recovery, and external-edit conflict detection.
- Added bounded diagnostic context and unhandled-exception report generation.
- Centralized brand, cassette-model, deck, and tape rules in typed services; the
  legacy `DataSet` and XML columns now live behind infrastructure adapters.
- Prevented deletion of referenced brands, models, and decks.
- Fixed incorrect deck counters, the model editor reading notes from the wrong
  table, Save As cancellation, duplicate Open dialogs, and recursive shutdown.
- Made bulk tape creation atomic, gave every created tape its correct sequence
  number, and stopped deleted sequence identifiers from being reused.
- Added format specifications, security/culture fixtures, 15 executable
  characterization tests, dependency checks, shared-project parity checks, PE
  architecture verification, and deterministic packaging.
- Reorganized sources by feature ownership and removed hidden default-form
  coordination from feature workflows.
- Centralized message, directory, and update-check preferences; legacy or unknown
  update policy values now normalize safely to `never`.

## Downloads

- `C3-v1.2.1-beta.1-win-x86-net40-portable.zip`
- `C3-v1.2.1-beta.1-win-x64-net48-portable.zip`
- `SHA256SUMS.txt`

Portable means no installer and no administrator requirement. Per-user settings
and diagnostics may still use Windows application-data locations.

## Requirements

| Build | Framework | Compatibility target |
| --- | --- | --- |
| x86 | .NET Framework 4.0 | Windows XP SP3 and later |
| x64 | .NET Framework 4.8 | Windows 7 SP1 and later |

The x86 package is the compatibility choice. The x64 package does not claim
Windows XP x64 support. Modern high-DPI behavior varies with operating-system
capabilities and is not guaranteed uniformly on Windows 7.

## Beta limitations and verification status

This candidate is not ready for publication until the manual workflow matrix and
minimum-OS checks in
[`release/validation/1.2.1-beta.1.md`](release/validation/1.2.1-beta.1.md)
are complete. Keep backups of important catalogues. The old installer and
uninstaller are retired; portable ZIPs are authoritative.

Catalogue format remains 1.1.0. Both builds must read and write the same files;
report any cross-build difference as a release-blocking defect.
