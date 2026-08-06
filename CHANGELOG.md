# Compact Cassette Catalogue


## C3 1.3 legacy maintenance


### Version 1.3.0 Alpha 5 - planned

Alpha 5 is the ratified Legacy Layout Stabilization checkpoint. It preserves
Alpha 4 as immutable discovery evidence and replaces its absolute/runtime-
anchor presentation mechanism with role-appropriate native WinForms layout.

- Defined pixel fidelity for the canonical Windows 7/96-DPI view, metric
  fidelity for other DPI/font profiles, and semantic fidelity across themes
  and operating systems.
- Froze an immutable 1.x visual inheritance contract and local copy of the
  repository demonstration image rather than following a moving `master` URL.
- Adopted one C3 96-DPI metric grid, native/system-colour presentation, one
  inherited form-root font, and consistent `AutoScaleMode.Font` policy.
- Required separate structural-reparenting and adaptive-behaviour commits for
  every mandatory form or form family.
- Selected `TableLayoutPanel`, `FlowLayoutPanel`, docking, scrolling panels,
  or `SplitContainer` according to window role instead of one universal
  container.
- Required Designer ownership of static commands and prohibited runtime fixed
  geometry in `Load`, `Shown`, and `Resize` handlers.
- Replaced property-only resize proof with fresh-process geometry, text,
  reachability, accessibility, performance, and native visual evidence.
- Added the first fresh-process STA geometry runner and retained the Alpha 4
  discovery baseline: all six mandatory workspace/browser cells reproduce the
  prohibited form-scroll/anchor mechanism, the main collision is measured,
  and 52 source-policy violations are closed to exact files and lines.
- Began the static-command migration by declaring the Brand New and Deck New
  Cancel commands in their Designers, preserving one-instance keyboard/default
  semantics while removing their runtime construction. A closed control-name
  allow-list now rejects unreviewed additions or any baseline control removal.
- Moved Model New's Add Brand/Cancel and Tape New's Add Model/Add Deck/Cancel
  commands into their Designers with compile-time event wiring. Removed their
  runtime rectangles, width/position edits, and layout-helper calls; the exact
  source-policy inventory fell from 52 findings in 10 files to 37 in 7 files.
- Declared Add Tape/Model/Brand/Deck once in the corresponding browser
  Designers, projected the former action-stack movement into inspectable
  design geometry, and removed the runtime button fields, rectangles,
  `AddHandler` calls, and child movement. Open source-policy findings fell to
  17 and are now limited to the remaining layout helpers, Main overlay, and
  Deck New form-scroll assignment.
- Declared the Main empty-catalogue panel, guidance, and Add First Tape command
  in `frmMain.Designer.vb`, retained its guided workflow through compile-time
  event wiring, and removed runtime construction, fixed rectangles, z-order
  mutation, and duplicated state fields. The closed control-name exception now
  covers exactly 14 additions with zero baseline removals; 133 characterization
  cases pass and the open source-policy inventory is down to 13 findings.
- Rebuilt the top-level Main workspace around Designer-owned table layouts: a
  menu/header/editor root, separate elastic header and command columns, an
  Identification/Scroll row, mutually exclusive editor/empty surfaces, and a
  dedicated scrollable preferred-width editor canvas. Existing data controls
  and group boxes are reparented unchanged; form-level `AutoScroll` and the
  runtime Main layout call are gone. The initial 32-cell Main intersection
  matrix passes while the separately recorded 800-pixel Find-field adaptation
  remains open.
- Made the Main Find and Identification interiors adaptive: the Find term,
  field selector, and command share a three-column table, while Short/Long
  identity fields stretch in a two-row table and the current/total counter
  remains grouped in a non-wrapping flow row. The 800-pixel view and 200%
  maximum-text view now retain every command and field without the Alpha 4
  collisions or off-canvas controls.
- Rebuilt `frmTapeNew` around a fill viewport and persistent bottom command
  region. A preferred-width three-column table owns metadata plus equal Side A
  and Side B groups; a metadata table stacks Model, Basic, Taped, Notes, and
  Bulk Add; and Add Deck/Add Tape/Cancel no longer move with the scrollable
  canvas. Form-level scrolling is disabled while the dedicated viewport owns
  both axes.
