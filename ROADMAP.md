# C3 Roadmap

The roadmap records outcomes, not promises or duplicate implementation notes.
Release scope is accepted only when catalogue compatibility and both build lanes
remain verifiable. Concrete defects belong in GitHub Issues.

## 1.2.1 Beta stabilization

- Complete manual New/Open/Save/Save As/Edit/Delete/Close smoke workflows in
  both build lanes.
- Record Windows XP SP3 x86 and Windows 7 SP1 x64 runtime evidence on the exact
  release candidate, or clearly mark any unverified claim.
- Finish replacing the main editor's remaining read-only `DataRow` projection
  with typed tape views.
- Put settings behind a typed adapter and test persistence across restart.
- Exercise invalid, externally modified, and interrupted catalogue saves through
  the UI as well as the store-level tests.
- Review accessibility, keyboard navigation, scaling, clipping, and contrast on
  representative 100%, 125%, 150%, and 200% display settings.
- Complete user documentation and downloadable-hash verification.

## 1.3 usability and interoperability

- Resizable layouts and deliberate high-DPI behavior in the .NET 4.8 lane,
  while retaining a conservative XP-safe layout.
- Sortable and persistent list columns, clearer empty states, and task-oriented
  search/filter presets.
- Import and export selected records through documented, versioned profiles.
- Optional starter data packs that never change the catalogue format silently.
- A privacy-reviewed diagnostic support bundle with explicit user consent.
- Localization-ready resources and removal of remaining hard-coded UI strings.

## Future format evolution

A format after 1.1.0 may add stable opaque identifiers, attachments, richer
metadata, or alternative serializations. It requires:

- a new language-neutral specification and schema;
- forward and backward compatibility rules;
- deterministic migration with backups and rollback;
- golden fixtures for every supported source version; and
- independent reader/writer conformance tests.

Brand/model/deck/tape keys and the 1.1.0 XML shape will not be silently rewritten
inside a maintenance release.

## Extensibility direction

Prefer declarative, versioned extensions such as data packs, export profiles,
column presets, and validation profiles. C3 1.x will not load arbitrary
in-process plugin assemblies. If executable extensions are introduced, they need
a versioned contract, capability boundaries, and process isolation.

## Deliberate non-goals for the current release

- automatic self-updating;
- an official installer independent from the verified portable payload;
- HTTP downgrade or TLS security bypasses;
- divergent catalogue formats between x86 and x64; and
- a whole-application rewrite during stabilization.
