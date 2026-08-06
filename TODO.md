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
The 6 August 2026 owner decision authorizes production and retention of the six
`1.3.0b1` Candidate ZIPs. The Beta tag and lease-protected `legacy/1.x`
advancement are authorized only on complete GO; public publication, `master`,
`dev/2.x`, the stable feed, and stable release remain unauthorized.

## Alpha 4 — owner acceptance test distribution

- [x] Project the completed repository-side scope back to an honest
  `1.3.0a4 / Alpha 4 / v1.3.0a4` test identity.
- [x] Build and verify exactly three portable and three classic-setup ZIPs from
  one clean source commit and one immutable external toolchain lock.
- [x] Reproduce all six ZIPs and their entry evidence from two clean,
  path-distinct source checkouts.
- [x] Create and push annotated `v1.3.0a4` only after the retained evidence
  record binds the exact source, lock, package, and checksum hashes.
- [x] Copy the verified local test bundle to the root development worktree's
  untracked `tmp/` directory without modifying its `dev/2.x` files.
- [x] Record the owner-test result: Alpha 4 exposed overlapping controls,
  unstable resize relationships, and runtime widget movement. Preserve its tag
  and bytes as discovery evidence; do not promote them to Beta.

## Alpha 5 — source topology and legacy layout stabilization

The repository remains identified as Alpha 4 until the implementation and
qualification gates earn the Alpha 5 transition.

- [x] Ratify the Alpha 5 visual inheritance, native-layout, fidelity, metric,
  accessibility, performance, and evidence contracts.
- [x] Choose the original-simple 1.x topology rather than a partial 2.x
  `src/` migration.
- [x] Remove ignored `dev/2.x` `src/` build residue from the shared local
  checkout; no tracked 1.x source existed there.
- [x] Move the byte-identical `SetupShared` implementation to
  `Compact Cassette Catalogue Installer/Shared/`, relink both setup projects
  and tests, update validators, and pass the topology/genome/source gates.
- [x] Reproduce Alpha 4 overlap, dynamic-command, form-scroll/anchor, tab-order,
  and handler-geometry failures with a fresh-process STA harness; retain six
  control-tree JSON records, screenshots, and the 52-item source-policy record.
- [ ] Move every known static command into its form Designer and remove the
  geometry-oriented `CatalogueUx` helpers.
  - [x] Move Brand New and Deck New Cancel commands into reviewed Designer
    declarations with closed default/cancel semantics.
  - [x] Move Model New and Tape New prerequisite/Cancel commands into reviewed
    Designer declarations and remove their runtime rectangles/event wiring.
  - [x] Move Tapes/Models/Brands/Decks browser Add commands and current action
    stack geometry into reviewed Designer declarations.
  - [x] Move the Main empty-state command, guidance, and overlay into the
    Designer; close their exact control names and event wiring mechanically.
- [x] Reconstruct `frmMain` in separate structural and adaptive commits.
  - [x] Reparent the existing menu, header groups, editor groups, and empty
    state into a Designer-owned root/header/overlay/viewport hierarchy; remove
    form-level scrolling and the runtime Main layout call.
  - [x] Make Find and Identification fields adaptive at the 800-pixel minimum
    and close their parentage and reachability in characterization tests.
  - [ ] Retain the commit-bound Main atlas and complete native DPI, keyboard,
    High Contrast, and assistive-technology review before qualification.
- [x] Reconstruct `frmTapeNew` in separate structural and adaptive commits.
  - [x] Reparent the editor into a dedicated scrolling canvas, stack its five
    metadata groups, and keep Add Deck/Add Tape/Cancel in a persistent
    Designer-owned command region outside that viewport.
  - [x] Replace the remaining fixed geometry inside the Model group with a
    local field table and close command-bar ownership/reachability.
  - [ ] Retain the commit-bound Tape New atlas and complete native DPI,
    keyboard, High Contrast, and assistive-technology review.
- [ ] Reconstruct the Tapes/Models/Brands/Decks SplitContainer browser family.
  - [x] Rebuild Brands with a fixed filter pane, fill results list, and
    persistent status/actions footer; remove its runtime layout helper.
  - [x] Rebuild Models to the same fixed-filter/fill-results/persistent-footer
    contract and remove its runtime layout helper.
  - [x] Rebuild Decks to the same ownership contract and remove its runtime
    layout helper.
  - [x] Rebuild Tapes to the same ownership contract, replace its 1924-pixel
    default canvas with a practical flexible workspace, and remove its runtime
    layout helper.
  - [ ] Convert each action group to an adaptive command row and retain the
    complete browser-family atlas.
- [ ] Reconstruct compact Brand/Model add/edit dialogs and dense Deck forms.
- [ ] Change Console, Find Results, Statistics, Settings, About, and setup/
  uninstall pages only when the characterization matrix reproduces a defect.
- [ ] Complete visible-text, ellipsis, access-key, default/cancel, tab/focus,
  ErrorProvider, UIA/MSAA, screen-reader, and High Contrast audits.
- [ ] Add bounded command-state, stable selection restoration, and duplicate-
  command protection only where a focused regression requires them.
- [ ] Pass every 800x552/1024x720/1366x728/1920x1040 and
  100/125/150/200% size/scale/content cell in a fresh process.
- [ ] Retain geometry JSON, text/reachability results, diagnostic atlases, and
  native canonical/real-DPI/theme screenshots.
- [ ] Measure repeated open/resize/close, layout events, handles, memory,
  subscriptions, list refresh, and first-window latency.
- [ ] Resolve or retain blocking `UI-START-001` with repeated x86/x64 launch
  milestone evidence; do not classify it as a layout fix without proof.