- Replaced the last fixed Model-row geometry in `frmTapeNew` with a local
  three-row table: the model choice stretches beside Add Model, Year and
  Length retain bounded numeric editors, and Region fills its row. Focused
  800-pixel and 200% maximum-text observations keep the fields ordered while
  Add Deck/Add Tape/Cancel remain owned by the non-scrolling command region.
- Began the shared entity-browser reconstruction with Brands: a fixed
  scrollable filter pane is separated from a flexible results pane, the
  ListView fills its result group, and result status plus the existing action
  group remain in a persistent footer. Form-level scrolling and the Brands
  runtime `ConfigureListForm` call are removed; tab indexes are unique.
- Applied the same closed browser structure to Models: the larger column set
  now expands with the result pane, filters stay in the fixed scrollable pane,
  status/actions persist below the list, action tab indexes are unique, and no
  behavior-time layout helper remains.
- Rebuilt Decks with the shared fixed filter/flexible results contract. Its
  21-column ListView now fills the available pane, status and actions persist
  below it, filter overflow belongs to the fixed pane, form-level scrolling is
  disabled, and its runtime layout helper and duplicate action tab order are
  gone.
- Completed Stage A of the browser family with Tapes. Its former 1924-pixel
  absolute canvas is now a practical 1200-pixel default SplitContainer;
  the deep filter stack scrolls only inside the fixed pane, the 18-column list
  fills the result pane, and status/actions persist below it. The last browser
  runtime layout call is removed and action tab order is unique.
- Completed the browser-family command-row pass. Brands, Models, Decks, and
  Tapes now keep their result status above a Designer-owned wrapping
  Add/Refresh/Edit/Delete flow, with logical minimum button sizes replacing
  inherited full-width rectangles. The geometry runner now populates maximum
  content without inventing invalid stable-key selections and fails when a
  command escapes its command bar.
- Retained the exact `cf2bae1...` browser-family atlas: all 128 fresh-process
  ordinary/maximum cells pass at four window sizes and 100/125/150/200%
  relative scales, with geometry JSON and screenshot hashes for every cell.
- Removed the now-unreferenced `CatalogueUx` module and its form-scroll,
  anchoring, rectangle, and runtime-control factories. A direct regression
  rejects its return; the layout source-policy inventory is reduced from the
  Alpha 4 baseline of 52 findings to the one open Deck New form-scroll setting.
- Rebuilt Deck New as a sizable dense editor with a dedicated scrolling
  preferred-size canvas and a persistent status/Add/Cancel row. All original
  groups and fields remain on the canvas, runtime size/scroll/accessibility
  mutations are removed from Load, and the layout source-policy gate is now
  clean in Qualification mode.
- Applied the same dense-editor contract to Deck Edit. Update and a new
  explicit Cancel result stay outside the scroll viewport; edit loading reads
  the supplied row directly while retaining its global index for persistence.
  The geometry harness now creates a complete representative Deck row and has
  an explicit System.Data dependency, so the actual Load path is characterized.
- Retained the exact `62bec3f...` Deck New/Edit atlas: all 64 fresh-process
  ordinary/maximum cells pass at four window sizes and 100/125/150/200%
  relative scales, including Deck Edit's real populated Load handler.
- Tracked the intermittent x64 process-without-window observation separately as
  `UI-START-001` with first-window milestone instrumentation.
- Implemented the original-simple 1.x source topology: application, installer,
  and uninstaller roots remain; the byte-identical 13-file setup engine now
  lives under Installer `Shared/`; all three consumers compile those same
  physical files, and a closed hash/path validator prohibits a partial 2.x
  `src/` migration or return of root `SetupShared/`.
- Kept release identity at Alpha 4 until implementation and qualification earn
  `1.3.0a5`; no new build, package, tag, publication, feed, `master`,
  `legacy/1.x`, or `dev/2.x` claim is made by this planning tranche.


### Version 1.3.0 Alpha 4 - 6 August 2026

Alpha 4 is the retained, intentionally unpublished owner-test preview of the
completed 1.x application and classic-setup work. It follows the untagged Alpha
3 implementation phase and the source-bound Beta Candidate NO-GO records; it
does not reinterpret missing historical or exact target-machine evidence.

- Projected exact `1.3.0a4 / Alpha 4 / v1.3.0a4` identity through the
  application, setup executables, manifests, package names, package content,
  validation, and evidence contracts.
