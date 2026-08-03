# Testing C3

C3 uses layered evidence because no single test proves data safety, backwards
compatibility, UI usability, or operating-system support.

## Automated repository gate

```powershell
.\build\verify.ps1 -Rebuild
```

The gate verifies build/update metadata, module dependency direction, WinForms
data/settings boundaries, shared UI and linked-source parity, local documentation
links, catalogue/domain/settings characterization, both release builds, assembly/
file/product identity for every shipped binary, executable architecture, and diff
whitespace.

Tests target .NET Framework 4.0 so the reusable assemblies used by the XP lane
are exercised. The runner is deliberately dependency-light and returns a nonzero
process code on any failure.

## Compatibility corpus

Fixtures are immutable, privacy-safe evidence organized by producer/profile and
observed variant—not only by nominal schema version. Current fixtures cover
catalogue 1.1.0; Alpha 2 must inventory other public 1.x formats and deviations
before “full 1.x compatibility” is claimed.

Each supported baseline eventually proves:

- new reader opens untouched old-writer output;
- load failure leaves active and original state unchanged;
- legacy-mode new writer output opens in both 2.0 and the baseline 1.x reader;
- convert-copy is deterministic and never changes the source;
- native-to-v1.1 export preview and loss report match baseline-reader results;
- both C3 runtime lanes produce the same logical model; and
- settings import succeeds once and repeated startup is idempotent.

A characterized result is classified as required behavior, tolerated legacy
quirk, or defect. Never add a personal catalogue; construct the smallest
synthetic reproduction with a provenance/expected-result record.

## Settings, migration, and recovery

Settings tests cover previous, missing, malformed, and current profiles; import
ordering; retry markers; normalization; save failure; repeated launch; and
side-by-side 1.x/2.x behavior. Framework-level fakes prove coordinator logic,
while real profile directories and both EXEs require isolated manual/VM evidence.

Migration/export tests cover dry runs, deterministic identity maps, ambiguity,
normalization, unknown/critical extensions, interrupted writes, destination
conflicts, reports, rollback, and old-reader verification. Fault injection must
target an owned temporary directory and preserve the known-good source.

## Manual workflows

For each lane and candidate:

1. launch/close and first/repeated settings startup;
2. new/open/save/save-as and cross-lane reopen;
3. add/edit/filter/delete every accepted record type;
4. clean, dirty, unapplied-editor, undo/redo, and recovery close paths;
5. invalid, unsupported, oversized, malicious, externally modified, and
   interrupted catalogue operations;
6. legacy/native/migration/export modes applicable to that milestone; and
7. packaged/downloaded-artifact launch rather than only a build-tree EXE.

Record OS, framework, source commit, artifact hash, steps, result, and limitation
in `release/validation/<version>.md`.

## UI, accessibility, DPI, and performance

Open every changed form in the authoritative Visual Studio designer. Test
keyboard-only completion, focus restoration, mnemonics, default/cancel behavior,
accessible names/roles, system high contrast, empty/error/long data, and 100%,
125%, 150%, and 200% scaling. Do not infer an operating-system capability from a
newer development host.

Performance uses named synthetic catalogue sizes and machine profiles. Capture
launch, open/save, list/filter, editor, bulk preview/apply, memory high-water, and
recovery-scan baselines before accepting numeric budgets. Optimize the measured
owner; do not introduce duplicate cached truth to meet an arbitrary target.
