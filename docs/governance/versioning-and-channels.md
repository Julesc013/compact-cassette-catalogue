# Versioning and update channels

C3 maintains separate identities because a product release, an assembly contract,
a catalogue format, and an update audience do not evolve at the same rate.

## Version identities

| Identity | Canonical owner | Current 2.0 alpha policy |
| --- | --- | --- |
| Product version | `build/Version.props` | `2.0.0` |
| Release label | generated from product version and stage | `2.0.0-alpha.1` |
| Release channel | `build/Version.props` | `alpha` |
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
- **alpha** receives explicit development previews and is opt-in;
- **legacy-1x** remains the maintenance feed for existing 1.x clients.

Stable users never receive preview metadata automatically. A channel document is
promoted only after its exact packages and checksum manifest exist. Removing a
release does not retarget its users silently to another channel.

## Legacy root `VERSION`

Published 1.x binaries fetch the repository root `VERSION` and understand only a
three-line numeric format. Therefore it is a compatibility feed, not a generated
projection of the current source tree. It remains synchronized with the
`legacy-1x` feed while those clients exist and is promoted deliberately only
after matching 1.x assets are public.

Current 2.x binaries read their configured channel feed. Build synchronization
must never overwrite the root legacy feed. Verification checks build identity
and published-feed identity independently.

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

1. Freeze one source commit and generate all version projections.
2. Build and test both lanes from that commit.
3. Create the packages once, verify exact contents, and record hashes.
4. Complete required manual, OS, compatibility, and migration evidence.
5. Publish the immutable assets and checksum manifest.
6. Download and verify them independently.
7. Promote the matching channel document last.

Historical tags, validation records, package names, and hashes are never
relabelled. An unpublished candidate may be marked superseded, but its identity
and evidence remain intact.