- Preserved all completed lifecycle, persistence, integrity, settings,
  diagnostics, guided-creation, layout, accessibility, and classic-setup
  repairs with their 130 source regressions.
- Defined exactly three portable and three offline classic-setup Alpha test
  ZIPs from one frozen source commit and one external immutable toolchain lock.
- Required closed PE/framework/config/payload verification and two clean
  path-distinct source reproductions before tagging or retaining the test set.
- Kept public Alpha publication, Beta naming/tagging/publication, `master`,
  `dev/2.x`, `legacy/1.x`, the stable tag, and the public feed outside this
  checkpoint.
- Made owner acceptance and the remaining historical/target rows explicit
  inputs to the later human decision on `v1.3.0b1`.
- Retained Alpha 4 unchanged after owner testing found control overlap, broken
  resize relationships, and runtime widgets moving independently. Those
  observations define Alpha 5; they do not invalidate Alpha 4's source/build/
  package hashes or authorize rewriting its tag.


### Version 1.3.0 Beta 1 - 6 August 2026

Beta 1 is the retained, intentionally unpublished Candidate for the final
original VB.NET WinForms release. The untagged Alpha 3 implementation phase was
superseded by this single metadata/build/qualification wave.

- Projected exact `1.3.0b1 / Beta 1 / v1.3.0b1` identity through application,
  setup, manifests, package names, package content, validation, and evidence.
- Completed reproduction-backed source repairs for APP-001 through APP-015:
  lifecycle gates, bounded temporary loading, transactional persistence,
  external-edit detection, named tape mapping, referential integrity, atomic
  bulk creation, counters/sequences, settings migration, and diagnostics.
- Added 21 lifecycle/persistence, 8 integrity, and 6 settings/diagnostics
  regressions while retaining the original catalogue 1.1.0 format, project,
  forms, resources, and VB.NET production topology.
- Added a bounded legacy creation coordinator inside the original EXE: Add
  Tape now guides Brand -> Model -> Tape, Add Model creates a missing Brand,
  and recording-side controls open an owned Add Deck detour without discarding
  the active tape draft.
- Creation forms now return stable keys and display names, use keyed combo
  choices and field-level validation, suppress chained guided confirmations,
  and expose keyboard-reachable inline actions from the creation and list
  windows.
- Added a first-tape empty state and made the main, tape, and list windows
  resizable, font-scaled, scroll-safe, and accessible while preserving the
  classic WinForms surface and catalogue 1.1.0 schema.
- Expanded catalogue characterization to 31 cases and reviewed exactly one new
  production source file plus bounded designer changes through the genome
  allow-list.
- Added an authenticated, write-through ten-phase setup transaction journal,
  commit-last installed state, startup recovery/fail-closed behavior, and 30
  process-death phase cases within 85 setup regressions.
- Required exactly three portable and three classic-setup Candidate ZIPs, one
  source commit, one immutable external toolchain lock, one fetched-provider
  receipt, closed entry manifests, and two complete clean path-distinct builds.
- Added fail-closed component verdicts for `portableBetaGo`,
  `classicSetupBetaGo`, and `overallBetaGo`, plus source-bound retention under
  `artifacts/candidates/1.3.0b1/<source-sha>/` on NO-GO.
- Kept historical Gate 1 executions and exact XP, Windows 7, and native ARM64
  runtime/setup qualification as non-substitutable GO gates. No missing target
  result may be inferred from builder smoke or binary inspection.
- Preserved the owner boundary: Candidate bytes may be retained, but the tag
  and `legacy/1.x` advancement require complete GO; public release, `master`,
  `dev/2.x`, `VERSION`, stable tag, and stable feed changes remain prohibited.
- Archived the first `bbebac288f4996939124f882d0e9febcf2e5bdae`
  source-bound NO-GO verdict before restoring the active verdict template for
  the required UX-hardened Candidate; the old Candidate bytes remain immutable.
- Froze the UX-hardened package source at `989e2987…`, captured one fetched-ref
  external lock, rebuilt all nine application/setup executables, retained six
  authenticated Candidate ZIPs, and reproduced the complete distribution from
  two clean path-distinct checkouts. Historical Gate 1 and exact XP/Windows 7/
  native ARM64 target evidence remain missing, so evidence commit `92a6466`
  records NO-GO with no tag or ledger movement.