- [ ] Pass the full source/genome/application/setup/package gates after the
  topology and Designer changes.
- [ ] Project exact `1.3.0a5 / Alpha 5 / v1.3.0a5` identity only at the final
  source freeze.
- [ ] Rebuild all nine executables and reproduce exactly three portable and
  three setup ZIPs from two clean path-distinct checkouts.
- [ ] Complete native owner visual/accessibility testing, create annotated
  `v1.3.0a5`, retain local Alpha packages, and commit post-tag attestation.
- [ ] Obtain explicit human approval before any Beta-labelled package or tag.

## Alpha 3 — legacy reliability and classic setup

Alpha 3 is now the internal implementation phase. Its uncreated tag and
separate retained-package wave are superseded by the authorized Beta 1
Candidate qualification; no functional or target gate is waived.

- [x] Ratify `1.3.0a3 / Alpha 3 / v1.3.0a3` scope, artifact, setup, and
  authorization doctrine.
- [x] Specify closed payload and installed-state XML manifests.
- [x] Add the S0–S3 application/setup defect and hazard ledger.
- [ ] Complete the remaining external historical Gate 1 executions before Beta
  GO. The owner amendment permits reproduction-backed repairs in parallel; no
  unexplained historical deviation or missing exchange cell may be waived.
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

- [x] S1: resolve pending tape edits before close/open/new/scroll transitions.
- [x] S1: replace recursive close cancellation with one explicit close gate.
- [x] S1: stop Save As cancellation from continuing into save/open operations.
- [x] S1: eliminate the duplicate Open dialog after save-before-open.
- [x] S0: load and validate temporary catalogue state before replacing active
  data or path.
- [x] S0: implement verified same-directory temporary save, backup, replacement,
  and cleanup.
- [x] S1: detect external catalogue modification before overwrite.
- [x] S0: replace positional tape updates with explicit named assignments.
- [x] S1: preserve existing tape identifiers and creation dates during edits.
- [x] S2: persist peak, bias, and calibration values from the actual controls.

Every item requires an isolated reproduction, regression, minimal original-file
patch, genome validation, and focused commit.

## Alpha 3 — referential and counter integrity

- [x] S2: read model notes from `Models`, not `Brands`.
- [x] S2: update the deck counter when deleting a deck.
- [x] S1: block deletion of referenced brands, models, and decks.
- [x] S1: make brand rename relationship-safe without changing identifiers.
- [x] S2: derive runtime counts from actual rows.
- [x] S1: allocate tape sequences from the maximum existing sequence.
- [x] S1: validate a complete bulk batch before inserting any row.
- [x] S1: assign every bulk copy an independent monotonic sequence.
- [x] S2: maintain correct per-model tape counters.
- [x] S2: clear both deck combo boxes before repopulation.
- [x] S1: recognize historical display-name `Models.Brand` references without a
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
- [x] Use short same-directory temporary filenames on classic Windows paths.
- [ ] Freeze the latest serviced VS2017 15.9, VS2022 17.14, and VS2026 stable
  installations in an immutable external source-bound lock immediately before
  candidate qualification; retain its SHA-256 in packages and evidence.
- [x] Finalize exactly three lane-specific config/manifest projections with
  identical application logic and resources.
- [x] Add deterministic portable packaging and exact payload verification.
- [x] Prove two clean, path-distinct portable builds produce identical ZIPs for
  Alpha 2.
- [x] Repeat two complete path-distinct builds for the exact Beta 1 application,
  setup, six-package, and assembled-distribution sources.
- [ ] Qualify XP SP3 x86/net40, Windows 7 SP1 x64/net48, and native Windows 11
  RTM ARM64/net481 on their target machines using exact retained package hashes.
- [ ] Complete all automated, manual, compatibility, and OS Beta gates.
- [ ] Resolve or reject every S0–S2 salvage-ledger entry with evidence.
- [x] Freeze the Beta-eligible source and retain the owner-authorized six
  Beta-labelled Candidate ZIPs under its exact source SHA.
- [ ] Create `v1.3.0b1` and advance `legacy/1.x` only after complete GO. Public
  publication remains a separate approval and the stable legacy feed remains
  unchanged throughout Beta.

## Beta 1 — bounded legacy usability; Alpha 4 layout proof superseded

- [x] Add a pure Brand/Model/Deck/Tape prerequisite planner in the original
  single WinForms EXE.
- [x] Give creation dialogs explicit OK/Cancel and created-key/display-name
  result contracts; remove their hidden main-form refresh/title mutations.
- [x] Carry stable brand codes, model identifiers, and deck names in combo
  choices instead of resolving display text.
- [x] Guide Add Tape through missing Brand and Model creation, and Add Model
  through missing Brand creation.
- [x] Preserve an in-progress tape while creating and selecting a new Model or
  Deck; treat cancellation as a normal stopped journey.
- [x] Replace routine creation exceptions with field validation and focus the
  first invalid control.
- [x] Add inline creation and empty-list/catalogue actions with one guided
  success summary at most.
- [x] Preserve the workflow intent for resizable forms, classic keyboard,
  mnemonic, tab-order, and accessibility behaviour. Alpha 4's runtime geometry
  implementation is superseded and reopens under Alpha 5 above.
- [x] Pass the historical planner, flow, identity, cancellation, draft, DPI
  policy, High Contrast policy, and keyboard-policy regressions. These do not
  substitute for Alpha 5's actual geometry/native evidence.
- [x] Review the single new workflow file and bounded form/control surfaces in
  the baseline-genome allow-list.
- [x] Retain the historical six Beta-labelled NO-GO Candidate ZIPs without
  overwriting `bbebac...`; they do not authorize or qualify post-Alpha-5 Beta
  bytes.

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
