# C3 update feeds

Update feeds are publication APIs, not mirrors of whichever version is in the
working tree.

- `legacy-1x/VERSION` is the three-line maintenance feed understood by published
  1.x binaries. The root `VERSION` is a compatibility projection of this file.
- `alpha/` is the current opt-in 2.x development feed.
- `beta/` and `stable/` are created only when a release is promoted to those
  channels; an absent feed is safer than invented availability.

`build/sync-version.ps1` generates the current development-channel `VERSION` and
`release.json` from `build/Version.props`. It never edits the root or legacy feed.
The generated JSON records build identity and has `published: false`; publishing
assets and promoting availability is a separate, evidence-gated operation.

Legacy `VERSION` files have exactly three lines:

```text
numeric product version
display stage
DD/MM/YYYY
```

Do not change a published feed until matching immutable assets exist and their
downloaded hashes have passed the release gate.
