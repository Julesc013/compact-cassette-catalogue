# Versioning and update channels

C3 maintains separate identities because a product release, an assembly contract,
a catalogue format, and an update audience do not evolve at the same rate.

## Version identities

| Identity | Canonical owner | Current 2.0 alpha policy |
| --- | --- | --- |
| Product version | `build/Version.props` | `2.0.0` |
| Release label | generated from product version and stage | `2.0.0-alpha.1` |
| Release channel | `build/Version.props` | `alpha` |
| Update publication metadata | channel `release.json` | `published: false`; Alpha 1 must not advertise availability |
| Assembly contract | `build/Version.props` | `2.0.0.0` for the 2.x contract line |
| File build identity | `build/Version.props` | numeric four-part version |
| Informational version | generated assembly metadata | SemVer release label, optionally plus a source revision |
| Legacy catalogue format | format specification and adapter | `1.1.0` until native v2 is implemented |
| Native catalogue format | versioned specification | unclaimed; `spec/catalogue/v2.0.0` is a draft design space |

A product major version does not imply a catalogue-format major version. The UI
may display both when the distinction matters.

## Channel contract

Channels are promotion targets, not branch names:

- **stable** receives only a stable release after assets and downloaded hashes
  pass the complete gate;
- **beta** receives public feature-complete previews after beta gates pass;
- **alpha** records generated development identity but is not promoted while
  alphas remain intentionally unpublished;
- **legacy-1x** remains the maintenance feed for existing 1.x clients.

Stable users never receive preview metadata automatically. A channel document is
promoted only after its exact packages and checksum manifest exist. Removing a
release does not retarget its users silently to another channel.

## Permanent branch contract

`maintenance/1.x` owns bounded supported 1.x maintenance. `master` is the
append-only promotion ledger for every qualified alpha, beta, release candidate,
and stable checkpoint. `dev` owns active, unqualified development toward exactly
one next checkpoint.

Normal feature branches target `dev`. A 1.x correction targets
`maintenance/1.x` first, is verified under the 1.x contract, and is then
forward-merged or deliberately ported to `dev` with the same regression evidence.
A 2.x-only change never flows backward into the maintenance line.

No permanent branch is force-pushed. After a milestone is frozen and qualified,
its evidence attestation is fast-forwarded to `master` and tagged immutably.
`dev` begins the next identity only after that tag exists. Git branches and tags
do not replace release channels: alpha checkpoints are visible but unpublished;
beta/stable availability begins only after the matching immutable assets and
channel promotion pass their stage-specific gates.

## Legacy root `VERSION`

Published 1.x binaries fetch the repository root `VERSION` and understand only a
three-line numeric format. Therefore it is a compatibility feed, not a generated
projection of the current source tree. It remains synchronized with the
`legacy-1x` feed while those clients exist and is promoted deliberately only
after matching 1.x assets are public.

Build synchronization must never overwrite the root legacy feed. Verification
checks build identity and published-feed identity independently. The 2.x updater
contract must read explicit publication state and compare the complete release
identity; a moving three-line `/dev/` VERSION endpoint cannot satisfy that rule.
Remediating the current Alpha 1 client behavior is a qualification gate, not a
future documentation aspiration.

## Release naming

Tags and package filenames use an unambiguous SemVer-compatible label:

```text
v2.0.0-alpha.1
C3-v2.0.0-alpha.1-win-x86-net40-portable.zip
C3-v2.0.0-alpha.1-win-x64-net48-portable.zip
SHA256SUMS.txt
```

Display text may use `2.0.0 Alpha 1`. Three-line compatibility feeds keep a
numeric first line because legacy code parses `System.Version`.

## Promotion and immutability

1. Freeze source/payload commit `C`; generate every packaged projection before it.
2. Build, test, and reproduce both lanes from `C` under the milestone gate.
3. Complete the manual, OS, compatibility, and migration evidence required for
   that stage; deferred alpha evidence remains explicit.
4. Create evidence-only commit `E`, naming `C` and the exact package hashes.
5. Rebuild `E` and prove that its evidence-only diff leaves payload bytes intact.
6. Fast-forward `E` to `master` and create the immutable annotated tag there.
7. For unpublished alphas, stop. For beta/RC/stable, publish exact tested assets,
   download and verify them, then promote the matching channel document last.
8. Return to `dev` and commit the next milestone identity before implementation.

Historical tags, validation records, package names, and hashes are never
relabelled. An unpublished candidate may be marked superseded, but its identity
and evidence remain intact.
