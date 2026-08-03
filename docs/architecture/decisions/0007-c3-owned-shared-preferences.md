# ADR 0007: Own one stable shared C3 2 preference profile

- Status: Accepted
- Date: 2026-08-04

## Context

C3 1.x used Visual Basic `My.Settings`, backed by .NET Framework's
`LocalFileSettingsProvider`. Its `user.config` location depends on application
identity, executable evidence, installation path, and assembly version. A moved
portable executable or the other C3 build lane can therefore receive a different
profile even for the same Windows user.

Calling `ApplicationSettingsBase.Upgrade()` only searches the provider's
previous-version path family. It cannot reliably discover every supported C3 1.x
portable location, and keeping it as the 2.0 owner would allow the x86 and x64
lanes to diverge.

## Decision

C3 2 owns one versioned XML preference profile for both build lanes:

```text
%LOCALAPPDATA%\Jules Carboni\C3\2\preferences.xml
```

`UserPreferencesService` is the in-memory owner. `XmlUserPreferencesStore` owns
locking, parsing, validation, dirty-field merging, verified temporary output,
durable flush, atomic replacement, backup, quarantine, and recovery.
`UserPreferencesFactory` is the WinForms composition boundary. Forms do not use
`My.Settings`, parse profile XML, or discover historical files.

On the first successful 2.0 initialization, a bounded importer examines only
known C3 1.x `LocalFileSettingsProvider` directory shapes. It reads sources
without writing, deleting, renaming, or normalizing them. It imports supported
values, records a versioned outcome in the same atomic checkpoint, and does not
run again for that migration version.

Malformed content may fall back to an older valid candidate with retained
diagnostic evidence. Access, locking, security, discovery, or checkpoint failures
do not record completion and remain retryable. A future native preference schema
is reported as unsupported and is never quarantined or overwritten by an older
C3 executable.

The generated settings sources, configuration sections, upgrade coordinator,
and `SaveMySettingsOnExit` behavior are removed. The build gate rejects their
reintroduction.

## Consequences

- x86/net40 and x64/net48 observe one coherent preference state.
- Moving the portable application no longer creates a new 2.0 profile.
- Concurrent instances merge only fields they changed while holding a
  cross-process lock.
- C3 1.x remains independently runnable and its profiles remain untouched.
- A binary-only portable ZIP does not imply a portable settings profile. An
  explicit future portable-profile mode needs its own identity, locking,
  migration, and security design.
- The preference schema is versioned independently from product and catalogue
  formats under `spec/preferences/v1`.

## Rejected alternatives

- Keep `My.Settings` and call `Upgrade()`: cannot meet moved-portable and
  cross-lane discovery requirements.
- Give each lane its own profile: creates inconsistent user behavior and lost
  updates.
- Recursively scan LocalAppData: crosses the product privacy and ownership
  boundary.
- Copy or delete a legacy `user.config`: harms rollback and side-by-side use.
- Treat an unknown future schema as corrupt: permits destructive downgrade.

## References

- [Microsoft `LocalFileSettingsProvider.Upgrade`](https://learn.microsoft.com/en-us/dotnet/api/system.configuration.localfilesettingsprovider.upgrade?view=netframework-4.8.1)
- [Microsoft `LocalFileSettingsProvider`](https://learn.microsoft.com/en-us/dotnet/api/system.configuration.localfilesettingsprovider?view=netframework-4.8.1)
- [Preference architecture](../preferences.md)
- [Preference format v1](../../../spec/preferences/v1/README.md)
