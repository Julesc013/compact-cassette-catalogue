# Infrastructure-library public API baseline v1

This directory versions the accepted public compiled surface of
`C3.Infrastructure`. It began as the 312-signature VB oracle for Alpha 3; Alpha
4 intentionally extends it to 358 signatures with the reviewed native reader,
writer, typed results, failure taxonomy, and transactional store.

`public-api.txt` is a deterministic reflection projection containing exported
types, enum values, constructors, properties, events, and ordinary methods. It
is a compatibility oracle, not an alternative implementation or a promise that
all current public types will remain part of the eventual native 2.0 API.

The normal test gate builds the assembly and compares it through the shared
reflection validator. An intentional API change requires an architecture record,
behavioral coverage, an explicit caller migration, and an explicitly regenerated
baseline. Do not edit the baseline to conceal an unexplained translation change.

Every further adapter or migration contract must carry executable evidence and
an explicit baseline update; generated surface drift remains a gate failure.
