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
- [x] Promote `legacy/1.x` to the qualified production-identical reconstruction
  checkpoint.
- [x] Document the Alpha 1, Beta 1, and stable milestone gates.
- [x] Set and mechanically verify `1.3.0 / Alpha 1` source identity while leaving
  the public 1.2 feed unchanged.
- [x] Rebuild all four diagnostic lanes from the exact corrected Alpha candidate.
- [x] Run the complete corrected Alpha validator and record exact evidence.
- [x] Create and verify corrected annotated tag `v1.3.0a1` after preserving the
  original tag object and lineage.
- [x] Atomically publish corrected `dev/1.x`, qualified `legacy/1.x`, the Alpha
  tag, and all preservation tags under exact old-object leases.

Alpha 1 is intentionally unpublished. It does not advance `legacy/1.x`, publish
binaries, or change the update feed.

## Post-Alpha three-lane correction

- [x] Ratify the owner decision superseding the Alpha four-lane release plan.
- [x] Preserve the immutable Alpha tag and its diagnostic result unchanged.
- [x] Define exactly `win-x86-net40`, `win-x64-net48`, and
  `win-arm64-net481` as Beta/stable release lanes.
- [x] Assign VS2017, VS2022, and VS2026 compiler authority respectively.
- [x] Keep portable classic WinForms ZIPs authoritative. This earlier exclusion
  of every installer path is superseded by the Alpha 3 optional classic setup
  decision; updater/network bootstrap paths remain excluded.
- [x] Replace the active lane manifest and package-name contract.
- [x] Add exact-family MSBuild resolution, effective-tools-version checks,
  binary logs, and hash-pinned toolchain evidence.
- [x] Add Debug/Release ARM64 project and solution configurations without
  conditional application source.
- [x] Verify closed x86/x64/ARM64 PE mappings, CLR CorFlags, framework/config,
  version, settings parity, and zero runtime DLLs.
- [x] Split builder inspection from target-machine runtime qualification.
- [x] Mechanically reject ClickOnce, bootstrapper, installer, uninstaller, or
  updater output from the three ZIPs.
- [x] Run the genome/source gates and record that application behaviour did not
  change during this correction.

## Beta 1 entry — finish historical baseline reconstruction

- [x] Download the official Microsoft Build Tools 2015 Update 3 bootstrapper,
  verify version `14.0.25420.1`, exact hashes, and Microsoft Authenticode
  identity, and retain it as compatibility-laboratory input only.
- [x] Reconstruct historical VS2015/MSBuild `14.0.25420.1` as an isolated,
  hash-recorded administrative toolset in the compatibility laboratory; do not
  register the retired product system-wide.
- [x] Rebuild `v1.2.0b1` x86/net40 and x64/net40 with that historical 1.2
  compiler oracle; do not use it for C3 1.3 release builds.
- [ ] Repeat launch, catalogue, list/filter/edit/delete, settings, and blocked-
  network workflows in disposable environments.
- [ ] Retain representative real catalogues as private test copies outside Git.
- [ ] Capture baseline screenshots and control/resource evidence.
- [ ] Prove `v1.1.2` and both `v1.2.0b1` architectures exchange the canonical
  catalogue without semantic loss.

No Beta runtime repair starts until this entry gate passes or the owner records
a specific governance amendment.

## Release-control hardening before Gate 1

- [x] Replace the tracked self-referential lock with an immutable external,
  source-bound candidate lock and retain its SHA-256.
- [x] Reject dirty tracked, staged, untracked, submodule-drifted, or
  remote-mismatched candidate source before compilation.
- [x] Make the actual target scripts parse and execute under Windows PowerShell
  2 without `$PSScriptRoot` or modern JSON dependencies.
- [x] Bind exact extracted file names, lengths, and SHA-256 values to a retained
  package-entry manifest rather than trusting extracted `BUILD.txt`.
- [x] Derive and enforce target OS build, service pack, native architecture,
  and installed Full Framework; prohibit caller-supplied environment labels.
- [x] Force, hash, and freeze the actual `ResGen.exe` used to produce packaged
  bytes, with binary-log and MSBuild-property evidence.
