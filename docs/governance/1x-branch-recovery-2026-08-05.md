# C3 1.x branch recovery record

Date: 5 August 2026

## Corrected decision

The original `legacy/1.x` and `dev/1.x` refs pointed at the unqualified broad
1.2.1-era refactor `f27c1d0c6798ea68b81ac0b0889ef770ad19d2d9`.
That tree is not the production base for C3 1.3.0.

Three distinct, immutable 1.2 identities control the reconstruction:

| Role | Commit/ref | Meaning |
| --- | --- | --- |
| Runtime source baseline | `509c9ec29679e30dcdcb1f57d8874b850cee310c` | Exact production source used for the validated x86/x64 packages |
| Qualified release checkpoint | `v1.2.0b1` / `2413e9139a098f3321385f2f946e743012a447f5` | Manual release qualification and GitHub prerelease tag |
| Development baseline | `58a5b7d21daf19e1b6112d44efb887c7d8ea9500` | Last safe pre-refactor tip, eleven documentation-only commits after the tag |

The misleading subject on `58a5b7d` does not describe a runtime change. The
aggregate difference from `v1.2.0b1` to `58a5b7d` contains only `CHANGELOG.md`,
`README.md`, and `TODO.md`; the production tree is unchanged. C3 1.3 therefore
develops from `58a5b7d`, while the release tag and runtime source remain the
behavioural and mechanical oracles.

## Preservation

The superseded histories are preserved by annotated archive tags:

```text
archive/1x-refactor-attempt-2026-08-03
  -> f27c1d0c6798ea68b81ac0b0889ef770ad19d2d9

archive/1.2-postrelease-tip
  -> 58a5b7d21daf19e1b6112d44efb887c7d8ea9500
```

No historical commit is discarded. The rough post-release roadmap remains in
ancestry and is deliberately replaced by the accepted gated 1.3 workboard in a
later reviewable commit.

## Ref correction sequence

The bootstrap layout is:

```text
legacy/1.x -> 2413e913... / v1.2.0b1
dev/1.x    -> 58a5b7d...
```

The first reconstructed checkpoint must retain `58a5b7d` as an ancestor,
replace the rough roadmap, tie its genome to `509c9ec`, prove zero production
difference across all three anchors, rebuild and test, and add evidence without
changing production. Only then may `legacy/1.x` fast-forward to that qualified
58a-derived checkpoint. Alpha identity work continues on `dev/1.x` afterward.

Remote ref corrections use exact-old-SHA leases and atomic publication where
supported. This exceptional bootstrap repair does not authorize routine
force-pushes; permanent branches return to fast-forward-only movement once the
corrected ancestry is installed.

## Permanent branch contract

| Branch | Permanent role |
| --- | --- |
| `master` | Qualified 2.x checkpoint ledger |
| `dev/2.x` | Moving 2.x integration |
| `legacy/1.x` | Qualified 1.x checkpoint ledger |
| `dev/1.x` | Moving, bounded 1.x integration |

Qualified 1.x work is implemented on `dev/1.x`, proven under the 1.x
qualification contract, and promoted by fast-forward to `legacy/1.x` only at
the milestone-defined boundary. After stable C3 1.3.0 both 1.x refs normally
remain at the same checkpoint.

## Protected public state

The repository-root `VERSION` feed continues advertising the available
`1.2.0 / Release / 14/05/2026` release throughout reconstruction and preview
qualification. It changes only after stable 1.3 packages are tagged, published,
downloaded again, and reverified.

## Audit checks

The reconstruction is correct only when:

- the runtime genome is pinned to exactly `509c9ec...`;
- `v1.2.0b1` resolves to exactly `2413e913...`;
- the development baseline and archive tag resolve to exactly `58a5b7d...`;
- all three production trees are identical before 1.3 identity projection;
- the reconstructed source descends from `58a5b7d...`;
- the refactor archive resolves to exactly `f27c1d0...`;
- no unqualified runtime patch reaches `legacy/1.x`;
- `master` and `dev/2.x` remain untouched; and
- every exceptional ref movement and superseded Alpha object remains auditable.
