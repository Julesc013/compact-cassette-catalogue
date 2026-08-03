# Migrating from C3 1.x

Status: **Alpha 1 safety guidance; native-v2 conversion is not implemented**

C3 2.0 is designed to coexist with C3 1.x and preserve supported legacy
catalogues. During Alpha 1 the product still writes catalogue format 1.1.0; there
is no native-v2 conversion command yet.

## Before trying a preview

1. Keep the original 1.x application/package.
2. Make an independent copy of every catalogue you intend to test.
3. Extract the 2.0 portable build to a different directory.
4. Record the exact version, channel, lane, Windows version, and test-copy hash.
5. Do not associate all XML files with a preview build.

Use only a package whose SHA-256 matches the manifest on its GitHub release. A
build or feed visible on `dev` is not automatically a published package.

## Settings

On first 2.0 launch, C3 asks the .NET settings provider to import the previous
profile, normalizes known values, checkpoints them, and marks the upgrade
complete. Repeated launch does not import again. C3 does not delete the 1.x
profile.

Current migrated settings are message preference, default catalogue directory,
update policy, and last update-check time. If migration fails, C3 records a
diagnostic warning and keeps the retry marker armed. Preserve the diagnostic
report and describe whether this was first or repeated launch; do not share
`user.config` without removing private paths.

## Catalogues in Alpha 1

Alpha 1 operates on the legacy 1.1 profile. Keep a test copy because the complete
matrix of every public 1.x producer is still being assembled. Cross-build or
old-reader differences are release-blocking defects.

Future native-v2 builds will make the choice explicit:

- **Legacy mode:** continue writing the named v1.1 compatibility profile.
- **Convert a copy:** write a new native destination and migration report while
  leaving the original unchanged.
- **Read-only:** inspect a source that cannot yet be preserved safely.
- **Export to 1.1:** preview and report information that cannot be represented.

Open, Save, and Save As will never silently change modes.

## Rollback

To stop testing Alpha 1, close it, keep its diagnostic/profile data for a bug
report if needed, and reopen the untouched/test legacy catalogue with the retained
1.x build. Do not expect uninstalling/replacing application binaries to reverse a
future native catalogue migration; rollback uses the preserved original or an
explicit verified legacy export.

See the engineering [compatibility charter](../compatibility/1x-to-2x-charter.md)
for the exact evidence required before stable compatibility is claimed.
