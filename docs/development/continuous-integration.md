# Continuous Integration

C3 uses hosted contract checks and private compatibility gates. A workflow name
describes evidence that it actually produces; repository checks are never
presented as compiled compatibility evidence.

## Hosted repository checks

`.github/workflows/repository-checks.yml` runs for pushes and pull requests on a
pinned Windows Server 2022 image. It verifies:

- canonical identity and machine-readable release metadata;
- strict UTF-8, duplicate-member, size-limit, and supported-schema validation;
- release-catalogue schema, lifecycle projections, and failure paths;
- create-only transport refs, exact-old leases, and atomic promotion fixtures;
- trusted-master target topology and ref-position guards;
- checkpoint topology when the event supplies the required Git history;
- the shared deterministic compiler contract;
- dependency direction between production modules;
- typed WinForms access through catalogue services and the C3-owned preference
  boundary, with `My.Settings` reintroduction prohibited;
- identical physical source ownership in both WinForms project files;
- local documentation links; and
- whitespace integrity.

These checks intentionally do not compile C3. Microsoft documents that Visual
Studio 2022 and later cannot build applications targeting .NET Framework 4.0
through 4.5.1. GitHub retired its Visual Studio 2019-capable `windows-2019`
hosted runner on 30 June 2025.

## Release-contract validation modes

The release validator separates repository shape from immutable checkpoint
transactions:

| Mode | Contract |
| --- | --- |
| Repository | Validate schema, ordering, lifecycle consistency, channel policy, record projections, and current identity without claiming qualification. |
| Candidate (`E`) | Require full `E` SHA, retained artifacts, qualification `pass`, promotion `unpromoted`, `E` with only `C` as its parent, and an exact catalogue-plus-validation diff. |
| Tag | Require an annotated tag exactly at `E`, reachable from `master`, while the tagged snapshot truthfully remains `unpromoted`. |
| Post-operation (`P`) | Require full `P` SHA and retained artifacts; prove it has only tagged `E` as its parent and its exact diff matches the observed outcome. |
| Master ledger | Require `master` HEAD to be one of the verified forms: exact qualified `E` or exact post-operation `P`. |

Alpha `P` and public post-verification-failure `P` change exactly the catalogue
and matching validation record. Successful public `P` changes those two files
plus the matching `release/feeds/beta/release.json` or
`release/feeds/stable/release.json`. Release candidates use the beta path. No
2.x `VERSION` file participates in any mode.

## Authoritative compatibility gates

Four narrowly scoped workflows use a self-hosted runner in the
`c3-private-release` runner group with these capability labels:

```text
self-hosted
Windows
X64
c3-legacy-msbuild
```

The runner must satisfy the [toolchain policy](toolchain.md). The maintained
machine currently resolves Visual Studio 2017 Enterprise 15.9. It must have:

- Visual Studio 2017 15.9 with MSBuild and VB/C# desktop support;
- the .NET Framework 4.0 targeting pack;
- the .NET Framework 4.8 targeting pack; and
- Windows PowerShell 5.1 or later.

| Workflow | Trigger and authority |
| --- | --- |
| `full-compatibility.yml` | Manually exercises the moving `dev` branch as development evidence only. |
| `candidate-qualification.yml` | Dispatched from trusted `master`; independently guards and qualifies full `E` from exact `attest/v*-candidate-<E>` while `origin/dev` remains at `C`. |
| `tagged-checkpoint-verification.yml` | Automatically re-verifies each pushed `v2.*` annotated tag at exact `E`. |
| `post-promotion-attestation.yml` | Dispatched from trusted `master`; independently guards and qualifies full `P` from exact `attest/v*-post-<P>` while both ledger refs still identify `E`. |

