# ADR 0010: Rebuild and requalify stable identity from accepted RC source

- Status: Accepted
- Date: 2026-08-05

## Context

C3 embeds stage identity in assembly/file/informational metadata and packaged
documentation. A release candidate therefore cannot become a correctly labelled
stable release by renaming its archive. Claiming unchanged RC bytes would either
publish preview identity as stable or conceal that the stable payload changed.

The alternative of externalizing every stage-bearing value before the final RC
would complicate runtime diagnostics and still require careful proof that no
stage projection remained in the payload.

## Decision

After the owner accepts an exact release candidate, create a direct
metadata-only stable source commit from that accepted RC source. The commit may
change only canonical release identity and its generated/documented projections;
it may not change product behavior, dependencies, catalogue formats, migration,
or payload composition.

Build the stable identity as new bytes and run the complete stable qualification
gate: both lanes, behavioral and compatibility suites, binary/PE identity,
path-distinct package reproducibility, stage-required manual evidence, immutable
publication, clean re-download and rehash, and stable-feed promotion last. Any
functional correction requires another release candidate, not expansion of the
metadata-only commit.

Stable and its accepted RC are source-equivalent except for the audited identity
transition. They are not byte-identical artifacts. The stable validation record
links the accepted RC, identifies the exact metadata diff, and records the newly
qualified stable hashes.

## Consequences

- Public binaries and diagnostics truthfully identify the stable stage.
- C3 never relabels or silently reuses preview bytes as stable.
- The stable release is a fresh qualified checkpoint with its own C/E/P evidence.
- RC evidence remains prerequisite evidence but does not waive stable rebuild,
  reproducibility, manual, or post-download gates.
- Any non-identity source or payload difference stops stable qualification and
  returns development to a new RC.
