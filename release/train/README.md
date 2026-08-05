# C3 release-train state

`2.0.0.json` is the small v2 resumable controller for the C3 2.0 programme. It owns
milestone order, the active pointer, and publication policy. It does not duplicate
the release catalogue's artifact evidence or pretend that a commit can contain
its own SHA.

## Ownership

| Fact | Canonical owner |
| --- | --- |
| Planned order and current milestone | `release/train/2.0.0.json` |
| Current build identity | `build/Version.props` |
| C/E/P lifecycle and package evidence | `release/catalog.v1.json` |
| Exact immutable qualification | annotated tag and its validation record |
| Exact in-flight candidate | SHA-bound attestation ref, or the Beta candidate branch |
| Milestone scope and exit gate | `docs/planning/2.0-execution-plan.md` |

`candidateCommit` is nullable by design. A frozen commit cannot record its own
SHA. Once qualification creates `E`, the release catalogue records source commit
`C`; while a candidate is in flight, its SHA-bound Git ref is authoritative. The
field may cache an already-recorded catalogue value, but the validator rejects a
different value.

Version 2 records Alpha 1 through Alpha 12 followed by Beta 1. The preserved v1
schema remains historical evidence of the original Alpha 1-6 plan and is not
rewritten.

The transition to the next milestone happens in one ordinary commit after exact
post-operation `P` is verified on both `master` and `dev/2.x`. That commit marks the
previous milestone `qualified`, advances `currentMilestone`, clears
`candidateCommit`, updates `lastQualifiedTag`, and changes the build identity.

Beta 1 ends in `awaiting-owner-manual-validation`. Its exact
`candidate/2.0.0-beta.1` branch and `dev/2.x` must identify the same frozen commit.
The controller never tags, publishes, or promotes the beta feed without the
owner's explicit acceptance.

## Orchestration commands

The train scripts compose the existing authoritative gates; they do not contain
a second build, packaging, or release-contract implementation:

```powershell
# Continuous development gate; add -Reproduce at candidate freeze.
.\build\verify-milestone.ps1 -ExpectedMilestone alpha.1 -Rebuild

# Require clean dev/2.x and return the exact frozen source commit C.
.\build\freeze-candidate.ps1 -ExpectedMilestone alpha.1 -Rebuild -Reproduce

# Wrap the guarded create/promote C/E/P reference transaction for an Alpha.
.\build\promote-alpha.ps1 -Phase CreateCandidate ... -Confirm

# Validate the immutable annotated tag against repository and package evidence.
.\build\validate-tag.ps1 -RequireArtifacts

# After verified P is on master and dev/2.x, create the next identity projections.
.\build\start-next-milestone.ps1 -Milestone alpha.2 -Confirm

# At feature-complete Beta 1, reproduce and create its exact candidate branch.
.\build\prepare-beta-candidate.ps1 -Rebuild -Push -Confirm
```

Mutating wrappers use PowerShell's standard `ShouldProcess`, `-WhatIf`, and
`-Confirm` behavior. Promotion still delegates to
`invoke-release-ref-transaction.ps1`, including create-only refs, annotated-tag
checks, atomic push, and exact-old-object leases.
