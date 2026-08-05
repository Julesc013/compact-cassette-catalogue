# Infrastructure-library public API baseline v1

This directory versions the accepted public compiled surface of
`C3.Infrastructure`. It began as the 312-signature VB oracle for Alpha 3; Alpha
4 intentionally extends it with reviewed native persistence and migration
contracts. The current bidirectional conversion boundary contains 470 signatures: strict
reader/writer, typed results and failures, transactional new-only storage,
deterministic mapping and reports, resumable recovery checkpoints, and the
read-only plus convert-copy legacy-to-native services. It also includes the
previewable, loss-aware native-to-v1.1 exporter and its create-only legacy
output contract.

`public-api.txt` is a deterministic reflection projection containing exported
types, enum values, constructors, properties, events, and ordinary methods. It
is a compatibility oracle, not an alternative implementation or a promise that
all current public types will remain part of the eventual native 2.0 API.

For C3 2.0 it is an **internal cross-project compatibility oracle**, not a
supported third-party binary SDK. External longevity belongs to versioned file,
process, report, and conformance contracts unless a later ADR explicitly accepts
a binary SDK.

The normal test gate builds the assembly and compares it through the shared
reflection validator. An intentional API change requires an architecture record,
behavioral coverage, an explicit caller migration, and an explicitly regenerated
baseline. Do not edit the baseline to conceal an unexplained translation change.

Every further adapter or migration contract must carry executable evidence and
an explicit baseline update; generated surface drift remains a gate failure.
