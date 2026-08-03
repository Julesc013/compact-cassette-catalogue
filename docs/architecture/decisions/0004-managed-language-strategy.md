# ADR 0004: Move the managed implementation toward C# 7.3 by proven slices

- Status: Accepted
- Date: 2026-08-04

## Context

C3 must retain .NET Framework 4.0 for its Windows XP lane and .NET Framework 4.8
for its modern lane. Microsoft assigns .NET Framework projects C# 7.3 as their
supported language version. The installed Visual Studio 2017 Enterprise 15.9
toolchain can compile both lanes and C# 7.3; Visual Studio 2022 and newer cannot
build the .NET Framework 4.0 target.

The current VB implementation is working compatibility evidence. Translating it
while redesigning domain semantics, persistence, and the UI in the same change
would make defects difficult to attribute and would remove the comparison oracle.

## Decision

C# 7.3 is the target language for new reusable managed 2.0 production code and
for behavior-preserving ports. Every C# project explicitly sets:

```xml
<LangVersion>7.3</LangVersion>
```

Do not use `latest`, `preview`, a compiler package that changes the supported
language/runtime contract, or syntax unavailable to the pinned compiler.

Port in dependency order after public contracts and characterization tests are
stable:

1. catalogue/domain slices;
2. infrastructure adapters;
3. validator/migrator composition; and
4. WinForms presentation slices.

A port changes language and assembly plumbing only. Redesign occurs in a later
reviewable change against both implementations. During transition, each behavior
has one production owner; the other implementation is an oracle or test fixture,
not a second independently evolving product path.

The intended managed end state is one C# implementation. A zero-VB tree is not
an unconditional C3 2.0 stable gate: a proven VB presentation slice may remain
temporarily if replacing it would reduce compatibility, accessibility, designer
reliability, or release safety. Its ownership and exit test must be recorded.

## Native languages

C11 is appropriate only in Universal Setup or a very small bootstrap/native
adapter with its own repository contract. C++11 is permitted only for an isolated
boundary justified by profiling or an unavailable OS API. Neither language owns
catalogue rules, file semantics, migrations, commands, settings, or WinForms
behavior.

All native boundaries expose a narrow C ABI or process protocol, validate input,
declare ownership and error behavior, and have managed fallbacks where practical.

## Consequences

- Contributors gain a broadly understood, explicitly pinned managed language.
- The compatibility lane determines the available runtime APIs even when a newer
  compiler could parse newer syntax.
- Migration remains incremental, bisectable, and reversible.
- Temporary VB/C# coexistence has a cost, so each port has a bounded parity gate
  and the old production source is removed after promotion.
- Future .NET or alternate native hosts consume language-neutral specifications
  and protocols rather than the net40 assembly as a permanent SDK.

## Rejected alternatives

- Rewrite everything in C# at once: removes the oracle and combines translation
  with behavior change.
- Keep all new code in VB indefinitely: supported, but works against the desired
  contributor reach and eventual implementation consolidation.
- Use modern C# against .NET Framework by overriding compiler defaults: creates
  an unsupported language/TFM combination and toolchain drift.
- Rewrite the domain in C or C++: adds memory-safety, interop, and contribution
  costs without solving a measured product constraint.

## References

- [Microsoft C# language version configuration](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version)
- [Microsoft Visual Basic strategy](https://learn.microsoft.com/en-us/dotnet/visual-basic/getting-started/strategy)
- [Microsoft .NET Framework version and toolchain dependencies](https://learn.microsoft.com/en-us/dotnet/framework/install/versions-and-dependencies)
