# C3 update feeds

Update feeds are publication APIs, not mirrors of whichever version is in the
working tree.

- `legacy-1x/VERSION` is the three-line maintenance feed understood by published
  1.x binaries. Root `VERSION` is its compatibility projection.
- `alpha/release.json` is generated 2.x development metadata. While it says
  `published: false`, it is not an availability feed and updater decisions must
  not advertise it. Qualified alphas never promote it.
- `beta/release.json` is the publication document for public betas and release
  candidates. Release candidates use channel `beta` and policy
  `public-prerelease`.
- `stable/release.json` is the publication document for public stable releases.

There are no 2.x `VERSION` files. Three-line `VERSION` exists only at the
repository root and in `legacy-1x/` for compatibility with existing 1.x clients:

```text
numeric product version
display stage
DD/MM/YYYY
```

`build/sync-version.ps1` generates an unpublished manifest from
`build/Version.props`. Alpha identity is written to
`alpha/release.json`; Beta, release-candidate, and stable identity is staged at
`release/candidates/<release-label>/release.json`. Synchronization never edits
root or legacy `VERSION`, and never creates or mutates a promoted beta/stable
feed.

The closed, bounded document contract and publication transaction are specified
in [`spec/update-feed/v1/README.md`](../../spec/update-feed/v1/README.md).

A successful public post-operation commit `P` is the sole repository owner of a
published-channel change. Its exact diff contains the release catalogue,
matching validation record, and exactly one matching beta or stable
`release.json`; it records `published / passed / feed true`. The channel document
therefore enters repository history atomically with its evidence.

If public post-download verification fails, failure `P` changes only the two
evidence files, records `published / failed / feed false`, and leaves the prior
feed byte-for-byte unchanged. An absent feed is safer than invented availability.