- [x] Pass the consolidated adversarial failure suite and restore a clean tree.
- [x] Classify hosted status checks as supplemental/non-authoritative unless a
  later owner decision adopts a specific hosted qualification environment.
- [x] Require one source, mode, locked status, and external-lock SHA-256 across
  the complete three-package Candidate set.
- [x] Fetch and retain the provider-ref/remote-URL receipt at lock capture, then
  keep the actual Candidate builds offline.
- [x] Repeat source/ref/submodule/genome/lock closure after all Candidate lanes
  and require the retained result before Candidate packaging.
- [x] Update VS2017 to at least 15.9.81, VS2022 to at least 17.14.37, and VS2026
  to at least 18.8.2 (or later stable servicing available at freeze), rebuild
  Preparation evidence, and create the first acceptable external candidate lock.

Servicing completed at VS2017 15.9.81, VS2022 17.14.37, and VS2026 18.8.2.

## Alpha 2 — three-lane control checkpoint

- [x] Define exact `1.3.0a2 / Alpha 2 / v1.3.0a2` source and manifest identity.
- [x] Project `1.3.0a2` into source display constants, assembly informational/
  product metadata, package names, `BUILD.txt`, entry manifests, target evidence,
  checksums, release notes, plans, and validation controls.
- [x] Reject mixed-label package sets and stable-looking Alpha package names.
- [x] Enforce channel semantics: Alpha `version+aN / Alpha N`, Beta
  `version+bN / Beta N`, and stable `version / Release`.
- [x] Add exact Alpha tag-message validation and the non-self-referential
  `C → E → tag → P` post-tag record/verifier topology.
- [x] Add a machine-readable qualification record at `E` so post-tag verification
  proves retained package, manifest, build-log, closure, and source-rebuild
  hashes are unchanged rather than merely taking a new snapshot at `P`.
- [x] Add a governed two-worktree Candidate source-rebuild harness; repeated
  packaging of one retained binary set is not source reproducibility.
- [x] Service all three maintained builders to the declared floors or later
  stable releases and discard stale outputs under the validated artifact roots.
- [x] Push clean Alpha source commit `C` and pass the complete source-only suite.
- [x] Rebuild all three lanes in Preparation mode and retain exact tool evidence.
- [x] Fetch `origin/dev/1.x`, capture one external immutable lock bound to `C`,
  and rebuild all three lanes in offline Candidate mode.
- [x] Pass final source/ref/submodule/genome/lane/lock closure.
- [x] Produce exactly the three `C3-v1.3.0a2-...-portable.zip` assets and
  `SHA256SUMS.txt`, with authenticated entry manifests.
- [x] Prove two clean path-distinct Candidate source rebuilds reproduce every
  authoritative build, package, checksum, and entry-manifest byte.
- [x] Run x86/x64 builder smoke and record ARM64 runtime as deferred.
- [x] Commit evidence `E`, create and verify annotated `v1.3.0a2` at `E` with
  the required deferral message, then push the tag.
- [x] Commit direct child `P` changing only the post-tag record; verify remote
  tag object/target and unchanged hashes/feed/legacy/publication boundaries.
- [x] Retain the exact lock/packages/source-build evidence without public release.

Alpha 2 does not close historical Gate 1, repair the inherited recursive-close
defect, qualify a minimum OS, move `legacy/1.x`, or change the public feed.
Every Beta-labelled operation remains prohibited without explicit human approval.

## Alpha 3 — legacy reliability and classic setup

Alpha 3 is now the internal implementation phase. Its uncreated tag and
separate retained-package wave are superseded by the authorized Beta 1
Candidate qualification; no functional or target gate is waived.

- [x] Ratify `1.3.0a3 / Alpha 3 / v1.3.0a3` scope, artifact, setup, and
  authorization doctrine.
- [x] Specify closed payload and installed-state XML manifests.
- [x] Add the S0–S3 application/setup defect and hazard ledger.
- [ ] Complete historical Gate 1 before changing application runtime behaviour.
- [x] Prepare and reproduce the deterministic historical Gate 1 runtime kit with
  exact v1.1.2 and v1.2.0b1 x86/x64 oracle hashes.
- [x] Freeze installer/uninstaller project, form, control, resource, and artwork
  identity before remediation.
