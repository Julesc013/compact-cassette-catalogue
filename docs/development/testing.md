# Testing C3

C3 uses layered evidence because no single test proves data safety, backwards
compatibility, UI usability, or operating-system support.

## Automated repository gate

```powershell
.\build\verify.ps1 -Rebuild
```

The gate verifies build/update metadata, module dependency direction, WinForms
data/settings boundaries, shared UI and linked-source parity, local documentation
links (including non-vacuous discovery in exported source trees), catalogue/domain/
settings characterization, both release builds, assembly/file/product identity for
every shipped binary, executable architecture, and diff whitespace.

The characterization build also reflects `C3.Catalogue.dll` and compares its
269 exported type/member signatures with
[`spec/catalogue-api/v1/public-api.txt`](../../spec/catalogue-api/v1/public-api.txt).
This freezes the VB migration oracle independently from behavior tests; neither
contract can be regenerated merely to make an unexplained port difference pass.

During the Alpha 3 mechanical port, an isolated C# 7.3 candidate compared every
admitted feature namespace with the same baseline. After its complete
269-signature surface matched, the project replaced the VB production assembly
atomically and the temporary harness was removed. The baseline continues to
protect the C# assembly from unintended API drift.

The same reflection mechanism freezes all 312 exported Infrastructure
signatures under `spec/infrastructure-api/v1`. It reflects layered assemblies
from an isolated copy of the library output directory so cross-assembly public
types resolve without locking the real outputs before the later rebuild gate.
The Infrastructure baseline was the independent API oracle for its Alpha 3
language-only migration and remains an ongoing compatibility alarm. The
complete 312-signature candidate was promoted atomically; the normal product
graph and all 68 characterization scenarios now execute the C# production
assembly, and the temporary candidate harness has been removed.

Tests target .NET Framework 4.0 so the reusable assemblies used by the XP lane
are exercised. The runner is deliberately dependency-light and returns a nonzero
process code on any failure.

## Compatibility corpus

Fixtures are immutable, privacy-safe evidence organized by producer/profile and
observed variant—not only by nominal schema version. The schema-validated
[`corpus.v1.json`](../../fixtures/compatibility/1x/corpus.v1.json) inventories
every public 1.x release and its exact official assets. It proves that all public
1.x producers wrote catalogue `1.1.0`; formats `1.0.0`, `1.0.1`, and `1.0.2`
belong to archival pre-1.x producers and are not silently included in the 1.x
support promise.

Each supported baseline eventually proves:

- new reader opens untouched old-writer output;
- load failure leaves active and original state unchanged;
- legacy-mode new writer output opens in both 2.0 and the baseline 1.x reader;
- convert-copy is deterministic and never changes the source;
- native-to-v1.1 export preview and loss report match baseline-reader results;
- both C3 runtime lanes produce the same logical model; and
- preference import succeeds once, preserves its source, and repeated startup is
  idempotent.

A characterized result is classified as required behavior, tolerated legacy
quirk, or defect. Never add a personal catalogue; construct the smallest
synthetic reproduction with a provenance/expected-result record.

The exact-binary compatibility laboratory is opt-in because historical EXEs are
hash-pinned but not redistributed in Git:

```powershell
.\build\fetch-compatibility-baselines.ps1
.\build\build.ps1 -Configuration Release
.\build\test-compatibility-baselines.ps1 -SkipBuild
```

The first command performs network I/O. The second and third run the complete
current-writer and native-export inputs through each old reader/writer, then
through the current reader, against verified local artifacts. The supported
Alpha 4 corpus currently produces 12 exact-binary rows: six historical artifacts
times two input profiles. See the
[evidence matrix](../compatibility/1x-evidence-matrix.md).

## Preferences, migration, and recovery

Preference tests cover exact allowlisted 1.x paths, Boolean/string historical
schemas, unchanged source bytes, candidate ordering/fallback, missing/invalid and
transient failures, atomic import markers, native schema/limits, backup recovery,
future-version downgrade safety, dirty-field merging, save/checkpoint retry, and
repeated launch. The canonical v1 example must pass both its XSD and the runtime
reader. Real profiles and both packaged EXEs still require isolated manual/VM
evidence.

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

## Brand workspace performance

Alpha 5 includes a bounded synthetic Brand measurement because the legacy
two-letter code space has a real maximum of 676 Brands. The normal
characterization gate applies conservative regression ceilings; the measurement
command reports the actual warm maxima used by candidate evidence:

```powershell
.\build\measure-brand-workspace.ps1 -Configuration Release
```

It measures 20 unfiltered refreshes, 20 filtered refreshes, and 20 editor
activations after setup and warm-up. This headless presenter measurement catches
algorithmic regressions but does not replace packaged paint, input-latency,
launch, memory, or minimum-OS measurements.

## Exact-package Brands workflow

After building and packaging, run the black-box Brands workflow against the
actual portable archives:

```powershell
.\build\test-packaged-brand-workspace.ps1 -SkipPackage
```

Omit `-SkipPackage` to regenerate packages from the existing build outputs first.
The driver derives package names and lanes from the authoritative release
identity and lane manifests, extracts each ZIP to a unique temporary root, and
uses the Windows accessibility surface to exercise real executable controls. It
performs create, edit, filter, confirmed delete, undo, redo, and save separately
in both lanes, then reopens both saved catalogues in both lanes. Successful runs
remove their temporary catalogues and payloads; failed runs retain the exact
temporary root for diagnosis. `-KeepWork` retains successful evidence when a
release investigation needs it.

The same run measures package startup, opening Brands, observable command
response, a native window paint, and process peak working set. The safety
ceilings intentionally catch catastrophic regressions rather than asserting a
universal performance promise. Record repeated frozen-candidate values and the
host profile before accepting or tightening a product budget. Accessibility-tree
retirement after a native file or confirmation dialog is not application response
time and is kept outside the command measurement.

This is an interactive desktop gate: it requires an unlocked user session and
may briefly foreground C3 and Windows file dialogs. It is intentionally separate
from the headless default verifier. Candidate evidence must name the archive and
executable hashes printed by the command.

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
