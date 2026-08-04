# Infrastructure-library public API baseline v1

This directory freezes the public compiled surface of the VB
`C3.Infrastructure` assembly at the start of the Alpha 3 C# migration.

`public-api.txt` is a deterministic reflection projection containing exported
types, enum values, constructors, properties, events, and ordinary methods. It
is a compatibility oracle, not an alternative implementation or a promise that
all current public types will remain part of the eventual native 2.0 API.

The normal test gate builds the assembly and compares it through the shared
reflection validator. An intentional API change requires an architecture record,
behavioral coverage, an explicit caller migration, and an explicitly regenerated
baseline. Do not edit the baseline to conceal an unexplained translation change.

The C# port must reproduce this surface and the executable characterization
suite before it can replace the VB production assembly.