- [x] Add matched x86/net40, x64/net48, and ARM64/net481 setup configurations.
- [x] Link the shared VB.NET setup contracts and transaction implementation
  directly into both EXEs without a DLL.
- [x] Implement offline verification, path/environment/elevation preflight,
  staging, journal, install, repair, upgrade, rollback, registry, and shortcuts.
- [x] Implement manifest-bound discovery, self-relocation, reversible removal,
  and unknown/catalogue/settings preservation.
- [x] Complete the classic wizard directory/cancel/error/accessibility flows and
  deduplicate repeated artwork without redesigning it.
- [x] Build and adversarially verify deterministic setup bundles from exact
  qualified portable payload bytes.
- [x] Add a PowerShell 2-compatible target setup preflight that authenticates
  the ten extracted bundle entries, derives the exact target environment, and
  keeps real setup launch/mutation explicit.
- [ ] Execute and retain every clean/repair/upgrade/rollback/uninstall,
  ownership, keyboard, DPI, and high-contrast row from the target setup record
  on all three exact native target environments.
- [ ] Pass two clean path-distinct product/setup Candidate builds from one
  source commit and one immutable external lock.
- [ ] Pass the full per-lane install/repair/upgrade/uninstall/fault matrix on XP,
  Windows 7, and native Windows 11 ARM64.
- [x] Supersede the uncreated Alpha 3 retained/tag checkpoint with the single
  authorized Beta 1 Candidate build and qualification wave.

## Alpha 3 — lifecycle and data safety

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

## Alpha 3 — referential and counter integrity

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

## Alpha 3 — settings, diagnostics, lanes, and packages

- [x] Add `settingsUpgradeRequired=True` and guarded `My.Settings.Upgrade()`.
- [x] Keep migration failures nonfatal and retryable; never alter old profiles.
- [x] Normalize only known values and preserve directory/message/update settings.
- [ ] Execute every real-profile and cross-lane settings transition.
- [x] Resolve console export through configured directory/Documents using
  `Path.Combine`.
- [x] Keep console write failures nonfatal.
- [x] Keep console logging, browser launch, and update-failure reporting
  nonfatal when diagnostic UI is unavailable.
- [ ] Use short same-directory temporary filenames on classic Windows paths.
- [ ] Freeze the latest serviced VS2017 15.9, VS2022 17.14, and VS2026 stable
  installations in an immutable external source-bound lock immediately before
  candidate qualification; retain its SHA-256 in packages and evidence.
- [x] Finalize exactly three lane-specific config/manifest projections with
  identical application logic and resources.
- [x] Add deterministic portable packaging and exact payload verification.
- [x] Prove two clean, path-distinct portable builds produce identical ZIPs for
  Alpha 2; repeat for the completed Alpha 3 product and setup sources.
- [ ] Qualify XP SP3 x86/net40, Windows 7 SP1 x64/net48, and native Windows 11
  RTM ARM64/net481 on their target machines using exact retained package hashes.
- [ ] Complete all automated, manual, compatibility, and OS Beta gates.
- [ ] Resolve or reject every S0–S2 salvage-ledger entry with evidence.
- [ ] Freeze and qualify the Beta-eligible source and evidence, then obtain
  explicit human approval before producing Beta-labelled distributions,
  creating `v1.3.0b1`, or optionally publishing it; never change the stable
  legacy feed for Beta.

## Stable 1.3.0

- [ ] Obtain owner acceptance of the exact Beta source and retained packages.
- [ ] Record the licence decision for redistributed package contents.
- [ ] Make a direct metadata-only transition to `1.3.0 / Release`.
- [ ] Audit the transition for zero functional/dependency/format/payload change.
- [ ] Build exactly the three ratified lanes twice from clean, different
  absolute paths with locked authoritative tools.
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
- broad application UI/localization redesign;
- MSI, MSIX, ClickOnce publication, network bootstrapper, background updater,
  service, or self-contained runtime;
- a separately qualified Universal Setup product binding; and
- ordinary feature requests formerly listed as 1.3 usability work.

Those belong to 2.x planning. After stable, 1.3.1 is reserved for a critical
data-loss, security, startup, or platform regression.