### Version 1.3.0 Alpha 3 - 5 August 2026

Alpha 3 is the active legacy reliability and classic setup revival checkpoint.
Its classic setup implementation passes source, shared-engine, six-binary, and
Preparation package controls; application repairs and exact target qualification
remain behind their focused gates.

- Ratified the 6 August Beta 1 Candidate authority: retained Beta-labelled
  bytes are permitted, while tagging and lease-protected `legacy/1.x`
  advancement remain conditional on complete GO; publication, `master`,
  `dev/2.x`, `VERSION`, and stable operations remain prohibited.
- Superseded the uncreated redundant Alpha 3 package/tag wave with one final
  Beta identity/build/qualification wave after the functional scope closes.
- Added a durable one-time settings-upgrade marker: failed migrations remain
  visible and retryable, known legacy update values normalize safely, and old
  profiles plus unrelated preferences remain untouched.
- Routed console exports through the configured directory with a Documents
  fallback and `Path.Combine`; write, console, message, browser-launch, and
  update-failure paths now remain nonfatal.
- Added net40 settings/diagnostics characterization covering the migration
  schema and fixtures, retry contract, export path, and guarded failure paths;
  real-profile and native target execution remain qualification work.

- Ratified the owner decision making the repaired classic VB.NET setup an
  optional secondary distribution while portable ZIPs remain authoritative.
- Defined offline, version-bound setup bundles for x86/net40, x64/net48, and
  native ARM64/net481.
- Specified closed payload and installed-state XML manifests, transactional
  install/repair/upgrade/rollback, and ownership-only uninstall.
- Added a strict offline seven-file payload verifier and a shared same-volume
  install/repair/upgrade transaction with verified rollback.
- Added ownership-manifest-driven removal that refuses modified owned files,
  preserves unknown content, and restores installed state after injected
  failures.
- Added native architecture, framework, elevation, running-process, Program
  Files, and rollback-space preflight shared by setup and uninstall.
- Added closed architecture-specific uninstall registration with collision,
  exact-ownership, removal, and rollback checks.
- Added transactional common Start Menu and optional desktop shortcut ownership
  with altered-link refusal and faulted-removal restoration.
- Composed payload files, installed state, registry, and shortcuts into one
  atomic clean-install/repair operation with cross-surface rollback.
- Composed manifest discovery, quarantine, exact system-state removal, unknown
  file preservation, and fault restoration into one reversible uninstall
  operation.
- Activated the original installer and uninstaller wizards against the offline
  transactional engine, including native/elevated preflight and exact
  self-relocation for ownership-only removal.
- Built and verified installer/uninstaller pairs as I386/net40, AMD64/net48,
  and native ARM64/net481 outputs with matching configs and zero runtime DLLs.
- Added deterministic setup ZIPs that reuse all five portable payload bytes,
  canonical seven-file XML manifests, authenticated entry evidence, and four
  rewritten-sidecar tamper regressions.
- Added a closed six-archive Alpha asset assembler without weakening the
  independently verified portable and setup package sets.
- Replaced five duplicate multi-megabyte form banner resources with the
  unchanged canonical project artwork and added keyboard/screen-reader wizard
  contracts without changing forms, controls, layout, or branding.
- Required preservation of catalogues, settings, unknown files, and unowned
  content.
- Added the S0–S3 defect ledger and static, differential, randomized, XML,
  persistence-fault, and lifecycle test programmes.
- Kept Universal Setup as a future consumer of the same contracts rather than
  a blocker for the completed historical implementation.
- Preserved the standing authority for retained unpublished Alpha artifacts and
  the explicit-human-approval boundary for every Beta-labelled operation.
- Reconstructed the exact Microsoft Build Tools 2015 Update 3 x86/amd64
  administrative payload as an isolated, hash-recorded compatibility-lab
  toolset without registering the retired product system-wide.
- Rebuilt immutable `v1.2.0b1` in x86/net40 and x64/net40 with exact MSBuild
  `14.0.25420.1`, verified both PE/framework contracts, and retained honest
  non-byte-identical comparisons with the official runtime-oracle binaries.
