# Contributing to C3

C3 values reviewable changes that preserve user catalogues, keep both Windows
lanes behaviorally identical, and leave clearer ownership than they found.

The repository currently has no explicit license. Contribution does not imply a
license grant beyond GitHub's applicable terms; selecting a project license is an
owner decision and an open 2.0 foundation task.

## Choose the correct branch

- Target `dev` for C3 2.0 features, refactors, ports, documentation, and fixes.
- Target `master` only for bounded C3 1.2 maintenance/security work.
- Carry accepted 1.x fixes forward to `dev` with the same regression evidence.
- Never backport a 2.x-only model, format, UI, or update-channel change to
  `master` merely to reduce diff size.

Use a short-lived topic branch. Do not force-push either permanent branch.

## Read before changing code

1. Read the [product vision](docs/product/vision.md) and
   [2.0 scope](docs/product/c3-2.0-scope.md).
2. Read the [architecture](docs/architecture/README.md),
   [repository ownership map](docs/architecture/repository-layout.md), and the
   relevant ADRs.
3. For data/settings/update work, read the
   [1.x compatibility charter](docs/compatibility/1x-to-2x-charter.md).
4. Identify the single current owner of the behavior.
5. Add or update a regression test, fixture, differential comparison, or named
   manual reproduction before changing behavior.

## Design rules

- Put catalogue concepts and rules in `C3.Catalogue`, external mechanisms in
  `C3.Infrastructure`, and interaction/layout/composition in `C3.WinForms`.
- Organize inside a module by product feature. Do not add generic `Core`,
  `Common`, `Helpers`, `Managers`, `Platform`, `Misc`, or speculative directories.
- Keep `DataSet`, raw XML, filesystem, concrete settings, and update transport at
  their existing boundaries. Forms use typed commands/services and results.
- Keep one dirty-state authority and one production owner for each behavior.
- Do not use VB default form instances for coordination.
- Do not put conditional compilation in designer-generated files.
- Do not change a catalogue format implicitly. A change requires a versioned
  specification, fixtures, migration/export behavior, security limits, and
  compatibility notes before implementation is promoted.
- Treat paths, catalogue contents, logs, screenshots, settings, migration maps,
  and support bundles as potentially private.

## Language and porting rules

C# 7.3 is the target language for new or mechanically ported reusable managed
2.0 code. Pin it explicitly; never use `latest` or `preview`. Existing VB remains
the behavioral oracle until the replacement passes both lanes and relevant
manual parity.

A language port changes language and project plumbing only. Do not combine it
with API redesign, behavior changes, broad renames, formatting, or cleanup. Once
the new implementation is promoted, remove the old production owner promptly.

C11 belongs to Universal Setup or a separately accepted bootstrap/native adapter.
C++11 requires a measured isolated boundary. Neither owns C3 domain, migration,
persistence, settings, or UI behavior.

See [ADR 0004](docs/architecture/decisions/0004-managed-language-strategy.md) and
the [toolchain policy](docs/development/toolchain.md).

## Change shape and commits

Prefer a reversible sequence:

1. failing characterization/fixture or recorded reproduction;
2. narrow boundary or seam;
3. behavior-preserving move or language translation;
4. focused behavior change;
5. cleanup after every relevant gate passes.

Keep moves separate from edits so history remains reviewable. Keep WinForms
designer serialization changes with the UI outcome they implement, or in a
separate mechanical commit.

Commit subjects use an imperative Conventional Commit style. Commit bodies state:

- why the change is needed;
- the owning boundary and alternatives rejected;
- catalogue/settings/runtime compatibility impact;
- migration, rollback, or recovery behavior where relevant; and
- exact automated and manual verification performed.

## Required evidence

Run from the repository root:

```powershell
.\build\verify.ps1 -Rebuild
```

At minimum, applicable changes pass metadata/feed validation, dependency and UI
boundaries, shared-project parity, documentation links, characterization, both
release builds, all packaged binary identities, PE architecture, and
`git diff --check`.

Additional gates:

- catalogue changes: source-version fixture and old/new reader/writer evidence;
- settings changes: first, repeated, missing, and malformed profile paths;
- migration/export changes: dry run, deterministic mapping, loss report, failure
  isolation, and baseline-reader evidence;
- UI changes: designer open/save, keyboard, accessibility, empty/error/long-data,
  DPI, dirty/close, and both-lane workflows;
- packaging/setup changes: deterministic hashes and exact payload equivalence;
- runtime claims: exact minimum-OS VM evidence for the candidate.

Never turn an unverified Windows XP, Windows 7, DPI, setup, signing, or
compatibility path into a claim through wording alone.

## Planning and review

The [execution plan](docs/planning/2.0-execution-plan.md) owns 2.0 dependency
order. `ROADMAP.md` owns public outcomes. Issues or future pinned AIDE work units
own assignments. Do not add another Markdown backlog.

Pull requests should be small enough to explain and revert. Review the diff, not
only the passing gate, and call out any evidence that remains manual or pending.
