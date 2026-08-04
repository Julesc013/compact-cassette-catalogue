# Compact Cassette Catalogue



## Releases

### Version 2.0.0 Alpha 3 - In development

- Made hosted tag-event checkout preserve annotated tag objects by checking out
  the immutable event commit explicitly, and added a repository-owned workflow
  contract that rejects regressions or unpinned third-party actions.
- Added the dependency-free .NET 4.0/C# 7.3 `C3.Domain` substrate with opaque
  aggregate-typed identifiers, deterministic and production generators, strict
  UTC/optional values, stable validation issues, command contexts/results,
  versioned change sets, and explicit undo-command semantics.
- Accepted the native aggregate vocabulary, including separate `DeckModel` and
  physical `DeckUnit` identities, tape-owned recordings, and legacy-key mapping
  as an explicit migration responsibility rather than domain identity.
- Froze all 269 exported signatures of the current VB catalogue library as a
  deterministic reflection oracle and made unexplained API drift fail the
  characterization gate before behavior-preserving C# ports begin.
- Added the first mechanical C# catalogue candidate—opaque persisted revision
  identity—and differential characterization against the still-authoritative VB
  implementation before changing production ownership.
- Promoted the proven C# revision identity to the sole behavior owner while
  retaining a logic-free VB compatibility facade, and made `C3.Domain.dll` an
  explicit version- and package-verified runtime payload in both lanes.


### Version 2.0.0 Alpha 2 - Qualified, intentionally unpublished (2026-08-04)

- Began the executable C3 1.x compatibility laboratory: public-release
  inventory, provenance-bearing fixtures, supported-baseline classification,
  old/new reader-writer evidence, settings migration, and update-channel dry
  runs.
- Added a strict compatibility-corpus schema and validator covering all 10
  public 1.x tags, exact commits/releases/assets, settings and updater profiles,
  supported-vs-inventory-only policy, and the pre-1.x provenance of catalogue
  formats 1.0.0, 1.0.1, and 1.0.2.
- Added deterministic privacy-safe fixtures generated from supported official
  writer schemas and proved that the secure production adapter loads every
  supported producer profile.
- Added an opt-in hash-pinned historical-binary laboratory that proves six
  current-writer/old-reader/old-writer/current-reader rows, including separate
  1.2 Beta 1 x86 and x64 artifacts, without redistributing or launching the old
  applications.
- Defined the exact maintained 1.x producer baseline and classified required
  behavior, tolerated identity/date quirks, corrected defects, and manual/OS
  evidence that remains pending before public Beta.
- Adopted the permanent `master`, `dev/2.x`, `legacy/1.x`, and `dev/1.x` branch
  contract with one schema-validated machine-readable owner and generated strict
  updater projections.
- Replaced the unavailable named runner-group assumption with a provider-neutral
  trusted-runner capability contract and an isolated ephemeral fallback for
  intentionally unpublished Alpha qualification.




### Version 2.0.0 Alpha 1 - Qualified, intentionally unpublished (2026-08-04)

- Reclassified the unpublished 1.2.1 overhaul as the C3 2.0 programme before
  publication; no 1.2.1 release or artifact was relabelled.
- Separated current build identity from the root 1.x update feed so legacy users
  cannot be offered an unavailable 2.0 preview.
- Kept catalogue writer version 1.1.0 independent from the 2.0 product version.
- Replaced path- and CPU-scoped `My.Settings` with one versioned C3-owned profile
  shared by both build lanes, using cross-process locking, dirty-field merging,
  verified atomic writes, durable flush, backups, quarantine, recovery, and
  downgrade-safe future-schema refusal.
- Centralized create-only sibling temporary files and compact recovery names so
  catalogue and preference transactions remain usable near the classic Windows
  path limit without weakening same-directory atomic replacement or cleanup.
- Added a bounded read-only importer for known C3 1.x profile locations and
  Boolean/string schemas. It preserves source bytes, records imported/not-found/
  invalid outcomes atomically, falls back only from invalid content, and keeps
  discovery/access/checkpoint failures retryable.
