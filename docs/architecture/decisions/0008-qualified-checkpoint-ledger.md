# ADR 0008: Qualified checkpoint ledger

- Status: Accepted
- Date: 2026-08-04

## Context

C3 2.0 is developed through six substantial alphas before owner-qualified public
betas. Keeping `master` on 1.x until stable 2.0 would hide the sequence of proven
engineering checkpoints, while treating every moving `dev` head as releasable
would make tags and evidence meaningless. The public repository also means a
pushed alpha branch or tag is visible even when no binary is published.

A validation record must name the exact frozen source commit. That commit cannot
contain its own SHA, so release evidence cannot be both self-referential and part
of the same commit without an external attestation.

## Decision

Use three permanent lines:

- `maintenance/1.x` owns bounded supported 1.x maintenance;
- `master` is the append-only ledger of qualified product checkpoints; and
- `dev` owns the next unqualified milestone.

Every alpha, beta, release-candidate, and stable tag is reachable from `master`.
No permanent branch or qualified tag is force-pushed or replaced. Applicable 1.x
changes flow forward; 2.x-only changes never flow backward.

Qualification uses two commits when evidence names a literal source SHA:

```text
C  frozen payload/source commit
   -> build, package, automated and required manual evidence
E  attestation commit naming C and its hashes
   -> may change only release/catalog.v1.json and C's validation record
```

`E` is fast-forwarded to `master` and receives the annotated milestone tag. The
promotion validator proves that `C` is an ancestor of `E`, that the `C..E` diff
is evidence-only, and that rebuilding `E` yields the recorded payload bytes.
Thus `master` and the tag identify a self-contained qualified checkpoint without
pretending the evidence commit changed the product payload.

Qualification, promotion, publication, and post-verification are recorded as
separate lifecycle dimensions. Alpha checkpoints are tagged but have no GitHub
release and no promoted update feed. Beta tags require owner manual qualification before a public prerelease.
Stable promotes unchanged qualified release-candidate payloads; any byte change
requires another release candidate.

## Consequences

- Milestone names follow evidence, not elapsed time or planned scope.
- Alpha source and tags are public; only distribution remains internal/unpublished.
- `dev` cannot advance to the next identity until the current checkpoint is
  qualified, promoted, and tagged.
- Release evidence becomes a mechanically validated architectural input.
- A missing licence, artifact, manual decision, compatibility oracle, or required
  runtime environment stops promotion without blocking unrelated evidence work.
