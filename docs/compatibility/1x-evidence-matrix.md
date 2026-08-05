# C3 1.x compatibility evidence matrix

This matrix distinguishes existing historical evidence from proof still needed
for C3 1.3.0. Exact public artifact metadata is maintained in
`fixtures/compatibility/1x/corpus.v1.json`; catalogue examples live under
`fixtures/catalogues/v1.1.0`; settings examples live under `fixtures/settings`.

## Producer baseline

| Producer | Catalogue format | 1.3 obligation | Current evidence |
| --- | --- | --- | --- |
| `v1.0.0` | 1.1.0 | Read and preserve represented values | Public artifact provenance recorded; execution pending |
| `v1.1.0` | 1.1.0 | Read and preserve represented values | Public artifact provenance recorded; execution pending |
| `v1.1.1` | 1.1.0 | Read and preserve represented values | Public artifact provenance and settings fixture recorded; execution pending |
| `v1.1.2` | 1.1.0 | Stable compatibility oracle; read 1.3 output | Public artifact provenance/settings fixture recorded; bidirectional execution pending |
| `v1.2.0b1` x86 | 1.1.0 | Direct behavioural baseline; read 1.3 output | Manually qualified release evidence and official SHA-256 recorded |
| `v1.2.0b1` x64 | 1.1.0 | Direct x64/net40 baseline; read 1.3 output | Manually qualified release evidence and official SHA-256 recorded |

The official 1.2 x64/net40 executable remains a compatibility-laboratory input,
not a C3 1.3 release lane. The active 1.3 readers are exactly x86/net40,
x64/net48, and native ARM64/net481 under the
[ratified matrix](../governance/1.3.0-three-lane-matrix-2026-08-05.md).

Older prerelease producers in the corpus are inventory-only unless a retained
real catalogue demonstrates a compatibility requirement not represented by the
supported producer set.

## Reader/writer matrix to execute

| Writer | v1.1.2 reader | v1.2.0b1 x86 reader | v1.2.0b1 x64 reader | 1.3 readers |
| --- | --- | --- | --- | --- |
| Canonical fixture | Required | Required | Required | Every lane |
| v1.1.2 | Self-check | Required | Required | Every lane |
| v1.2.0b1 x86 | Required | Self-check | Required | Every lane |
| v1.2.0b1 x64 | Required | Required | Self-check | Every lane |
| 1.3 x86/net40 | Required | Required | Required | Every lane |
| 1.3 x64/net48 | Required | Required | Required | Every lane |
| 1.3 ARM64/net481 | Required | Required | Required | Every lane |

Every cell means open, inspect key values, perform a copy-only save where safe,
reopen, and compare identifiers, dates, notes, recordings, relationships, table
order, column order, and primitive XML representations. Tests use copies only.

## Settings transitions

| Source profile | Target | Required result |
| --- | --- | --- |
| v1.0.0 two-field | 1.3 x86/net40 | known preferences retained; missing values take safe defaults |
| v1.1.1 Boolean update policy | 1.3 x86/net40 | policy normalizes without startup network access |
| v1.1.2 scheduled update | 1.3 x86/net40 | message, directory, schedule, and timestamp retained |
| v1.2.0b1 x86 | 1.3 x86/net40 | all four settings retained |
| v1.2.0b1 x64/net40 | 1.3 x64/net48 | all four settings retained |
| 1.3 x86/net40 | 1.3 x64/net48 | all settings retained |
| 1.3 x64/net48 | 1.3 ARM64/net481 | all settings retained |
| absent profile | every lane | clean defaults, no network request |
| corrupt/inaccessible profile | every lane | startup succeeds; retry remains armed |

Real profile tests must run under isolated disposable Windows user profiles.
Fixture validation alone does not prove `My.Settings.Upgrade()` path discovery.

## Current recovery evidence

- `RELEASE_VALIDATION_1.2.0.md` identifies the manually validated source commit,
  MSBuild 14.0.25420.1, x86/x64 PE results, smoke tests, workflow tests, settings
  persistence, blocked-network behaviour, and exact hashes.
- `build/download-baseline-assets.ps1` retrieves the two official v1.2 tagged
  EXEs and verifies both their sizes and recorded SHA-256 values.
- `build/prepare-historical-gate1-runtime-kit.ps1` also verifies the official
  v1.1.2 oracle and reproducibly packages all three oracles plus the canonical
  catalogue for the closed Gate 1 operator record.
- `build/validate-compatibility-corpus.ps1` checks provenance and fixture hashes.
- `build/test.ps1` builds and runs the safe standalone XML characterization tests.
- `build/validate-baseline-genome.ps1` prevents drift from the qualified program
  while compatibility work is implemented.

None of the pending execution cells may be marked passed from source inspection
alone.
