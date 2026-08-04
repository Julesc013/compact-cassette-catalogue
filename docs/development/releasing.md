# Releasing C3

A C3 checkpoint is an evidence-backed promotion of one immutable product
payload. Branches, tags, GitHub publication, update-feed promotion, and
post-download verification are separate facts and must never imply one another.

Every checkpoint uses three exact commits:

```text
C  frozen source and payload inputs
E  qualification attestation and immutable tag target
P  post-operation attestation recording what actually happened
```

`E` is the direct, single-parent child of `C`; `P` is the direct,
single-parent child of tagged `E`. Promotion always names the full `E` or `P`
SHA. A moving `dev/2.x` reference is never a release input.

## Permanent branches

`build/branches.json` is the machine-readable owner of these identities; release
automation consumes it rather than repeating ref names.

- `dev/1.x` owns active, unqualified bounded C3 1.x fixes.
- `legacy/1.x` is the append-only ledger of qualified C3 1.x checkpoints.
- `master` is the append-only ledger of verified C3 2.x checkpoints.
- `dev/2.x` owns the next unqualified C3 2.x milestone.
- topic branches are short-lived.
- `attest/v*-candidate-<E>` and `attest/v*-post-<P>` are reserved, short-lived,
  SHA-bound transports for one exact attestation commit. They are never product
  lines or substitutes for either permanent 2.x branch.

Qualified 1.x checkpoints fast-forward from `dev/1.x` to `legacy/1.x`;
applicable 1.x changes are then reproduced and forward-implemented on `dev/2.x`.
2.x-only work never flows
backward. No permanent branch or qualified tag is force-pushed, replaced, or
deleted.

Every alpha, beta, release candidate, and stable tag must be reachable from
`master`. A tag proves qualification, not publication. Because this repository
is public, pushed alpha source and tags are visible; “internal alpha” means that
no binary release is published, announced, or promoted through an update feed.

`release/train/2.0.0.json` makes the current programme step resumable. Validate
it with `build/validate-release-train.ps1`; use the thin orchestration scripts in
`release/train/README.md` to compose the gates below. The train file owns order
and the active pointer only. This release process and `release/catalog.v1.json`
remain authoritative for C/E/P topology, artifacts, and observed external facts.

## Stage gates

| Evidence | Alpha | Beta | Release candidate | Stable |
| --- | --- | --- | --- | --- |
| Metadata, feed isolation, boundaries, tests, both builds, PE checks | Required | Required | Required | Required |
| Two clean path-distinct package builds and checksums | Required | Required | Required | Required under the accepted identity strategy |
| Known data-loss defects | Zero | Zero | Zero | Zero |
| Critical local workflow smoke | Required | Required | Required | Required |
| Full owner workflow matrix | Selected changed/critical paths | Required | Required | Required |
| XP SP3 / Windows 7 SP1 support matrix | May be explicitly deferred | Required for public support claims | Required | Required |
| DPI/accessibility | Changed surfaces | Required | Required | Required |
| GitHub release and post-download verification | No | Required prerelease | Required prerelease | Required public release |
| Update-feed promotion | No | Beta feed last | Beta feed last | Stable feed last |

Beta 1 is feature complete and compatibility complete. Release candidates use
the `beta` channel and `public-prerelease` publication policy. The stable
byte-identity strategy is deliberately unresolved; it must be accepted before
the first release candidate. Until then, C3 claims neither unchanged RC bytes
nor a metadata-only stable rebuild.

## Prepare and freeze source commit C

1. Confirm the accepted milestone scope and applicable evidence tier.
2. Set identity in `build/Version.props`, run `build/sync-version.ps1`, and commit
   every packaged projection, release note, changelog, and user-facing document.
   Synchronization never edits root `VERSION`, the legacy 1.x feed, or a
   publishable beta/stable feed.
3. Finish milestone implementation. Do not start the next milestone's scope.
4. Resolve every known data-loss path and record any allowed deferred evidence.
5. Commit and freeze exact source/payload commit `C`.

No packaged input may change after `C`. In particular, do not edit `README.md`,
`RELEASE_NOTES.md`, build identity, source, resources, manifests, or packaging
logic while writing evidence. C3 2.x channels use `release.json`; do not create
2.x `VERSION` files. The three-line root and `legacy-1x/VERSION` files exist only
for published 1.x compatibility.

## Qualify C

From a clean checkout of exact `C`:

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

## Create qualification attestation E

A commit cannot contain its own SHA. Once `C` passes, create one direct,
single-parent attestation commit `E` that changes exactly these two files:

- `release/catalog.v1.json`; and
- `release/validation/<release-label>.md`.

`E` names the full `C` SHA and exact package names, lengths, and lowercase
SHA-256 values. It records qualification `pass` and promotion `unpromoted`.
Publication remains `unpublished`; post-verification remains `not-applicable`
until public assets exist. This is intentional: the later annotated tag points
to `E`, so `E` cannot truthfully contain its own observed tag-object identity;
`promotion.tagObject` remains null.

Rebuild/package exact `E` and require byte-identical artifacts. Expose it through
its create-only, SHA-bound candidate transport while `origin/dev/2.x` remains at
`C` and `origin/master` remains at the preceding verified checkpoint:

```powershell
$c = '<full frozen C SHA>'
$e = '<full proposed E SHA>'
$previous = '<full expected origin/master SHA>'
$label = '2.0.0-alpha.N'
.\build\invoke-release-ref-transaction.ps1 `
    -Mode CreateCandidate `
    -ReleaseLabel $label `
    -ExpectedCommit $e `
    -ExpectedMasterCommit $previous `
    -ExpectedDevCommit $c `
    -Confirm
```

Run the candidate validator with the full `E` SHA and retained artifacts. It
must prove that `E`
has only `C` as its parent and that `C..E` is exactly the two-file evidence diff.
For Alpha 2 onward, dispatch `candidate-qualification.yml` from trusted
`master` workflow control and supply full `E` plus the exact generated transport
ref. The workflow checks out that SHA explicitly, runs a trusted-master topology
guard before target code, and proves fresh `origin/dev/2.x` still identifies `C`.
The run URL and immutable inputs are release evidence; this manual dispatch is
not treated as a branch-protection status attached to `E`.

For the first 2.0 checkpoint only, run this gate directly on the maintained
compatibility machine and record the commands and host evidence. GitHub permits
manual dispatch only for workflow definitions already present on the default
branch, so the new gate cannot be dispatched while it exists only on `dev/2.x`.
If candidate qualification fails, delete only its exact transport ref with an
exact-object lease and reconstruct `E` from unchanged `C`; a corrected `E` has a
new SHA-bound ref. Do not tag or advance a permanent branch.

## Promote and tag exact E

Only after the complete pre-tag gate passes, create the annotated tag locally at
exact `E`, then use the guarded ref transaction:

```powershell
$tag = 'v2.0.0-alpha.N'
git tag -a $tag $e -m "C3 2.0.0 Alpha N qualified checkpoint"
.\build\invoke-release-ref-transaction.ps1 `
    -Mode PromoteCandidate `
    -ReleaseLabel $label `
    -ExpectedCommit $e `
    -ExpectedMasterCommit $previous `
    -ExpectedDevCommit $c `
    -Confirm
```

This performs one server-atomic, exact-old-object leased transaction: `master`
and `dev/2.x` advance to `E`, the absent annotated tag is created, and the exact
candidate transport ref is consumed. Any moved, missing, reused, non-fast-forward,
or differently tagged ref rejects the complete transaction. Use the real
stage-specific tag. The Alpha 1 tag push bootstraps the workflow definition onto
`master` and triggers independent tag validation; later checkpoints can use the
normal pre-tag dispatch. Tag validation requires full history and proves that:

- the annotated tag resolves exactly to `E` and is reachable from `master`;
- origin exposes both the annotated tag object and its peeled `E` commit;
- tag identity equals `build/Version.props`;
- `E` records qualification `pass` and promotion `unpromoted`;
- `E` has only recorded `C` as its parent and `C..E` is the exact evidence pair;
- expected lanes, package names, lengths, checksums, and retained bytes agree.

Keep both permanent refs quiescent at `E` until the `master` repository check
and automatic tag verification have completed. Their fresh-ref guards are
designed to turn red if `P` races ahead of an `E` event; do not interpret such a
race as qualification evidence.

For beta and release-candidate checkpoints, owner manual qualification of the
exact retained packages is part of the pre-tag gate. Do not tag first and decide
whether those bytes qualified later.

## Perform external operations and create P

`P` is one post-operation commit whose only parent is tagged `E`. It changes the
exact files allowed for the outcome below. Validate the full `P` SHA, then
atomically fast-forward both `master` and `dev/2.x` to that exact commit. Only after
verified `P` is on both refs may `dev/2.x` begin the next release identity or scope.

### Intentionally unpublished alpha

Do not create a GitHub release or promote an alpha feed. Alpha `P` changes
exactly:

- `release/catalog.v1.json`; and
- the matching validation record.

