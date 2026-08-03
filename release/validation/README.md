# Release validation records

Each file records evidence for one exact candidate identity and source commit.
Records are never renamed to make an old candidate look like a newer release.
`release/catalog.v1.json` is the canonical machine-readable lifecycle and
artifact index; Markdown is the detailed human-readable projection. The
validator keeps them aligned.

## Independent lifecycle facts

Do not overload one status with unrelated facts. Every active 2.x record declares
five independently validated dimensions:

| Dimension | Allowed values |
| --- | --- |
| Qualification | `planned`, `active`, `blocked`, `pass`, `fail` |
| Promotion | `unpromoted`, `tagged` |
| Publication policy | `intentionally-unpublished`, `public-prerelease`, `public-stable` |
| Publication state | `unpublished`, `published` |
| Post-verification | `not-applicable`, `pending`, `passed`, `failed` |

`supersededBy` is a separate nullable relationship and never erases whether
qualification or public post-verification passed or failed. For example, a
completed alpha is `pass / tagged / intentionally-unpublished / unpublished /
not-applicable`. A beta can be qualified and tagged while its tagged `E` snapshot
still says `unpromoted / unpublished`; only later `P` may record actual
publication and post-download results.

## C, E, and P transaction

The frozen source/payload commit `C` cannot contain its own SHA. After qualifying
`C`, create `E` as its direct, single-parent child. `C..E` must change exactly:

- `release/catalog.v1.json`; and
- `release/validation/<release-label>.md`.

`E` records qualification `pass`, promotion `unpromoted`, full `C`, and exact
artifact identities. Rebuilding exact `E` must reproduce the qualified bytes.
After that check, expose exact `E` through its create-only SHA-bound candidate
transport while `dev` remains at `C`. Atomically and with exact-old-object leases
advance both permanent refs to `E`, create the absent annotated tag, and consume
the transport. The tag snapshot remains `unpromoted` because a commit cannot
attest to its own future external operation.

After tag/publication operations, create `P` as the direct, single-parent child
of tagged `E`. `P` records the full annotated tag-object SHA, which is checked
against both the local object and origin's raw/peeled tag refs:

- Alpha `P` changes exactly the same two evidence files and records
  `tagged / unpublished / not-applicable / feed false`.
- Successful public beta/RC `P` changes exactly those two files plus
  `release/feeds/beta/release.json`; successful stable `P` uses the matching
  stable path. It records `published / passed / feed true`.
- Public post-verification-failure `P` changes exactly the two evidence files,
  records `published / failed / feed false`, leaves the feed unchanged, and may
  be superseded by an immediate successor.

Validate the full `P` SHA from its create-only `attest/v*-post-<P>` ref while
both permanent refs remain at `E`, then use exact-old-object leases to atomically
fast-forward exact `P` to both `master` and `dev` and consume the temporary ref.
Only then may `dev` begin the
next identity. Neither promotion step accepts a moving branch name. Release candidates use the
beta/public-prerelease policy. C3 2.x has no
three-line `VERSION` projection; beta/stable `release.json` is changed only by a
successful public `P`.

| Record | Classification |
| --- | --- |
| `1.2.0-beta.1.md` | Historical 1.x candidate/release evidence |
| `1.2.1-beta.1.md` | Superseded unpublished local candidate evidence |
| `2.0.0-alpha.1.md` | Blocked, intentionally unpublished C3 2.0 candidate evidence |

A superseded record keeps its original hashes and limitations. A new identity
requires a clean build, new packages, new hashes, and a new validation record.
