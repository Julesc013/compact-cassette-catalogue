# C3 release catalogue v1

`release/catalog.v1.json` is the canonical machine-readable index of milestone
identity, qualification, promotion, publication policy/state,
post-verification, supersession, and exact artifacts. Human validation records
provide detail and are checked as projections of this catalogue.

Lifecycle facts are independent. `supersededBy` does not erase whether a
candidate qualified or a public post-download check failed. Promotion and
publication begin as factual `unpromoted`/`unpublished` values and change only
after the corresponding operations exist.

Catalogue order is semantic, not lexical. Product versions never decrease. For
one product version the permitted direction is Alpha → Beta → Release Candidate
→ Release; sequence numbers within one prerelease family strictly increase.
Stages may be skipped when an accepted plan permits it, but they cannot move
backward, and a stable version is terminal until the numeric product version
increases. Very large numeric components are compared without fixed-width
integer conversion.

Release labels and Git tags are deliberately separate identities. Package,
binary, feed, and validation labels retain the readable SemVer form such as
`2.0.0-alpha.5`; Git tags from Alpha 5 onward use the compact form `2.0.0a5`.
The catalogue schema also accepts the immutable historical Alpha 1–4 tags in
their original `v2.0.0-alpha.N` form. A recorded tag is never derived again for
a historical row and is never renamed or moved.

## Transaction

```text
C  frozen source and payload inputs
|
E  direct, single-parent child; exact catalogue + matching validation diff
|  pass / unpromoted; rebuilds byte-identically; annotated tag targets E
|
P  direct, single-parent child of tagged E; exact stage/outcome-specific evidence diff
```

The full `E` SHA, never moving `dev/2.x`, is validated and atomically
fast-forwarded to `master` with its annotated tag. The catalogue at tagged `E` remains
`unpromoted` with a null `promotion.tagObject`: it cannot truthfully predict an
external tag operation. `P` records the lowercase 40-character annotated-tag
object ID. Validation binds both that immutable object and its peeled commit on
`origin`, so replacing only a tag's annotation is detected even when its target
commit does not change.

`P` records only facts that are already observable:

| Outcome | Exact `E..P` paths | Required final facts |
| --- | --- | --- |
| Intentionally unpublished alpha | catalogue + matching validation record | `tagged / unpublished / not-applicable / feed false` |
| Successful public beta or RC | evidence pair + `release/feeds/beta/release.json` | `published / passed / feed true` |
| Successful public stable | evidence pair + `release/feeds/stable/release.json` | `published / passed / feed true` |
| Failed public post-verification | catalogue + matching validation record | `published / failed / feed false`; channel feed unchanged |

`P` must have only tagged `E` as its parent. Its full SHA is validated and then
atomically fast-forwarded to both `master` and `dev/2.x`; only afterward may
`dev/2.x` begin the next release identity. A failed public checkpoint and its immutable
tag/assets may be linked as superseded by an immediate successor.

Release candidates use channel `beta` and policy `public-prerelease`. Stable is
a direct metadata-only source transition from the accepted RC followed by a
complete rebuild and requalification of its new bytes; RC and stable artifacts
are not claimed to be byte-identical. C3 2.x uses channel `release.json` and never a `VERSION` projection.
Ordinary version synchronization cannot own published beta/stable metadata;
successful public `P` is its sole repository owner.

Package records are absent while no exact source has been attested. A passing
qualification requires one exact package per portable lane plus the checksum
manifest. Filenames derive from `build/Version.props` and `build/lanes.json`.
The v1 catalogue transport is strict UTF-8 and limited to 4 MiB before parsing;
that ceiling leaves ample long-lived milestone capacity while bounding hosted
and compatibility-gate memory use.
