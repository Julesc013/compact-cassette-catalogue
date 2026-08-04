# ADR 0008: Qualified checkpoint ledger

- Status: Accepted
- Date: 2026-08-04

## Context

C3 2.0 is developed through six substantial alphas before owner-qualified public
betas. Keeping `master` on 1.x until stable 2.0 would hide the sequence of proven
engineering checkpoints, while treating every moving `dev` head as releasable
would make tags and evidence meaningless. The public repository also means a
pushed alpha branch or tag is visible even when no binary is published.

A validation record cannot name the SHA of the commit that contains it. Tag,
GitHub release, downloaded-asset, and feed-promotion facts likewise cannot be
truthfully recorded before those operations exist. The ledger therefore needs a
small explicit transaction rather than self-referential metadata.

## Decision

Use four permanent lines in two independently promoted trains:

- `maintenance/1.x` owns active, unqualified bounded 1.x maintenance;
- `legacy/1.x` is the append-only ledger of qualified 1.x checkpoints;
- `master` is the append-only ledger of verified 2.x product checkpoints; and
- `dev` owns the next unqualified 2.x milestone.

Every 2.x alpha, beta, release-candidate, and stable tag is reachable from
`master`; every newly qualified 1.x maintenance tag is reachable from
`legacy/1.x`. No permanent branch or qualified tag is force-pushed or replaced.
Qualified 1.x changes advance by fast-forward from `maintenance/1.x` to
`legacy/1.x`, then applicable fixes flow forward to `dev`; 2.x-only changes never
flow backward.

Each checkpoint is the linear transaction:

```text
C  frozen source and payload inputs
|
E  direct, single-parent child of C; exact catalogue + validation diff
|  qualification pass, promotion unpromoted; annotated tag points here
|
P  direct, single-parent child of tagged E; exact post-operation evidence diff
```

The promotion validator rebuilds exact `E`, proves that it preserves the payload
bytes qualified from `C`, and accepts only its full SHA through a create-only,
SHA-bound candidate transport while `dev` remains at `C`. Exact-old-object leases
then atomically fast-forward `master` and `dev` to `E`, create the absent
annotated tag, and consume the transport. The tagged snapshot remains factually
`unpromoted` because it cannot predict its own tag.

After the tag and any stage-specific external operations, direct,
single-parent child `P` records the exact annotated tag-object identity and
observed stage facts:

- intentionally unpublished alpha `P` changes exactly the catalogue and matching
  validation record; it records `tagged`, `unpublished`, post-verification
  `not-applicable`, and feed promotion `false`;
- successful public beta or release-candidate `P` additionally changes exactly
  `release/feeds/beta/release.json` and records `published`, post-verification
  `passed`, and feed promotion `true`;
- successful public stable `P` additionally changes exactly
  `release/feeds/stable/release.json` under the separately accepted stable
  identity strategy; and
- public post-verification-failure `P` changes only the two evidence files,
  records `published / failed / feed false`, preserves the failed tag and assets,
  and may be superseded by an immediate successor.

`P` is validated by full SHA through a create-only, SHA-bound
`attest/v*-post-<P>` transport ref while both permanent branches remain at `E`.
Exact-old-object leases then atomically fast-forward both to `P` and consume the
transport. It is not a fourth product line, and its moving name is never the
promotion input.
`dev` cannot begin the next identity until both permanent 2.x refs name verified
`P`.

Qualification, promotion, publication policy/state, post-verification, and
supersession remain separate facts. Release candidates use the beta channel and
public-prerelease policy. C3 2.x publication uses channel `release.json`; no 2.x
`VERSION` files are introduced.

The RC/stable byte-identity strategy is intentionally outside this decision and
remains unresolved. It must be accepted before the first release candidate. No
claim that stable reuses RC bytes or that a metadata-only rebuild is adequate is
valid until then.

## Consequences

- Milestone names follow evidence, not elapsed time or planned scope.
- Alpha source and tags are public; only distribution remains intentionally
  unpublished.
- `master` advances through two mechanically verified exact SHAs, `E` then `P`.
- Exact `P` receives durable pre-push workflow evidence without advancing a
  permanent branch prematurely; its temporary attestation ref is disposable.
- Release evidence and allowed diffs become mechanically validated architectural
  inputs.
- Replacing an annotated tag object is detected even when it still peels to the
  same commit; repository immutable-tag rules remain the race-prevention boundary.
- Published beta/stable feed metadata has one owner: successful public `P`.
- A missing licence, artifact, manual decision, compatibility oracle, required
  runtime environment, or stable identity decision stops promotion without
  blocking unrelated evidence work.
