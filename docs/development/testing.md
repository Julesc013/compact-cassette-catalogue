# Testing C3

C3 uses layered evidence because no single test proves compatibility.

## Automated gate

Run from Windows PowerShell:

```powershell
.\build\verify.ps1 -Rebuild
```

The gate verifies metadata projections, module dependency direction, shared
WinForms source parity, local documentation links, catalogue fixtures, domain
and adapter characterization, both release builds, executable architecture, and
diff whitespace.

Tests target .NET Framework 4.0 so the same domain and infrastructure binaries
used by the XP lane are exercised. The runner is deliberately dependency-light
and returns a non-zero process code on any failure.

## Fixture policy

Fixtures under `fixtures/catalogues/v1.1.0` are immutable compatibility evidence.
Add the smallest safe fixture for a newly discovered edge case. Never copy a
user catalogue into the repository; construct a minimal synthetic example.

Valid fixtures must pass the format schema. Invalid, security, and culture
fixtures must fail or normalize for the documented reason. A format behavior
change requires updating the language-neutral specification before its adapter.

## Manual workflows

Before a release, exercise these workflows independently in each build lane:

1. start with no catalogue and create a new one;
2. add, edit, filter, and delete brands, models, decks, and tapes;
3. save, close, reopen, and compare all visible values;
4. use Save As and verify cancellation leaves state unchanged;
5. attempt invalid, unsupported, and externally modified input;
6. close with clean, dirty, and uncommitted editor state; and
7. reopen settings and confirm persistence.

Record the OS, framework, commit, commands, artifact hashes, result, and any
limitations in `release/validation/<version>.md`. “Build passed” is not runtime
or high-DPI evidence.

## UI changes

Open every changed form in the Visual Studio 2019 designer. Smoke test keyboard
navigation, default/cancel actions, empty data, long values, and supported scale
factors. Designer serialization changes belong in a separate commit when they do
not implement the same user-visible change.
