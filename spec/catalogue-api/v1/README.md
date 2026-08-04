# Catalogue-library public API baseline v1

This directory freezes the public compiled surface of the VB
`C3.Catalogue` assembly at the start of the Alpha 3 C# migration.

`public-api.txt` is a deterministic reflection projection containing exported
types, enum values, constructors, properties, events, and ordinary methods. It
is an oracle and compatibility alarm, not a new application API or a second
owner of catalogue behavior. The production source and characterization tests
remain authoritative for semantics.

The normal test gate builds the assembly and runs:

```powershell
.\build\validate-catalogue-api.ps1 -Configuration Release
```

An intentional API change requires all of the following in one reviewed slice:

1. explain the compatibility effect and migration in the relevant ADR;
2. add or update behavioral/differential characterization;
3. make one implementation the sole production owner; and
4. regenerate the baseline explicitly with `-WriteBaseline` only after review.

Do not edit the baseline to hide an unexplained difference. A mechanical C#
port must first reproduce the frozen signatures and behavior; later native-2.0
contracts evolve through their own versioned boundary.
