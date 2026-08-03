# ADR 0003: Establish C3 2.0 as an evolutionary product boundary

- Status: Accepted
- Date: 2026-08-04

## Context

The unreleased work previously labelled 1.2.1 changed repository ownership,
build lanes, persistence safety, domain access, testing, diagnostics, packaging,
and contribution rules. The intended next programme also introduces stable
identity, explicit migration, command-based editing, undo/redo, a redesigned
workspace, and independently versioned integrations.

Calling that body of work a 1.2 maintenance release understates its product and
engineering boundary. Starting again with an uncontrolled rewrite would discard
the compatibility evidence and seams already created.

## Decision

Reclassify the unpublished overhaul as C3 `2.0.0-alpha.1`. Continue from the
current modular-monolith codebase through small, behavior-preserving and
evidence-gated slices.

Product version and catalogue-format version remain independent. The first 2.0
alphas continue reading and writing the legacy 1.1.0 catalogue profile. A native
v2 profile is introduced only after its typed model, specification, migrations,
fixtures, reader, writer, and recovery behavior pass their gates.

Published 1.x history remains immutable. The root three-line `VERSION` remains a
legacy update feed until old clients are retired; 2.x development uses an
independent preview feed.

## Consequences

- The major version honestly communicates broad internal and experience change.
- Existing catalogue files do not become incompatible merely because the product
  version changes.
- Settings, updater, side-by-side installation, and export are compatibility
  surfaces alongside catalogue XML.
- The 1.2.1 candidate evidence is retained as superseded, never relabelled as 2.0
  evidence.
- Each replacement slice keeps the current implementation as an oracle until
  parity is demonstrated.
- C3 2.0 may ship with a proven VB presentation slice if removing it would reduce
  compatibility or release safety; implementation-language purity is not a user
  requirement.

## Relationship to earlier decisions

ADR 0001 remains accepted. Its rejection of a complete rewrite is consistent
with this decision: 2.0 is an evolutionary replacement programme inside a
modular monolith, not a restart. ADR 0002 remains the build-lane contract.

## Rejected alternatives

- Ship the overhaul as 1.2.x: obscures the size of the product boundary and
  compresses migration and preview work into a maintenance version.
- Change the catalogue format to 2.0 immediately: couples unrelated versions and
  creates compatibility risk before the native model is proven.
- Freeze the current code and rewrite elsewhere: duplicates behavior, loses
  incremental gates, and postpones useful safety improvements.
- Require a language conversion in a single change: combines translation with
  redesign and makes regressions difficult to localize.
