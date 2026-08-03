# ADR 0002: Maintain two compatibility build lanes

- Status: Accepted
- Date: 2026-08-04

## Context

C3 must retain a 32-bit Windows XP-compatible build while also providing a
64-bit build that can opt into newer .NET Framework runtime behavior. Changing
the target framework conditionally inside one old-style project makes designer
and generated-file behavior difficult to reason about.

## Decision

Publish one C3 product through two explicit build lanes:

| Lane | Target | Platform | Compatibility claim |
| --- | --- | --- | --- |
| `win-x86-net40` | .NET Framework 4.0 | x86 | Windows XP SP3+ |
| `win-x64-net48` | .NET Framework 4.8 | x64 | Windows 7 SP1+ |

Both projects compile the same physical source and preserve the same user-facing
product identity, settings identity, and catalogue format. Differences are
limited to the list in `repository-layout.md` and are checked mechanically.

The portable ZIP is the authoritative distribution until an external setup
system has a stable, exercised product-binding contract. Setup metadata is data,
not a C3 production assembly.

## Consequences

- A fix normally changes one source file and applies to both lanes.
- Runtime-specific code is pushed to small edge adapters.
- The x64 lane can enable .NET 4.8 DPI/runtime improvements where the operating
  system supports them without overstating Windows 7 DPI behavior.
- Compatibility claims require lane-specific build and runtime evidence.
- The build must fail if the two project files drift in their shared source list.

