# AIDE integration boundary

Status: **Planned, optional development integration**

[AIDE](https://github.com/Julesc013/aide) is being developed as a repository-native
agentic engineering control plane built around work units, evidence, events,
policy, and review. Its current repository describes a protocol-first,
pre-runtime state. C3 therefore integrates only capabilities that exist in a
pinned, audited AIDE revision; roadmap descriptions are not callable contracts.

## Ownership boundary

C3 remains authoritative for:

- product and format specifications;
- source, fixtures, build lanes, tests, and release scripts;
- compatibility and release gates;
- accepted changes and Git history; and
- user catalogues and diagnostics.

AIDE may coordinate, invoke, index, and explain that evidence. It is not a C3
runtime dependency, catalogue mutator, hidden source of truth, automatic release
approver, or substitute for code review.

## Adoption stages

### Stage 0 — observe and report

- Pin an AIDE revision and record its actual supported protocol/capabilities.
- Map C3 verification commands to read-only work/evidence descriptions.
- Store only repository-safe metadata; keep machine-local paths, credentials,
  private catalogues, and diagnostic contents out of committed state.
- Compare AIDE-reported results with native script exit codes and artifacts.

### Stage 1 — compatibility laboratory assistance

- Propose synthetic fixtures and minimizations for review.
- Link failures to the owning compatibility-matrix row and regression scenario.
- Produce evidence packets that reference immutable commits and hashes.
- Detect stale or missing evidence without changing release status itself.

### Stage 2 — reviewed patch assistance

- Generate bounded patch proposals against an explicit work unit.
- Require normal diff review, repository gates, and maintainer acceptance.
- Never combine language translation, behavior redesign, and unrelated cleanup in
  one proposed patch.
- Treat generated output and designer changes under their existing owners.

### Stage 3 — release orchestration candidate

Only after AIDE implements stable scheduling, patch transactions, evidence
verification, and policy conformance may it coordinate candidate gates. C3 scripts
still perform the checks, and maintainers still promote update feeds and releases.

## Repository state

Do not commit a speculative `.aide` manifest merely to imply integration. Add it
only after choosing a pinned schema and proving a complete Stage 0 round trip.
Durable committed state must be minimal and reviewable; volatile logs, caches,
credentials, and private evidence remain machine-local and ignored.

## Acceptance criteria

An AIDE stage is accepted when it is optional, removable without changing product
behavior, produces the same result as direct C3 commands, records provenance and
hashes, redacts private data, fails closed, and has a documented manual fallback.
