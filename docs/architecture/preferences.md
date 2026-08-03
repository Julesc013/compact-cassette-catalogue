# Preference ownership, migration, and recovery

C3 2 uses one product-owned preference lifecycle. The framework target, CPU
lane, executable location, and installation method do not choose a different
profile.

## Responsibility map

| Responsibility | Owner |
| --- | --- |
| Current values and dirty fields | `UserPreferencesService` |
| Native XML, limits, lock, merge, backup, atomic replace | `XmlUserPreferencesStore` |
| Known historical path allowlist | `LegacySettingsProfileLocator` |
| Read-only historical parsing | `LegacySettingsProfileReader` |
| Candidate ordering and fallback policy | `LegacyUserSettingsImporter` |
| Windows paths and diagnostics composition | `UserPreferencesFactory` |
| Native wire contract | `spec/preferences/v1` |
| Historical schemas and provenance | `fixtures/settings` |

Forms use typed properties and `TrySave`; they do not know file paths or XML.
The legacy importer is a one-way boundary and is never a second live store.

## Native files

```text
%LOCALAPPDATA%\Jules Carboni\C3\2\
  preferences.xml                  current profile
  preferences.xml.bak              last replaceable profile
  preferences.lock                 persistent cross-process lock file
  .bad-yyMMddHHmmssXXX.xml          quarantined invalid primary, if recovery ran
```

A save takes the cross-process lock, reloads the current profile, merges only
the caller's dirty fields, writes a same-directory temporary file, flushes it to
disk, reopens and verifies it, then uses `File.Replace` when a primary exists.
The in-memory dirty fields clear only after success.

Temporary and recovery sibling names are deliberately compact so a valid
destination near the classic Windows 259-character path boundary does not become
unsavable merely because a transactional filename is longer. Recovery names
retain a UTC timestamp to the second; the final three URL-safe characters and
create-only retry distinguish concurrent recoveries without embedding or
duplicating the preference filename.

Initialization holds the same lock across this complete transaction:

```text
load primary
  -> classify supported / missing / invalid / inaccessible / future
  -> inspect backup when safe
  -> quarantine invalid primary before replacement
  -> run pending bounded 1.x import
  -> normalize known historical values
  -> checkpoint values and import outcome atomically
```

Unexpected or transient failures leave initialization incomplete. C3 continues
with explicit temporary in-memory values, warns the user, and retries before a
later save. Unsupported future schemas are read-only from the older process's
perspective.

## Historical discovery boundary

Discovery never recursively scans LocalAppData. It accepts the known full and
25-character-truncated C3 application roots, then the exact .NET Framework
evidence shape using the truncated C3 friendly-name stem, an allowlisted
`Url`/`Path`/`StrongName` evidence kind, a 32-character framework hash, a four-part
0.x/1.x version directory, and `user.config` at that exact depth. Reparse points,
lookalikes, deeper paths, and 2.x profiles are rejected.

Candidates are ordered by assembly version, last-write time, and path. Invalid
content can fall back with evidence; an unavailable candidate stops the import
so a transient failure cannot make stale settings win. Imported source bytes are
never changed.

## Compatibility and evolution

The current import covers message confirmations, default catalogue directory,
update policy, and last update-check time. Boolean historical update values map
`True` to startup and `False` to never. The exact historical My Documents
expression is normalized; legitimate paths merely beginning `My.` are preserved.

Native schema and legacy-import marker versions are independent. A new schema or
marker requires a reader decision, fixtures, downgrade-preservation tests, user
notes, and an ADR when compatibility policy changes.
