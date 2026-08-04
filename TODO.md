# C3 1.3.0 legacy maintenance workboard

This file is the short operational view. Detailed requirements live in
`docs/planning` and `docs/testing/1.3.0-qualification-matrix.md`. Work is checked
off only when its committed evidence passes; implementation alone is not done.

## Alpha 1 — maintenance foundation

- [x] Recover `dev/1.x` from safe post-release tip `58a5b7d...`.
- [x] Preserve `58a5b7d...` under annotated tag
  `archive/1.2-postrelease-tip`.
- [x] Pin the production genome to exact package source `509c9ec...` while
  retaining `v1.2.0b1` / `2413e913...` as the release oracle.
- [x] Preserve the superseded refactor tip under an annotated archival tag.
- [x] Record the branch correction and permanent branch roles.
- [x] Define the frozen 1.3 genetic compatibility constitution.
- [x] Create the itemized refactor salvage ledger.
- [x] Project the catalogue 1.1.0 specification and fixtures.
- [x] Project historical release provenance and settings fixtures.
- [x] Add strict compatibility-corpus and JSON validation.
- [x] Add standalone VB.NET XML characterization tests.
- [x] Generate the baseline-genome manifest and empty allow-list.
- [x] Add one-project build lanes for x86/net40, x64/net40, x64/net48, and
  x64/net481.
- [x] Verify PE architecture, framework metadata, settings parity, and no runtime
  DLL payload.
- [x] Download and hash-check the official `v1.2.0b1` x86/x64 executables.
- [x] Rebuild all four diagnostic lanes from the production-identical
  58a-derived reconstruction checkpoint.
- [x] Run the corrected three-anchor baseline verifier and record exact evidence.
- [ ] Promote `legacy/1.x` to the qualified production-identical reconstruction
  checkpoint.
- [x] Document the Alpha 1, Beta 1, and stable milestone gates.
- [ ] Set and mechanically verify `1.3.0 / Alpha 1` source identity while leaving
  the public 1.2 feed unchanged.
- [ ] Rebuild all four diagnostic lanes from the exact Alpha candidate.
- [ ] Run the complete Alpha validator and record exact evidence.
- [ ] Create and verify annotated tag `v1.3.0a1`.
- [ ] Push the exact `dev/1.x` tip and absent tag atomically.

Alpha 1 is intentionally unpublished. It does not advance `legacy/1.x`, publish
binaries, or change the update feed.

## Beta 1 entry — finish baseline reconstruction

- [ ] Install or reconstruct VS2015/MSBuild `14.0.25420.1`.
- [ ] Rebuild `v1.2.0b1` x86/net40 and x64/net40 with the authoritative toolchain.
- [ ] Repeat launch, catalogue, list/filter/edit/delete, settings, and blocked-
  network workflows in disposable environments.
- [ ] Retain representative real catalogues as private test copies outside Git.
- [ ] Capture baseline screenshots and control/resource evidence.
- [ ] Prove `v1.1.2` and both `v1.2.0b1` architectures exchange the canonical
  catalogue without semantic loss.

No Beta runtime repair starts until this entry gate passes or the owner records
a specific governance amendment.

## Beta 1 — lifecycle and data safety

- [ ] S1: resolve pending tape edits before close/open/new/scroll transitions.
- [ ] S1: replace recursive close cancellation with one explicit close gate.
- [ ] S1: stop Save As cancellation from continuing into save/open operations.
- [ ] S1: eliminate the duplicate Open dialog after save-before-open.
- [ ] S0: load and validate temporary catalogue state before replacing active
  data or path.
- [ ] S0: implement verified same-directory temporary save, backup, replacement,
  and cleanup.
- [ ] S1: detect external catalogue modification before overwrite.
- [ ] S0: replace positional tape updates with explicit named assignments.
- [ ] S1: preserve existing tape identifiers and creation dates during edits.
- [ ] S2: persist peak, bias, and calibration values from the actual controls.

Every item requires an isolated reproduction, regression, minimal original-file
patch, genome validation, and focused commit.

## Beta 1 — referential and counter integrity

- [ ] S2: read model notes from `Models`, not `Brands`.
- [ ] S2: update the deck counter when deleting a deck.
- [ ] S1: block deletion of referenced brands, models, and decks.
- [ ] S1: make brand rename relationship-safe without changing identifiers.
- [ ] S2: derive runtime counts from actual rows.
- [ ] S1: allocate tape sequences from the maximum existing sequence.
- [ ] S1: validate a complete bulk batch before inserting any row.
- [ ] S1: assign every bulk copy an independent monotonic sequence.
- [ ] S2: maintain correct per-model tape counters.
- [ ] S2: clear both deck combo boxes before repopulation.
- [ ] S1: recognize historical display-name `Models.Brand` references without a
  format migration.

## Beta 1 — settings, diagnostics, lanes, and packages

- [ ] Add `settingsUpgradeRequired=True` and guarded `My.Settings.Upgrade()`.
- [ ] Keep migration failures nonfatal and retryable; never alter old profiles.
- [ ] Normalize only known values and preserve directory/message/update settings.
- [ ] Execute every real-profile and cross-lane settings transition.
- [ ] Resolve console export through configured directory/Documents using
  `Path.Combine`.
- [ ] Keep console write failures nonfatal.
- [ ] Use short same-directory temporary filenames on classic Windows paths.
- [ ] Finalize lane-specific config/manifest metadata with identical application
  logic and resources.
- [ ] Add deterministic portable packaging and exact payload verification.
- [ ] Prove two clean, path-distinct builds produce identical ZIPs.
- [ ] Complete all automated, manual, compatibility, and OS Beta gates.
- [ ] Resolve or reject every S0–S2 salvage-ledger entry with evidence.
- [ ] Freeze, qualify, tag, and optionally publish `v1.3.0b1` without changing the
  stable legacy feed.

## Stable 1.3.0

- [ ] Obtain owner acceptance of the exact Beta source and retained packages.
- [ ] Record the licence decision for redistributed package contents.
- [ ] Make a direct metadata-only transition to `1.3.0 / Release`.
- [ ] Audit the transition for zero functional/dependency/format/payload change.
- [ ] Build twice from clean, different absolute paths with authoritative tools.
- [ ] Run the complete automated, manual, compatibility, settings, and OS matrix.
- [ ] Create the evidence-only qualification commit and annotated `v1.3.0` tag.
- [ ] Fast-forward `legacy/1.x` to the qualified checkpoint.
- [ ] Publish the already-qualified ZIPs and checksum manifest without rebuilding.
- [ ] Download and reverify every published byte and critical workflow.
- [ ] Commit post-publication evidence.
- [ ] Promote the root legacy feed to `1.3.0 / Release` only after success.
- [ ] Align `dev/1.x` and `legacy/1.x`, archive evidence, and freeze ordinary 1.x
  development.

## Explicitly outside C3 1.3

- new catalogue fields, formats, plugins, import/export framework, or updater;
- modular-monolith/service/repository architecture;
- C# translation or production source relocation;
- broad UI/DPI/localization redesign; and
- ordinary feature requests formerly listed as 1.3 usability work.

Those belong to 2.x planning. After stable, 1.3.1 is reserved for a critical
data-loss, security, startup, or platform regression.
