# ADR 0016: Unpublished alpha qualification authority

Status: Accepted

Date: 6 August 2026

## Context

C3 2.0 uses several intentionally unpublished alpha checkpoints to integrate a
large compatibility-preserving overhaul. Repeating the complete owner-operated
real-display, assistive-technology, minimum-OS, and usability matrix for every
internal checkpoint would transfer engineering qualification work to the owner
and delay convergence without creating a distributable release.

Automated layout, accessibility-contract, interaction, exact-package,
cross-lane, compatibility, and reproducibility evidence cannot prove what a
person observes on physical displays or through a screen reader. Deferring those
checks must therefore remain distinguishable from passing them.

## Decision

Intentionally unpublished alpha checkpoints are qualified by the complete
maintained-machine automated engineering gate appropriate to their changed
surface. This includes both lanes, exact-package process/workflow tests,
deterministic UI construction and layout checks, accessibility metadata and
keyboard-contract checks, binary identity, compatibility tests, and two clean
path-distinct reproducible builds.

The repository owner performs the consolidated real-display, high-contrast,
screen-reader, keyboard-usability, minimum-OS, and end-to-end workflow matrix
against the exact feature-complete Beta 1 candidate. Beta 1 is not tagged or
published until that evidence passes.

Every alpha validation record names the deferred rows and states that they are
not passed. No unpublished alpha creates an operating-system, accessibility, or
usability support claim. A Beta manual failure creates corrected candidate bytes
and repeats both automated and owner gates.

## Consequences

- Codex and engineering automation may progress through qualified unpublished
  alphas without requiring routine owner interaction.
- Alpha C/E/P integrity, data-safety, compatibility, security, deterministic
  packaging, and exact-package automation remain mandatory.
- Manual evidence is consolidated rather than discarded.
- Beta 1 remains the irreducible owner acceptance boundary.
- A missing automated equivalent for a changed alpha behavior remains a blocker;
  it cannot be relabelled as deferred manual evidence.