Each gate runs the complete repository contract, compiles both release lanes,
runs characterization, verifies binary identities and PE architecture, and
builds the payload twice from clean path-distinct source roots. The candidate,
tag, and post-operation gates also compare the reproduced packages with the
release catalogue. Candidate and post-operation dispatches require the complete
immutable `E` or `P` SHA; a moving branch name is not release evidence. The
workflows and jobs deliberately have distinct names so branch protection and a
validation record cannot confuse development, pre-tag, tag, and post-operation
results. The candidate and post-operation workflows use the reviewed `master`
workflow definition, check out trusted control and target trees separately, and
run the trusted topology/ref guard before any target-commit script. Target
scripts then perform the complete semantic and artifact validation. Both gates
require the release-owned SHA-bound transport ref to identify the exact supplied
commit. Their run URL and immutable inputs are evidence; they are not represented
as commit-status contexts attached to the checked-out SHA.

The candidate-qualification run is mandatory before `E` is promoted or tagged.
Alpha 1 is the bootstrap exception in transport, not rigor: GitHub only
allows `workflow_dispatch` for a workflow definition present on the default
branch, while this new definition initially exists only on `dev`. Run the same
candidate commands directly on the maintained machine against exact Alpha 1 `E`
and record that evidence. The Alpha 1 tag push then places the workflow on
`master` and triggers its independent tag check. Later checkpoints use the
normal pre-tag dispatch. A tag-triggered run is defense in depth, never the first
release gate.

Intentionally unpublished alpha packages remain local qualification artifacts
and are not uploaded by CI. Candidate, tag, and post-operation workflows may
retain public-stage packages for 14 days solely as inputs to the controlled
publication and verification procedure.

## Branch protection

Make the hosted contract job required for pull requests. Protect `master` from
direct feature commits, force-push, and deletion; protect `maintenance/1.x` from
force-push/deletion and 2.x identity changes; and prevent replacement of `v2.*`
tags. `master` advances only through the documented exact-SHA transaction:

1. expose `E` through create-only `attest/v*-candidate-<E>` while `dev` remains
   at `C`, then use exact-old-object leases to atomically advance both permanent
   refs to verified `E`, create its absent annotated tag, and consume the ref;
2. expose direct child `P` on create-only `attest/v*-post-<P>`, pass the exact-P
   self-hosted gate while both permanent refs remain at `E`, then use exact-old
   leases to atomically advance both permanent refs and consume the temporary
   ref.

Promotion tooling must accept the full validated SHA and reject a moving branch
name. Ordinary version synchronization must not mutate published beta/stable
feeds; only a successful public `P` owns that change.

The transaction deliberately has quiescent observation windows: wait for the
`E` master/tag checks before constructing or exposing `P`, and wait for the `P`
master check before committing the next identity on `dev`. Fresh-ref validation
will reject an event whose permanent refs have already raced to the next state.

Do not make the manually dispatched self-hosted job a routine pull-request
requirement because an offline private runner would leave ordinary contributions
permanently queued. It is mandatory evidence before promoting any checkpoint and
for every public release candidate.

Create GitHub environments named `c3-release-qualification` and
`c3-development-compatibility`. The release environment permits only `master`
and protected `v2.*` tags and requires owner/designated-reviewer approval; the
development environment permits only `dev` and carries no release authority.
Bind jobs to the private `c3-private-release` runner group, restrict that group
to this repository, and use an isolated/ephemeral compatibility worker with no
unrelated credentials. A workflow ref check is a second line of defense, not a
replacement for environment, immutable-tag, and runner policy. Without those
repository-side settings, the self-hosted gates are not authorized for release
use. If the current hosting account/plan cannot provide an organization-scoped
runner group, do not attach a long-lived personal workstation directly to this
public repository: migrate the release runner boundary or approve an isolated
ephemeral alternative before enabling these workflows.

## Supply-chain policy

Workflows have read-only repository permissions and checkout does not persist its
credential for repository scripts. Third-party actions are limited to
GitHub-maintained checkout and artifact actions, pinned to reviewed full commit
SHAs with their human release version retained in a comment. Renovation of an
action SHA is reviewed and tested like any other dependency change; moving major
tags and `@main` are not release-gate inputs.

## References

- [Microsoft: .NET Framework and Windows OS versions](https://learn.microsoft.com/en-us/dotnet/framework/install/versions-and-dependencies)
- [GitHub: Windows 2019 runner retirement](https://github.com/actions/runner-images/issues/12045)
