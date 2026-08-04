# Versioning and update channels

C3 maintains separate identities because a product release, an assembly contract,
a catalogue format, and an update audience do not evolve at the same rate.

## Version identities

| Identity | Canonical owner | Current 2.0 alpha policy |
| --- | --- | --- |
| Product version | `build/Version.props` | `2.0.0` |
| Release label | generated from product version and stage | `2.0.0-alpha.2` |
| Release channel | `build/Version.props` | `alpha` |
| Update publication metadata | channel `release.json` | `published: false`; Alpha 2 must not advertise availability |
| Assembly contract | `build/Version.props` | `2.0.0.0` for the 2.x contract line |
| File build identity | `build/Version.props` | numeric four-part version |
| Informational version | generated assembly metadata | SemVer release label, optionally plus a source revision |
| Legacy catalogue format | format specification and adapter | `1.1.0` until native v2 is implemented |
| Native catalogue format | versioned specification | unclaimed; `spec/catalogue/v2.0.0` is a draft design space |

A product major version does not imply a catalogue-format major version. The UI
may display both when the distinction matters.

## Channel contract

Channels are promotion targets, not branch names:

- **stable** receives only a stable release after exact assets and downloaded
  hashes pass the complete gate;
- **beta** receives public betas and release candidates after their public
  prerelease gates pass;
- **alpha** records generated development identity but is not promoted while
  alphas remain intentionally unpublished; and
- **legacy-1x** remains the maintenance feed for existing 1.x clients.

A release candidate therefore uses channel `beta` and publication policy
`public-prerelease`. Stable users never receive preview metadata automatically.
A publishable beta or stable `release.json` is promoted only in successful
post-operation commit `P`, after its exact packages and checksum manifest exist
and downloaded verification has passed. Removing a release does not silently
retarget its users.

The final RC/stable byte-identity strategy remains unresolved. C3 must accept a
strategy before the first release candidate; until then it does not promise that
stable reuses RC bytes or that a metadata-only rebuild is sufficient.

## Permanent branch contract

`build/branches.json` is the single machine-readable owner of permanent branch
identities. The current contract is:

| Branch | Role |
| --- | --- |
| `master` | Qualified current-generation checkpoint ledger; C3 2.x for this programme. |
| `dev/2.x` | Moving C3 2.x integration branch. |
| `legacy/1.x` | Qualified C3 1.x checkpoint ledger and reconstruction authority. |
| `dev/1.x` | Moving bounded C3 1.x maintenance branch. |

Normal 2.x topic branches target `dev/2.x`. A 1.x correction targets `dev/1.x`
first, is verified under the 1.x contract, and is then promoted to `legacy/1.x`
through its qualification gate. Applicable fixes are reproduced and
forward-implemented through the 2.x owner on `dev/2.x` with matching regression
evidence; divergent 1.x implementation history is not merged wholesale. A
2.x-only change never flows backward into either 1.x line. Reserved,
SHA-bound `attest/v*-candidate-<E>` and `attest/v*-post-<P>` refs temporarily make
exact 2.x attestation commits reachable to private gates while permanent refs
remain at their expected old objects. They are create-only transport, not version
lines, and are consumed by the leased atomic promotion transactions.

No permanent branch is force-pushed or deleted. `legacy/1.x` advances only by
fast-forward from a qualified `dev/1.x` checkpoint under the applicable 1.x release
contract. A 2.x checkpoint follows `C -> E(tag) -> P`:

- `C` freezes every payload input;
- `E`, the direct, single-parent child of `C`, changes exactly the release
  catalogue and matching validation record, records qualification `pass` and
  promotion `unpromoted`, and receives the immutable annotated tag; and
- `P`, the direct, single-parent child of tagged `E`, records the now-observable
  annotated tag-object identity, publication, feed, and post-verification facts.

`master` advances only to the verified full `E` SHA and then the verified full
`P` SHA, never to moving `dev/2.x`. `dev/2.x` begins the next identity only after `P` is
also on `master`. Git branches and tags do not replace release channels: alpha
checkpoints are visible but unpublished; beta/stable availability begins only
after matching immutable assets and successful channel promotion.

## Legacy root `VERSION`

Published 1.x binaries fetch the repository root `VERSION` and understand only a
three-line numeric format. Therefore it is a compatibility feed, not a generated
projection of the current source tree. It remains synchronized with
`release/feeds/legacy-1x/VERSION` while those clients exist and is promoted
deliberately only after matching 1.x assets are public.

Build synchronization must never overwrite the root legacy feed. Verification
checks build identity and published-feed identity independently. C3 2.x uses
channel `release.json` documents with explicit publication state and complete
release identity. There are no 2.x `VERSION` files, and a moving three-line
`/dev/2.x/` endpoint cannot satisfy the 2.x updater contract by branch identity
alone. Remediating current
Alpha 1 client behavior is a qualification gate, not a future aspiration.

## Release naming

Tags and package filenames use an unambiguous SemVer-compatible label:

```text
v2.0.0-alpha.1
C3-v2.0.0-alpha.1-win-x86-net40-portable.zip
C3-v2.0.0-alpha.1-win-x64-net48-portable.zip
SHA256SUMS.txt
```

Display text may use `2.0.0 Alpha 1`. Only the root and legacy 1.x compatibility
feeds retain the three-line numeric format because old code parses
`System.Version`.

## Promotion and immutability

1. Freeze source/payload commit `C`; generate every packaged projection before it.
2. Build, test, and reproduce both lanes from exact `C` under the milestone gate.
3. Complete the manual, OS, compatibility, and migration evidence required for
   that stage; deferred alpha evidence remains explicit.
4. Create direct, single-parent child `E` with the exact two-file evidence diff,
   naming full `C` and its artifact identities; record `pass / unpromoted`.
5. Rebuild exact `E`, prove identical payload bytes, and validate its full SHA.
6. Qualify exact `E` through its SHA-bound transport while `dev/2.x` remains at `C`,
   then atomically and with exact-old-object leases fast-forward `master` and
   `dev/2.x` to `E`, create the absent annotated tag, and consume the transport ref.
7. Create direct, single-parent child `P` after external operations:
   - Alpha changes exactly the two evidence files and records
     `tagged / unpublished / not-applicable / feed false`.
   - Successful beta/RC changes those two files plus
     `release/feeds/beta/release.json` and records
     `published / passed / feed true`.
   - Successful stable changes those two files plus
     `release/feeds/stable/release.json` under the accepted stable strategy.
   - Public post-verification failure changes only the two evidence files,
     records `published / failed / feed false`, leaves the feed unchanged, and
     may be superseded by an immediate successor.
8. Validate full `P` through its SHA-bound transport, then atomically and with
   exact-old-object leases fast-forward `P` to both `master` and `dev/2.x`, consume
   the transport ref, and only then commit the next milestone identity.

Historical tags, validation records, package names, hashes, failed publication
facts, and channel history are never relabelled. Supersession adds history; it
does not rewrite it.