- Replaced numeric-only 2.x update discovery with a bounded, closed-schema JSON
  manifest. Unpublished manifests cannot advertise assets; published manifests
  must identify the exact tagged release, checksum file, portable packages,
  byte lengths, lowercase SHA-256 hashes, and canonical GitHub asset URLs.
- Added complete SemVer precedence for alpha, beta, release-candidate, stable,
  and build-metadata identities while keeping the three-line `VERSION` API
  isolated to existing 1.x clients.
- Added a machine-readable release catalogue and exact `C -> E(tag) -> P`
  transaction gates for qualification, atomic promotion, immutable tags,
  publication, post-download verification, feed promotion, and honest failure.
- Split development, candidate, post-promotion, and tag verification into
  distinct release-gate contexts so a lighter development run cannot satisfy a
  checkpoint gate.
- Pinned every third-party workflow action to a reviewed commit, disabled
  checkout credential persistence, and required restricted GitHub environments
  plus a private legacy-toolchain runner before self-hosted evidence is trusted.
- Added trusted-master topology guards, create-only SHA-bound attestation refs,
  exact-old-object leased atomic promotion, and raw annotated-tag identity checks
  so target scripts, ref races, or same-commit tag replacement cannot self-attest.

- Reorganized C3 as a four-project modular monolith: catalogue rules,
  infrastructure adapters, characterization tests, and shared WinForms sources.
- Added distinct x86/.NET Framework 4.0 and x64/.NET Framework 4.8 project lanes
  with common feature files, manifests, resources, and catalogue behavior.
- Retired obsolete installer/uninstaller backup projects and repaired the
  solution so every referenced project exists and builds.
- Established canonical version/build-lane manifests, synchronized projections,
  deterministic portable packages, SHA-256 manifests, and PE checks.
- Added a language-neutral catalogue 1.1.0 specification plus valid, invalid,
  culture, and XML-security fixtures.
- Added secure temporary-state loading, verified temporary-file saving, atomic
  replacement with backup, and external-edit conflict detection.
- Rejected namespaced legacy catalogue rows/fields and nested scalar markup
  instead of allowing `DataSet.ReadXml` to silently normalize nested content.
- Added byte-exact persistence evidence for overwrite backups, external revision
  conflicts, missing destinations, owned temporary-file cleanup, duplicate
  keys/version rows, invalid structure, and culture-independent round trips.
- Added typed catalogue session ownership for path, display name, revision,
  dirty state, and document lifecycle.
- Added bounded diagnostics, action breadcrumbs, and unhandled-exception report
  generation.
- Moved brand, cassette-model, deck, and tape validation and mutation behavior
  into typed catalogue services with sole legacy `DataSet` repository adapters.
- Prevented deletion of brands, models, and decks still referenced by dependent
  records.
- Made bulk tape insertion atomic, fixed per-tape sequence storage, synchronized
  counters from actual rows, and prevented deleted identifiers being reused.
- Removed public `DataRow` hand-offs between feature forms and replaced default
  form instances with explicit form ownership and a single UI mutation seam.
- Removed all remaining form-level row/table access; typed services now own tape
  rendering, selection, counts, and mutations, while metadata writes stay in the
  versioned XML adapter.
- Preserved legacy cassette-model brand references stored as display names,
  including deletion protection and migration to stable codes during rename.
- Removed generated `My.Settings`, automatic settings-on-exit behavior, and the
  superseded upgrade coordinator; the boundary gate now rejects reintroduction.
- Moved update-check parsing, serialization, UTC normalization, clock-skew safety,
  and scheduling into a tested policy.
- Fixed Save As cancellation continuing into save, a duplicate Open dialog,
  deck deletion writing the tape count, model notes loading from the wrong table,
  duplicate deck combo entries, recursive application shutdown, unapplied tape
  edits being lost on close, and console-export path failures terminating C3.
- Expanded executable characterization to preference format, import, security,
  recovery, retry, concurrency, and compatibility contracts; retained dependency
  direction, WinForms-boundary enforcement, shared-project parity validation,
  hosted repository checks, and an authoritative legacy-capable self-hosted
  compatibility workflow.



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
