# C3 preference format v1

Status: **Implemented internal persistence contract**

This directory specifies the C3-owned user preference profile shared by the
`win-x86-net40` and `win-x64-net48` builds. It is not a catalogue format and is
not intended for interchange between users.

Canonical files:

- `preferences.xsd` defines the structural v1 wire contract.
- `example.xml` is a privacy-safe canonical example loaded by characterization.

The live location is:

```text
%LOCALAPPDATA%\Jules Carboni\C3\2\preferences.xml
```

The writer emits UTF-8 without a byte-order mark, LF line endings, one ordered
element for each setting, `schemaVersion="1"`, and no namespace. Readers reject
unknown/duplicate fields, attributes on scalar fields, nested scalar markup,
DTDs, external resolution, oversized files, invalid XML characters, and invalid
enum/date values.

Semantic invariants not expressible in XSD 1.0:

- `legacy1xImportVersion="0"` requires outcome `pending`.
- version `1` requires `imported`, `not-found`, or `invalid`.
- a version newer than the reader supports is `UnsupportedVersion`, not corrupt.
- a recognizable future schema/namespace must remain byte-for-byte untouched by
  an older executable.
- `lastUpdateCheck` is empty for no prior check; persisted non-empty timestamps
  are normalized to UTC.

Change this contract only with reader/writer tests, upgrade and downgrade
preservation cases, documentation, and release-validation evidence.