- Retrieved the exact official `v1.1.2` oracle and reproduced a deterministic
  three-oracle/canonical-catalogue Gate 1 runtime kit with a closed operator
  workflow and nine-cell exchange record.
- Added a PowerShell 2-compatible target setup verifier that authenticates the
  retained setup ZIP, all ten extracted entries, bound portable/source/lock
  identity, and the mechanically derived lane environment before an explicit
  wizard launch; added a closed real-machine mutation and accessibility record.
- Kept historical Gate 1 open for target workflows, private catalogue exchange,
  screenshots, and control/resource evidence before application behaviour work.


### Version 1.3.0 Alpha 2 - 5 August 2026

Alpha 2 is the completed, intentionally unpublished three-lane build and
release-control checkpoint. Its annotated tag, retained packages, immutable
lock, clean-source reproduction, and post-tag record passed without public
publication, feed change, or legacy movement.

- Added a closed stage-aware manifest identity for `1.3.0a2`, `Alpha 2`,
  `v1.3.0a2`, alpha channel, and retained-unpublished status.
- Kept CLR assembly and file versions at `1.3.0.0` while projecting `1.3.0a2`
  through assembly informational/product metadata.
- Renamed the active package authority to the exact three
  `C3-v1.3.0a2-...-portable.zip` assets so Alpha production cannot emit
  stable-looking filenames.
- Bound the release stage through `BUILD.txt`, authenticated entry manifests,
  checksum filenames, PowerShell 2 runtime projection, and target evidence.
- Added cross-lane enforcement for one release label, source commit, toolchain
  mode, lock status, and external-lock SHA-256.
- Enforced internally complete Alpha/Beta/stable label, ordinal, stage, and
  channel relationships, including a fully consistent stable-looking Alpha
  negative test.
- Added exact annotated-tag message requirements and a post-tag verifier using
  `C → E → tag → P`, so the immutable tag stays at `E` while `P` records its
  now-existing local/remote object and target without self-reference.
- Added a machine-readable qualification record at `E` covering package,
  entry-manifest, checksum, Candidate closure, source-rebuild, toolchain
  evidence, and binary-log hashes; `P` must match those frozen values.
- Replaced the Alpha packaging-only reproducibility call with two clean,
  path-distinct Candidate source rebuilds that compare authoritative build,
  package, checksum, and entry-manifest bytes and retain both build logs.
- Documented the full Alpha 2 source/lock/Candidate/reproducibility/smoke/tag
  gate and its retained-but-not-public authority boundary.
- Kept historical Gate 1, runtime repairs, target-OS qualification, ARM64
  execution, public release, legacy-feed promotion, and `legacy/1.x` movement
  explicitly open.


### Post-Alpha stable-matrix decision - 5 August 2026

- Ratified exactly three C3 1.3 Beta/stable release lanes:
  `win-x86-net40`, `win-x64-net48`, and native `win-arm64-net481`.
- Retained the immutable Alpha tag and its four-lane diagnostic result as
  truthful historical evidence while superseding that matrix for future
  candidates.
- Assigned VS2017/MSBuild 15 to the 1.3 x86/net40 lane, VS2022/MSBuild 17 to
  x64/net48, and VS2026/MSBuild 18 to native ARM64/net481.
- Kept VS2015/MSBuild 14 solely as the 1.2 historical reconstruction oracle.
- Removed new x64/net40 and x64/net481 packages from the 1.3 publication plan.
- Restricted C3 1.3 publication to three portable classic WinForms ZIPs and a
  checksum manifest; installers and Universal Setup are independent later work.
- Made native ARM64 output and actual Windows-on-ARM qualification a
  release-blocking boundary.
- Replaced the release manifest with the exact three package contracts and
  added an unfrozen candidate toolchain lock.
- Added exact VS15/VS17/VS18 family resolution, effective-tools-version
  enforcement, forced Roslyn/reference paths, binary logs, and hashed evidence.
- Added ARM64 project/solution configurations without application-source
  branching and produced verified `0xaa64`/PE32+ output.
- Extended binary proof to CLR CorFlags, versions, matching runtime config,
  settings parity, and zero runtime DLLs.
- Added deterministic five-file portable ZIP creation, exact checksum/payload
  verification, and mechanical exclusion of installer/updater artifacts.
- Split builder smoke from exact-hash target-machine proof and added a durable
  XP/Windows 7/Windows-on-ARM qualification template.
