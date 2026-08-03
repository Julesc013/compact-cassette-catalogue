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

On first successful 2.0 launch, C3 searches only the known C3 1.x per-user profile
locations, reads the newest usable profile without changing it, normalizes known
historical values, and checkpoints the result into the shared C3-owned profile:

```text
%LOCALAPPDATA%\Jules Carboni\C3\2\preferences.xml
```

Both the x86 and x64 builds use this file, including when the portable executable
is moved. Repeated launch does not import again. C3 never deletes, renames, or
rewrites the 1.x `user.config`.

Current migrated values are message preference, default catalogue directory,
update policy, and last update-check time. A malformed newest profile may fall
back to an older valid C3 profile and record diagnostic evidence. Access, lock,
discovery, or checkpoint failures show a warning, keep completion pending, and
retry before a later save. C3 preserves invalid native preferences under a
timestamped quarantine name when safe recovery occurs and can restore its last
known-good `.bak` file.

Preserve diagnostic/recovery files when reporting a problem and describe whether
this was first or repeated launch. Preference files can contain private local
paths; review or redact them before sharing.

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
