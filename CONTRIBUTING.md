# Contributing to C3

Thank you for improving C3. The project values small, reviewable changes that
preserve users' catalogues and behave consistently in both Windows build lanes.

## Before changing code

1. Read [`docs/architecture/README.md`](docs/architecture/README.md).
2. Identify the single owner of the behavior you are changing.
3. Add or update a regression test or fixture for observable behavior.
4. Keep unrelated renames, moves, formatting, and behavior changes separate.

## Design rules

- Put domain rules in `C3.Catalogue`, external mechanisms in
  `C3.Infrastructure`, and interaction/layout in `C3.WinForms`.
- Keep feature files together; do not create generic catch-all folders.
- Do not expose `DataSet` or `DataRow` outside the legacy XML adapter boundary.
- Do not use Visual Basic default form instances for cross-form coordination.
- Do not add conditional compilation to designer-generated files.
- Do not change the catalogue format implicitly. Format changes require a
  versioned specification, fixtures, migration behavior, and compatibility notes.
- Treat paths, error messages, logs, and support bundles as potentially private
  user data.

## Change shape

Prefer vertical slices that compile and can be reverted independently:

1. characterization test;
2. boundary or seam;
3. behavior-preserving move;
4. focused behavior change;
5. cleanup after both lanes pass.

Commit messages use an imperative conventional subject and an explanatory body.
The body should state the reason, compatibility effect, and verification run.

## Required checks

Before submitting a change, run the repository verification script. At minimum,
changes must build both lanes, pass automated tests, pass project-parity and
catalogue-fixture checks, and produce no `git diff --check` errors. UI changes
also require the relevant manual smoke workflow and a designer-open check.

Never claim Windows XP, DPI, installer, signing, or upgrade compatibility without
recorded evidence for that exact path.

Pull requests receive hosted repository-contract checks. The authoritative
dual-lane build uses a Visual Studio 2019 self-hosted runner because newer Visual
Studio releases cannot compile the .NET Framework 4.0 lane. See
[`docs/development/continuous-integration.md`](docs/development/continuous-integration.md).
