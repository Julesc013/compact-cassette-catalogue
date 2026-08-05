# Catalogue-library public API baseline v1

This directory versions the accepted public compiled surface of the
`C3.Catalogue` assembly. It began as the VB oracle for the Alpha 3 C# migration;
Alpha 4 deliberately extends the same contract with the typed native-v2 model
after ADR 0009, the candidate format invariants, and executable graph tests.

`public-api.txt` is a deterministic reflection projection containing exported
types, enum values, constructors, properties, events, and ordinary methods. It
is an oracle and compatibility alarm, not a new application API or a second
owner of catalogue behavior. The production source and characterization tests
remain authoritative for semantics.

For C3 2.0 this is classified as an **internal cross-project compatibility
oracle**, not a supported third-party binary SDK. Public longevity is promised
through the file/process schemas, stable result codes, and language-neutral
fixtures unless a later ADR deliberately accepts an SDK surface.

The Alpha 6 canonical document/query resource contract deliberately expands the
accepted internal surface to 524 signatures under ADR 0015. It remains a
non-production contract until the complete graph, adapter, and differential
gates pass.

The normal test gate builds the assembly and runs:

```powershell
.\build\validate-catalogue-api.ps1 -Configuration Release
```

An intentional API change requires all of the following in one reviewed slice:

1. explain the compatibility effect and migration in the relevant ADR;
2. add or update behavioral/differential characterization;
3. make one implementation the sole production owner; and
4. regenerate the baseline explicitly with `-WriteBaseline` only after review.

Do not edit the baseline to hide an unexplained difference. The Alpha 3
mechanical port first reproduced all 269 legacy signatures. The Alpha 4 native
model expands the accepted surface to 398 signatures, and ADR 0015 expands the
canonical resource contract to 524; later changes still need their own explicit
contract-and-test slice.