- Passed the post-correction preparation suite with an unchanged baseline
  genome and recorded the remaining Beta/stable gates.


### Post-Alpha release-control hardening - 5 August 2026

- Replaced the structurally self-referential tracked candidate-lock design with
  an immutable external lock bound to a clean source commit and exact remote ref.
- Made Candidate mode reject tracked, staged, untracked, submodule-drifted, or
  remote-mismatched source before compilation, and require clean-intermediate
  rebuilds.
- Added a lock-capture command that refuses stale servicing baselines,
  repository-local output, overwrite, stale build evidence, and unpushed source.
- Forced and hash-froze the actual `ResGen.exe` used by every lane, with MSBuild
  property proof, binary logs, before/after stability checks, and package
  provenance.
- Added retained SHA-256 package-entry manifests and made target verification
  authenticate every extracted file independently of extracted `BUILD.txt`.
- Made all target-side verification execute under actual Windows PowerShell 2
  and removed ordinary-script reliance on `$PSScriptRoot` or modern JSON APIs.
- Replaced caller-asserted target labels with mechanically derived OS build,
  service pack, native architecture, and installed Full Framework checks.
- Added adversarial regression coverage for wrong locks, dirty source, altered
  extraction, environment spoofing, wrong target facts, and stale builders.
- Kept this work classified as preparation: current local Visual Studio
  installations remain below the decision-date floors and cannot be frozen as
  candidate authority.
- Required one source/mode/status/lock identity across the complete three-package
  set, with independent package-verifier enforcement and negative tests.
- Made lock capture fetch and retain the provider ref, commit, remote URL, and
  timestamp before allowing subsequent offline Candidate builds.
- Added final source/ref/submodule/genome/lock closure after all Candidate lanes
  and required it before Candidate packaging.
- Recorded the owner authorization allowing deliberate Alpha tags and retained
  Alpha distributions while requiring explicit human approval for every Beta
  tag, Beta-labelled distribution, or Beta publication.
- Corrected the VS2022 decision-date servicing floor to 17.14.37 after official
  release history confirmed its 22 July 2026 release; no lock or Candidate build
  had been accepted under the earlier 17.14.36 floor.
- Added a pinned historical-toolchain manifest and fail-closed download verifier
  for the Microsoft-signed Build Tools 2015 Update 3 `14.0.25420.1`
  compatibility-laboratory bootstrapper.


### Version 1.3.0 Alpha 1 - 5 August 2026

Alpha 1 is an intentionally unpublished source checkpoint for the recovered
legacy maintenance programme. It does not contain the planned runtime repairs
and is not advertised to existing C3 users.

- Reconstructed `dev/1.x` from safe post-release tip `58a5b7d...`, retaining all
  eleven documentation-only commits after the manually qualified `v1.2.0b1`
  tag instead of starting from the broad unpublished 1.2.1-era refactor.
- Distinguished exact package source `509c9ec...`, qualified release checkpoint
  `2413e913...`, and development baseline `58a5b7d...`; proved that their
  production trees are identical.
- Preserved the superseded refactor tip exactly under annotated archival tag
  `archive/1x-refactor-attempt-2026-08-03`.
- Defined `legacy/1.x` as the qualified checkpoint ledger: initially the release
  tag, then the evidence-only qualified 58a-derived reconstruction checkpoint.
- Defined the final 1.x maintenance constitution: original VB.NET WinForms
  application, forms, filenames, DataSet model, settings, resources, assembly
  identity, and catalogue format 1.1.0 remain authoritative.
- Added a severity-classified salvage ledger for lifecycle, persistence, tape,
  referential, counter, settings, and diagnostic repair candidates.
- Added explicit Alpha 1, Beta 1, and stable milestone plans, including entry,
  exit, publication, rollback, and post-1.3 freeze rules.
- Replaced the feature-oriented TODO with a gated legacy maintenance workboard.
- Added the catalogue 1.1.0 prose specification and XSD.
- Added valid, invalid, culture, external-entity, and five historical catalogue
  fixtures.
- Added four captured historical settings profiles.
- Added a strict compatibility corpus covering ten public 1.x releases, five
  supported producers, four observed catalogue formats, exact source/release
  provenance, and SHA-256 identities.
