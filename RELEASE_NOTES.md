# Compact Cassette Catalogue 2.0.0 Alpha 4

C3 2.0.0 Alpha 4 is an in-development checkpoint in the repository-owned 2.0
release train. Its scope, evidence, limitations, and package identities will be
recorded here before candidate freeze.

This checkpoint is not published. See the
[execution plan](docs/planning/2.0-execution-plan.md) and
[validation record](release/validation/2.0.0-alpha.4.md).

## Candidate scope implemented so far

- Native `.c3catalogue` is strict deterministic UTF-8 XML with stable opaque
  identities, exact typed-reference validation, bounded parsing, and canonical
  writer output.
- Legacy v1.1 catalogues can be inspected through a deterministic dry run,
  migrated transactionally to a new copy with recovery/report evidence, or kept
  in explicit compatibility mode. The original is never silently overwritten.
- Loss-aware v1.1 export previews unsupported data and writes only when the
  caller accepts the reported result.
- `c3.exe` supplies headless validate, migrate, recover, and legacy-export
  operations without implementing a second catalogue parser or migration path.
- Both portable lanes consume one strict distribution profile contract and one
  payload manifest. Each ZIP extracts beneath one versioned root and includes
  Desktop, CLI, shared assemblies, build identity, README, and these notes.

The checkpoint remains in development until the combined native reader/writer,
migration/export, historical-reader, CLI, package, reproducibility, and recovery
gates pass against its exact frozen source commit. Windows XP SP3, Windows 7 SP1,
accessibility, and public-release evidence remain explicit later/manual gates;
this alpha creates no GitHub release or update-feed availability.
