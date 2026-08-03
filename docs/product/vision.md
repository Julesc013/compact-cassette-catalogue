# C3 product vision

C3 is an offline-first, archival-grade catalogue for compact cassettes,
recordings, cassette models, brands, and tape decks. It should feel like a
carefully made native Windows product while keeping the user's collection in an
open, inspectable, portable form.

The long-lived product is not a particular form, programming language, database,
or framework. It is the combination of:

- a documented catalogue model and interchange contract;
- deterministic migrations and compatibility fixtures;
- explicit commands, validation rules, and observable results;
- a dependable native desktop workflow; and
- reproducible releases with verifiable evidence.

Implementation technology may be replaced when evidence proves the replacement.
User data and user intent remain authoritative throughout that change.

## Product promise

C3 should let a collector understand and control the complete lifecycle of a
catalogue:

1. create or open it without a network connection;
2. find and edit records quickly without learning storage internals;
3. see validation and pending changes before committing them;
4. save transactionally without destroying the last known-good copy;
5. move the catalogue and application between supported computers;
6. migrate deliberately, with a report and a reversible path; and
7. recover useful diagnostic evidence when something goes wrong.

The default experience is calm, native, keyboard-friendly, and predictable.
Advanced operations disclose their consequences before changing data. C3 never
turns an ordinary open or save into an implicit format conversion.

## Non-negotiable invariants

In priority order:

1. Never corrupt, silently discard, or silently reinterpret user data.
2. Preserve the documented C3 1.x compatibility contract.
3. Keep catalogue semantics identical in both Windows build lanes.
4. Give each rule, state transition, and external mechanism one canonical owner.
5. Make failures diagnosable and recovery actions explicit.
6. Prefer open, deterministic formats and language-neutral specifications.
7. Optimize only after representative measurements identify a real constraint.

Backward compatibility is evidence, not a slogan. A compatibility statement is
valid only when the relevant fixture, old/new round trip, settings migration,
runtime, and release-channel checks have passed.

## Product architecture

C3 remains a modular monolith. It has one domain and one production behavior,
with explicit boundaries for presentation, application commands, catalogue
rules, persistence, operating-system integration, and release tooling.

The desired direction is:

```text
Native WinForms workspace
        |
Application commands, drafts, results, undo/redo
        |
Typed catalogue domain with stable opaque identities
        |
Ports for persistence, import/export, settings, diagnostics, and updates
        |
Versioned adapters and external integrations
```

This is not a mandate to create a project for every box. A project exists only
where a dependency boundary is worth enforcing. Feature folders keep related
types adjacent; generic `Helpers`, `Managers`, `Common`, and `Platform` dumping
grounds are prohibited.

## Compatibility lanes

C3 is one product built through two explicit lanes:

| Lane | Runtime | Purpose |
| --- | --- | --- |
| `win-x86-net40` | x86, .NET Framework 4.0 | Conservative Windows XP SP3 compatibility lane |
| `win-x64-net48` | x64, .NET Framework 4.8 | Modern 64-bit Windows lane, starting at Windows 7 SP1 |

Both lanes use the same domain behavior, catalogue adapters, features, and user
documentation. Runtime-specific behavior stays at narrow edges. Minimum-OS,
high-DPI, and accessibility claims require evidence on the exact release
candidate.

## Experience principles

- **Direct manipulation:** lists, inspectors, and editors use the same command
  model and reveal the selected object and pending operation.
- **Progressive disclosure:** common catalogue work is obvious; batch, import,
  migration, and recovery tools are available without dominating the workspace.
- **Visible state:** active catalogue, dirty state, compatibility mode, filters,
  validation, background work, and recovery state are never hidden.
- **Reversible work:** edits use drafts; destructive and bulk changes show a
  preview; practical operations support undo and redo.
- **Native restraint:** use system fonts, colors, controls, keyboard conventions,
  accessibility APIs, and DPI-aware layout before decorative custom rendering.
- **Honest portability:** portable distribution remains a supported first-class
  path; profile portability is claimed only when settings and diagnostics honor
  an explicit portable-profile mode.

## Extensibility principles

Start with declarative, versioned extension points: reference data packs,
import/export profiles, saved views, validation profiles, localization, and
report templates. Validate every external artifact before it reaches domain
state.

Executable extensions are a later capability. If introduced, they use a small
versioned protocol, explicit capabilities, time and resource limits, and process
isolation. C3 does not load arbitrary third-party assemblies into the catalogue
process.

## What 2.0 is not

C3 2.0 is not:

- a big-bang rewrite that discards proven behavior;
- a requirement to eliminate every line of Visual Basic before a useful release;
- a new catalogue format merely because the product major version changed;
- a cloud account, service, telemetry, or network dependency;
- an excuse to put catalogue logic in C, C++, setup code, or UI event handlers;
- a plugin marketplace; or
- a claim that Windows XP or Windows 7 themselves are supported by Microsoft.

## Definition of an excellent release

A C3 release is excellent when users can understand it, trust it with a copy of
their collection, reproduce its important workflows, and leave it without being
locked in. The release has no known release-blocking defect, every fixed defect
has lasting regression evidence, unsupported paths are explicit, artifacts are
reproducible and hash-verified, and a future implementation can validate itself
against the same public contracts.
