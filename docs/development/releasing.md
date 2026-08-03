# Releasing C3

A C3 checkpoint is an evidence-backed promotion of one immutable product
payload. Branches, tags, GitHub publication, and update-channel promotion are
separate states and must never imply one another accidentally.

## Permanent branches

- `maintenance/1.x` owns bounded supported C3 1.x fixes.
- `master` is the append-only ledger of qualified C3 checkpoints.
- `dev` owns the next unqualified milestone.
- topic and optional `candidate/*` branches are short-lived.

Applicable 1.x changes flow forward to `dev`; 2.x-only work never flows backward.
No permanent branch or qualified tag is force-pushed, replaced, or deleted.

Every alpha, beta, release candidate, and stable tag must be reachable from
`master`. A tag proves qualification, not publication. Because this repository
is public, pushed alpha source and tags are visible; “internal alpha” means that
no binary release is published, announced, or promoted through an update feed.

## Stage gates

| Evidence | Alpha | Beta | RC/stable |
| --- | --- | --- | --- |
| Metadata, feed isolation, boundaries, tests, both builds, PE checks | Required | Required | Required |
| Two clean path-distinct package builds and checksums | Required | Required | Required |
| Known data-loss defects | Zero | Zero | Zero |
| Critical local workflow smoke | Required | Required | Required |
| Full owner workflow matrix | Selected changed/critical paths | Required | Required |
| XP SP3 / Windows 7 SP1 support matrix | May be explicitly deferred | Required for public support claims | Required |
| DPI/accessibility | Changed surfaces | Required | Required |
| GitHub release and post-download verification | No | Required prerelease | Required |
| Update-feed promotion | No | Beta feed last | Stable feed last |

Beta 1 is feature complete and compatibility complete. Stable is an unchanged
promotion of a qualified release-candidate payload. If any payload byte must
change, create another release candidate.

## Prepare and freeze source commit C

1. Confirm the accepted milestone scope and applicable evidence tier.
2. Set identity in `build/Version.props`, run `build/sync-version.ps1`, and commit
   every packaged projection, release note, changelog, and user-facing document.
   Synchronization never edits root `VERSION` or the legacy 1.x feed.
3. Finish milestone implementation. Do not start the next milestone's scope.
4. Resolve every known data-loss path and record any allowed deferred evidence.
5. Commit and freeze exact source/payload commit `C`.

No packaged input may change after `C`. In particular, do not edit `README.md`,
`RELEASE_NOTES.md`, build identity, source, resources, manifests, or packaging
logic while writing evidence.

## Qualify C

From a clean checkout of `C`:

```powershell
.\build\verify.ps1 -Rebuild
.\build\verify-reproducible-packages.ps1
```

The reproducibility gate exports `C` into two clean source roots with different
absolute paths, performs a full Release build/package pass in each through the
pinned Windows PowerShell host, compares names, sizes, and SHA-256 values, and
retains one already-proven set in `artifacts/packages`.

Run the milestone's critical workflows against those exact retained packages.
Record the source SHA, MSBuild patch, PowerShell/CLR, OS, package identities,
hashes, and all manual or deferred evidence. A failed or missing required check
leaves the candidate `blocked`; wording cannot manufacture a support claim.

## Create evidence attestation E

A commit cannot contain its own SHA. Once `C` passes, create one evidence-only
commit `E` that names `C` in:

- `release/catalog.v1.json`; and
- `release/validation/<release-label>.md`.

`C..E` may change only those two files. The catalogue contains exact package
names, lengths, and lowercase SHA-256 values. The validation record changes to
qualification `pass`; promotion, publication, and post-verification remain
separate factual dimensions.

Rebuild/package `E` and require the same hashes. This proves that the attestation
did not alter the payload. The promotion validator rejects any non-evidence file
in `C..E`.

## Promote and tag

Fast-forward only:

```powershell
git switch master
git merge --ff-only dev
git tag -a v2.0.0-alpha.N -m "C3 2.0.0 Alpha N qualified checkpoint"
git push origin master
git push origin v2.0.0-alpha.N
```

The annotated tag points to `E`, which contains the complete attestation and has
the same payload bytes as `C`. Tag validation requires:

- tag identity equals `Version.props`;
- the tag is annotated and reachable from `master`;
- the catalogue and validation record identify `C` and a passing status;
- `C` is an ancestor and `C..E` is evidence-only; and
- expected lanes, package names, lengths, checksums, and retained bytes agree.

For alpha, stop here: do not create a GitHub release or promote an alpha feed.
Return to `dev`, set the next alpha identity in a separate commit, and only then
resume feature work. If qualification cannot finish, remain at the last tagged
checkpoint and continue only work that does not cross the unmet exit gate.

## Publish beta and RC checkpoints

Beta/RC packages are published only after the owner manually qualifies the exact
candidate bytes. Then:

1. fast-forward and tag the qualified attestation as above;
2. create one GitHub prerelease for both lanes;
3. upload the proven ZIPs and `SHA256SUMS.txt` without renaming;
4. download every asset into a clean location;
5. rehash and launch the downloaded packages as required;
6. record the release URL and post-download evidence; and
7. promote matching beta metadata last.

An absent beta feed is safer than invented availability. Alpha, beta, and stable
clients must not infer availability from a moving development branch.

## Promote stable

Stable uses the exact qualified RC payload bytes. Publication metadata may refer
to the stable release, but product binaries and portable payload contents are not
rebuilt or relabelled. If the adopted distribution design requires stage-specific
bytes or filenames, the release candidate must already use the final stable
identity; otherwise create another RC rather than claiming byte identity.

After upload, download, rehash, launch, and record the stable assets. Promote the
stable feed last. The root/legacy feed changes only for a matching published 1.x
maintenance release.

## Rollback and immutability

Retain prior verified payloads and feed metadata. Never delete or relabel
historical tags, assets, hashes, or validation. A release rollback promotes a
previously verified payload; it does not automatically reverse a user's catalogue
migration. Catalogue rollback uses the preserved original/export and its own
documented workflow.

The repository currently has no explicit licence. Selecting one is an owner
decision and remains a qualification blocker before C3 advertises third-party
reuse or redistribution rights.