- Added standalone .NET Framework 4.0 VB characterization tests for catalogue
  shape, version handling, malformed XML, entity rejection, and invariant
  decimals.
- Added a generated baseline-genome manifest covering production paths, VB type
  declarations, designer controls, resource keys/hashes, application identity,
  framework references, settings, DataTables, catalogue/update constants, and
  principal assets.
- Added an explicit genome allow-list contract; Alpha 1 requires it to remain
  empty.
- Added one-project, source-identical build lanes for x86/net40, x64/net40,
  x64/net48, and x64/net481.
- Added lane-specific AppConfig metadata without conditional application logic.
- Added PE architecture, target-framework, settings-parity, and no-runtime-DLL
  checks.
- Added reproducible download and exact verification of the official
  `v1.2.0b1` x86 and x64 executables.
- Kept the root three-line update feed at the actually available 1.2.0 release.
- Recorded that VS2017 fallback builds are diagnostic only and do not replace
  the pending authoritative VS2015/MSBuild 14 net40 gate.
- Made GUI smoke report the known recursive `Closing`/`Application.Exit` defect
  honestly: every lane must launch, normal-close timeouts are bounded and
  cleaned up, and the lifecycle repair remains assigned to Beta 1.
- Preserved the superseded Alpha tag object and original reconstruction lineage
  under explicit archive tags before correcting the public development lineage.

Known runtime data-safety and integrity defects remain assigned to Beta 1. No
1.3 Alpha binary package is published from this checkpoint.



## Releases



### Version 1.2 Beta 1 - 14 May 2026

- Retargeted the main application from .NET Framework 4.6 to .NET Framework 4.0.
- Added an explicit x86 compatibility build for Windows XP SP3 and newer.
- Added explicit x64 build configurations for 64-bit Windows release assets.
- Re-aligned the main project for Visual Studio 2015 / MSBuild 14 validation.
- Removed unused framework references and imports that blocked the .NET 4.0 compatibility path.
- Changed the application to be offline-first by default.
- Disabled automatic update checking by default to avoid startup network failures on old Windows systems.
- Fixed update-check scheduling logic for weekly and monthly checks.
- Fixed update-check settings so stored values and the settings UI use startup, weekly, monthly, and never consistently.
- Migrated the old manually update setting to never while keeping manual update checks available from the menu.
- Improved update-check failure handling so network and TLS failures do not show modal errors during startup.
- Added a manual update-check failure path that offers to open the releases page in the browser.
- Added best-effort TLS 1.1 / TLS 1.2 enablement for update checks without claiming guaranteed old-Windows TLS support.
- Guarded browser-opening links so link launch failures do not crash the app.
- Updated system requirements to Windows XP SP3 or newer with .NET Framework 4.0.
- Updated release metadata, README, VERSION, and source version constants for 1.2.0.
- Kept the catalogue file format version at 1.1.0.
- The portable x86 build is the official Windows XP SP3 compatibility path; x64 builds are for 64-bit Windows only and XP x64 support is unverified unless separately tested.
- The old network installer is not the recommended XP path unless separately tested.



### Version 1.1.2 – 22 April 2020

- Can choose to check for updates on startup, weekly, monthly, or never (manually).



### Version 1.1.1 – 22 April 2020

- Up-to-date notification doesn't show anymore upon start-up.



### Version 1.1 – 19 April 2020

- Bulk add multiple copies of a new tape to a catalogue.
- Automatically and manually check for updates.
- Submit feedback (bug reports or feature requests).
- Automatic updates disableable via settings.
- Redisigned about program information form.



### Version 1.0 – 20 December 2019

- Can view all tapes, models, brands and decks in list views.
- Can filter results in list views.
- Can select a result in list to make changes to.
- Can select result(s) in list to delete.
- Added a basic settings form with minimal functionality.
- Disabled access to incomplete functions.
- Successful console output now shows confirmation message.
- Only warning messages have sounds now.
- Fixed bugs.



## Betas



### Beta 0.6.2 – 7 December 2019

- Rebuilt UI to fix element alignment.
- Added console output header.
- Fixed tape updates detection bug.



### Beta 0.6.1 – 4 December 2019

- Can now write console output to a log file.
- Improved console *help* command.
- Added *close* and *kill* commands.
- Bug fixes.



