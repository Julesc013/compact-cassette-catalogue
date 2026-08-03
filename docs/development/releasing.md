# Releasing C3

A release is an evidence-backed promotion of one immutable commit and payload,
not a rebuild performed while drafting release text.

## Branch and identity

- C3 1.2 maintenance releases originate on `master` and flow forward to `dev`.
- C3 2.0 previews originate from a frozen `dev` candidate.
- Stable 2.0 is promoted by merging the proven candidate into `master` and
  tagging that exact commit; permanent branches are not force-pushed.

Set current build identity in `build/Version.props`, run
`build/sync-version.ps1`, and commit generated projections. The synchronizer does
not edit root `VERSION` or the published `legacy-1x` feed.

## Prepare

1. Confirm accepted scope and target channel.
2. Update `CHANGELOG.md`, `RELEASE_NOTES.md`, and a new validation record without
   rewriting historical evidence.
3. Run the complete compatibility, settings, migration/export, UI, accessibility,
   security, performance, and OS gates applicable to the milestone.
4. Freeze the candidate source commit and record its full SHA/toolchain.

Any failed or unverified required check leaves publication blocked. Narrow a
claim explicitly when allowed; never convert missing evidence into a support
claim through wording.

## Build and package once

```powershell
.\build\verify.ps1 -Rebuild
.\build\package.ps1 -SkipBuild
```

Record resolved toolchain, exact binary identities, package filenames, sizes,
SHA-256 values, and a second deterministic package comparison. Do not modify or
restage a file after hashing. A setup build, if present, consumes these exact
payload bytes and has separate transaction evidence.

## Publish and promote

1. Create one GitHub release for both lanes from the frozen tag/commit.
2. Upload both ZIPs and `SHA256SUMS.txt` without renaming them.
3. Mark alpha/beta/RC releases as prereleases.
4. Download every asset into a clean location; verify hashes and launch both
   downloaded builds where applicable.
5. Record release URL and post-download evidence.
6. Promote matching update-channel metadata **last**.

Stable users never receive preview metadata. The root/legacy 1.x feed is changed
only for a matching published 1.x maintenance release. An absent beta/stable 2.x
feed is preferable to invented availability.

## Rollback and immutability

Retain the prior verified portable payload and its feed metadata. Do not delete or
relabel historical tags, assets, hashes, or validation. A release rollback changes
channel promotion to a previously verified product payload; it never automatically
reverses a user's catalogue migration. Catalogue rollback uses the preserved
original/export and its own documented workflow.
