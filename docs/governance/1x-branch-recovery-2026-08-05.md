# C3 1.x branch recovery record

Date: 5 August 2026

## Decision

The original `legacy/1.x` and `dev/1.x` refs were created from an unqualified,
broad 1.2.1-era refactor. Both refs pointed at
`f27c1d0c6798ea68b81ac0b0889ef770ad19d2d9`. That tree is not the production
base for C3 1.3.0.

The recovery authority is `v1.2.0b1`, commit
`2413e9139a098f3321385f2f946e743012a447f5`. Its recorded manually tested source
is `509c9ec29679e30dcdcb1f57d8874b850cee310c`; the later commits through the tag
change release documentation rather than runtime code.

The corrected lineage therefore starts directly at `v1.2.0b1`. Documentation
commits may follow that tag without changing the baseline runtime tree. Runtime
work may then advance only on `dev/1.x` through bounded, independently tested
1.3.0 patches.

## Preservation and correction

Before either permanent ref moves, the superseded tip is preserved exactly by
the annotated archival tag:

```text
archive/1x-refactor-attempt-2026-08-03
  -> f27c1d0c6798ea68b81ac0b0889ef770ad19d2d9
```

The one-time correction is performed with exact-old-SHA leases:

```text
legacy/1.x (old f27c1d0...) -> reconstructed v1.2.0b1 lineage
dev/1.x    (old f27c1d0...) -> reconstructed v1.2.0b1 lineage
```

Remote publication must be atomic if the hosting service supports it. It must
include explicit leases for both old permanent refs. The archived tag must be
published no later than the corrected refs. No commit, tag, or refactor evidence
is deleted.

This is an exceptional bootstrap repair to refs that did not yet obey their
documented roles. It does not authorize routine force-pushes. After recovery,
permanent refs return to normal fast-forward-only movement.

## Permanent branch contract

| Branch | Permanent role |
| --- | --- |
| `master` | Qualified 2.x checkpoint ledger |
| `dev/2.x` | Moving 2.x integration |
| `legacy/1.x` | Qualified 1.x checkpoint ledger and reconstruction authority |
| `dev/1.x` | Moving, bounded 1.x integration |

Qualified 1.x work is implemented on `dev/1.x`, proven under the 1.x
qualification contract, and promoted by fast-forward to `legacy/1.x`. After
C3 1.3.0 publication both 1.x refs normally remain at the same checkpoint.
Only a critical data-loss, security, startup, or platform regression may justify
a 1.3.1 release. All other product evolution belongs on `dev/2.x`.

## Protected public state

During development and qualification, the repository-root `VERSION` feed and
any legacy three-line release feed must continue advertising the available
`1.2.0 / Beta 1` release. They move only after the final 1.3.0 packages have been
qualified, tagged, published, downloaded again, and reverified.

## Audit checks

The recovery is complete only when all of the following are true:

- the archival tag resolves to exactly `f27c1d0...`;
- the corrected 1.x tips descend from exactly `v1.2.0b1`;
- the production tree at the shared reconstructed checkpoint is identical to
  the production tree at `v1.2.0b1`;
- `legacy/1.x` contains the governance and recovery plan but no unqualified
  runtime patch;
- `dev/1.x` contains only the documented non-runtime setup until Gate 1 passes;
- `master` and `dev/2.x` are not moved by this correction; and
- the original 1.2.1/refactor history remains reachable from the archive tag.