It records qualification `pass`, promotion `tagged`, publication policy
`intentionally-unpublished`, publication state `unpublished`, feed promotion
`false`, post-verification `not-applicable`, and the exact annotated tag-object
SHA observed from origin.

### Successful public beta or release candidate

Beta and release-candidate checkpoints both use the beta channel and a GitHub
prerelease:

1. create the GitHub prerelease for the tag on `E`;
2. upload the proven ZIPs and `SHA256SUMS.txt` without renaming;
3. download every asset into a clean location;
4. rehash and launch the downloaded packages as required; and
5. prepare matching `release/feeds/beta/release.json` metadata only after all
   post-download checks pass.

Successful public `P` changes exactly the catalogue, matching validation record,
and `release/feeds/beta/release.json`. It records publication `published`,
post-verification `passed`, and feed promotion `true`. The three files are one
atomic repository transaction; ordinary identity synchronization must not edit
the published beta feed.

### Failed public post-verification

If a public tag/release exists but downloaded assets fail any required check, do
not change the beta or stable feed. Failure `P` changes exactly the catalogue and
matching validation record. It records publication `published`,
post-verification `failed`, and feed promotion `false`. Preserve the tag, assets,
hashes, and failure evidence; the checkpoint may be linked as superseded by an
immediate successor.

### Successful stable publication

The same transaction rule applies after an RC/stable identity strategy is
accepted: successful stable `P` changes exactly the catalogue, matching
validation record, and `release/feeds/stable/release.json`; a failure changes
only the two evidence files and leaves the stable feed untouched. The accepted
strategy must define which bytes are qualified and whether a stable rebuild is
required. This document intentionally does not decide that unresolved contract.

## Promote exact P

First expose exact `P` through its create-only, SHA-bound attestation ref so the
private compatibility runner can check it while both permanent refs still
identify `E`:

```powershell
$p = '<full proposed P SHA>'
$e = '<full tagged E SHA>'
.\build\invoke-release-ref-transaction.ps1 `
    -Mode CreatePost `
    -ReleaseLabel $label `
    -ExpectedCommit $p `
    -ExpectedMasterCommit $e `
    -ExpectedDevCommit $e `
    -Confirm
$attestBranch = "attest/v${label}-post-${p}"
```

Dispatch `post-promotion-attestation.yml` from trusted `master` workflow control
and supply both the full `P` SHA and the temporary branch name. The workflow
checks out `P` explicitly, rejects a missing/mismatched attestation ref,
reproduces the retained payload, and runs `PostPromotion` mode while fresh
`origin/master` and `origin/dev/2.x` still equal tagged `E`. Its run URL and inputs,
not a status attached to the temporary branch, are the durable evidence. A
failed gate does not advance either permanent ref. Delete only that exact failed
transport ref with an exact-object lease, correct the evidence, and reconstruct
`P` as a direct child of `E`; the replacement SHA receives a different transport
name.

```powershell
$attestRef = "refs/heads/${attestBranch}"
git push --force-with-lease="${attestRef}:${p}" origin ":${attestRef}"
```

After that exact workflow run succeeds, atomically advance both permanent refs
to the same `P` and consume the transport branch:

```powershell
.\build\invoke-release-ref-transaction.ps1 `
    -Mode PromotePost `
    -ReleaseLabel $label `
    -ExpectedCommit $p `
    -ExpectedMasterCommit $e `
    -ExpectedDevCommit $e `
    -Confirm
```

The helper re-reads every remote object, proves topology and fast-forward
relationships, then uses exact-old-object leases in one atomic push to advance
both permanent refs and consume the exact transport ref. A race rejects every
update. The hosted repository check then independently runs `Master` mode against
the pushed ledger state. The resulting permanent history is linear: `C ->
E(tag) -> P`, and both permanent 2.x refs name verified `P`. Commit the next
identity separately on `dev/2.x`, but only after the `master` repository attestation
for `P` finishes while both permanent refs remain quiescent at `P`.

## Rollback and immutability

Retain prior verified payloads and feed metadata. Never delete or relabel
historical tags, assets, hashes, validation, or catalogue history. A release
rollback promotes a previously verified payload through a new evidence-backed
transaction; it does not automatically reverse a user's catalogue migration.
Catalogue rollback uses the preserved original/export and its own documented
workflow.

The repository currently has no explicit licence. Selecting one is an owner
decision and remains a hard gate before the first public C3 2.0 beta is tagged or
published, or before C3 advertises third-party reuse or redistribution rights. It
does not block qualified, intentionally unpublished Alpha checkpoints.