### Beta 0.6 – 2 December 2019

- Can now update information of existing tapes.
- Can now delete existing tapes.
- File version checker now supports *'x.x.xbx'* version numbers.
- .NET Framework changed to version 4.6.
  - Added Windows Vista support.



### Beta 0.5.4 – 26 October 2019

- Fixed bug where program reads wrong combination box selections (specifically models).



### Beta 0.5.3 – 26 October 2019

- Cannot add new recording to existing tape if no decks have been added.
- When adding new recordings to existing tapes, default values are now loaded.
- Combination boxes are sorted alphabetically (brands and models only).



### Beta 0.5.2 – 25 October 2019

- If no recording on a side, the objects now display their defaults (not user definable).
- Bug fixes.



### Beta 0.5.1 – 25 October 2019

- File version updated to *1.1.0* (see below for changes).
- Modification date updated at time of save.



### Beta 0.5 – 25 October 2019

- Added ability to load saved catalogues (file format version *1.0.2* only).
- Optimised console output function (included date and time with logs).
- Loading now writes updated program data to the file.
- Loading files checks against list of supported file versions.
- Disabled enter-to-accept functionality where multi-line notes can be written (temporarily).



### Beta 0.4.3 – 19 October 2019

- Added application icon.
- Updated default values for new decks.
- Added tutorial and feedback buttons (does nothing).
- Added default filename to Save As dialog.
- Included copyright symbol.
- Modified assembly information.
- Form name changes.
- Other minor changes.



### Beta 0.4.2 – 19 October 2019

- Added keyboard shortcuts to menu.



### Beta 0.4.1 – 19 October 2019

- Title bar now displays file name and save status.



### Beta 0.4 – 18 October 2019

- Ability to add a new catalogue (restarts program).
- Added save functionality.
- Added save-as functionality.
- Partially added checks for unsaved changes to tapes.
- Added new settings to console.
- Added query functionality to settings command.
- Fixed help command formatting.
- Bug fixes.



### Beta 0.3 – 5 October 2019

- Date and time is now stored with each new item added.
- Added help command to console.
- Menu bar restructure.
- Set up code for save/load functionality.
- Other minor changes.
- Short identifier is now the primary key for tapes.



### Beta 0.2 – 4 October 2019

- Added rudimentary console to avoid too many confirmation dialogs.
- All fields reset when toggling a new tape's sides.
- New tape's number is now non-editable.
- Program now records version's release time of day.



### Beta 0.1 – 3 October 2019

- Big restructure — a crap tonne of reengineering.
- Moved new tapes to separate form.
- Major additions to functionality.
- Added Global Variables module.
- Added code to store entered data.
- Tape numbers are automatically assigned.
- Cannot change a tape's model/year/length/region once it has been added.
- Restructured data record format.
- Many bug fixes.



## Alphas



### Alpha 10 – 5 September 2019

- Added file open/close dialogs.
- Added input and peak level options when adding new tape.
- Minor changes.



### Alpha 9 – 2 September 2019

- Added add new model form.



### Alpha 8 – 2 September 2019

- Added add new brand form.
- Small changes.
- Added more dummy code.



### Alpha 7 – 2 September 2019

- Added add new deck form.



### Alpha 6 – 31 August 2019

- Added view decks form.
- Fixed tab indexes.



### Alpha 5 – 30 August 2019

- Added view statistics form.



### Alpha 4 – 30 August 2019

- Changed main form layout.
- Added tape decks.
- Added more options to tape data.



### Alpha 3 – 30 August 2019

- Created View Models and View Brands forms.



### Alpha 2 – 29 August 2019

- Created View Tapes form.
- Included basic form opening code.



### Alpha 1 – 22 August 2019

- Created main form.
- Basic version code and about information.





# Catalogue File Format



### Version 1.1 – 25 October 2019

- Data tables now have names.
- New date-times stored:
  - Creation date-time.
  - Modification/save date-time.
  - File format update date-time.



### Version 1.0.2 – 5 October 2019

- Removed indexes as identifiers.
- Short identifier is now primary key for tapes.
- Date and time stored with each new item added.



### Version 1.0.1 – 4 October 2019

- Included time of day in stored dates.



### Version 1.0 – 3 October 2019

- Stores decks, brands, models, and tapes.
