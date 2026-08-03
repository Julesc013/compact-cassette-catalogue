# C3 update feeds

Update feeds are publication APIs, not mirrors of whichever version is in the
working tree.

- `legacy-1x/VERSION` is the three-line maintenance feed understood by published
  1.x binaries. The root `VERSION` is a compatibility projection of this file.
- `alpha/` currently contains generated 2.x development metadata only. While its
  manifest says `published: false`, it is not a promoted availability feed and
  updater decisions must not advertise it.
- `beta/` and `stable/` are created only when a release is promoted to those
  channels; an absent feed is safer than invented availability.

`build/sync-version.ps1` generates the current development-channel `VERSION` and
`release.json` from `build/Version.props`. It never edits the root or legacy feed.
The generated JSON records build identity and has `published: false`; publishing
assets and promoting availability is a separate, evidence-gated operation.
Three-line `VERSION` exists only for legacy compatibility and must not be treated
as sufficient publication state by new prerelease clients.

Legacy `VERSION` files have exactly three lines:

```text
numeric product version
display stage
DD/MM/YYYY
```

Do not change a publishable beta/stable feed until matching immutable assets
exist and their downloaded hashes have passed the release gate. Qualified alpha
checkpoints do not promote a feed or create a GitHub release.
