# Release validation records

Each file records evidence for one exact candidate identity and source commit.
Records are never renamed to make an old candidate look like a newer release.
`release/catalog.v1.json` is the machine-readable lifecycle and artifact index;
Markdown remains the detailed human evidence. The validator keeps them aligned.

## Independent lifecycle dimensions

Do not overload one status with unrelated facts. Every active 2.x record declares
four independently validated dimensions:

| Dimension | Allowed values |
| --- | --- |
| Qualification | `planned`, `active`, `blocked`, `pass`, `fail`, `superseded` |
| Promotion | `unpromoted`, `master`, `tagged` |
| Publication | `intentionally-unpublished`, `prerelease`, `stable` |
| Post-verification | `not-applicable`, `pending`, `passed`, `failed` |

For example, a completed alpha is `pass / tagged /
intentionally-unpublished / not-applicable`. A public beta cannot become
`prerelease` before owner qualification of the exact packages, and remains
`pending` until downloaded assets are rehashed and launched. Current development
may be `active` or `blocked` while remaining intentionally unpublished.

## Evidence-only attestation

The frozen source/payload commit cannot contain its own SHA. After qualifying
that commit, one attestation commit may update only the matching validation file
and `release/catalog.v1.json`. The milestone tag points to that attestation
commit; automated validation proves the frozen source is its ancestor and that
the evidence-only diff does not alter packaged inputs.

| Record | Classification |
| --- | --- |
| `1.2.0-beta.1.md` | Historical 1.x candidate/release evidence |
| `1.2.1-beta.1.md` | Superseded unpublished local candidate evidence |
| `2.0.0-alpha.1.md` | Blocked, intentionally unpublished C3 2.0 candidate evidence |

A superseded record keeps its original hashes and limitations. A new identity
requires a clean build, new packages, new hashes, and a new validation record.
