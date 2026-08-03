# Packaging and distribution

Portable ZIPs are C3's authoritative distribution. Both lanes consume the same
verified build outputs and packaging script:

```powershell
.\build\package.ps1
```

The command rebuilds both active lanes, verifies assembly/file/product identity
for every shipped binary, stages each payload under
`artifacts/staging`, writes a lane-specific `BUILD.txt`, creates ZIP files under
`artifacts/packages`, writes `SHA256SUMS.txt`, and reopens every ZIP to verify its
hash and exact entry set.

For a checkpoint candidate, prove that a rebuild does not change those bytes:

```powershell
.\build\verify-reproducible-packages.ps1
```

That gate requires a clean committed worktree, exports the exact `HEAD` tree into
two fresh source roots with different absolute paths, and runs the complete
Release rebuild/package transaction in each through Windows PowerShell 5.1. It
compares the name, length, and SHA-256 of both ZIPs and `SHA256SUMS.txt`, verifies
the retained copy, and leaves that proven second set in `artifacts/packages` for
candidate inspection. It also records the commit, MSBuild patch, PowerShell/CLR,
and host OS in the run output. Running `package.ps1 -SkipBuild` twice is not
equivalent evidence because it cannot detect compiler or source-path drift.

Packaged `README.md` and `RELEASE_NOTES.md` use repository-canonical UTF-8/LF
bytes. ZIP entry timestamps come from the release date, which must remain in the
ZIP format's 1980–2107 range. Package cleanup refuses reparse-point targets, and
verification rejects extra files as well as missing or changed files.

Each package contains only:

- `Compact Cassette Catalogue.exe`
- `Compact Cassette Catalogue.exe.config`
- `C3.Catalogue.dll`
- `C3.Infrastructure.dll`
- `BUILD.txt`
- `README.md`
- `RELEASE_NOTES.md`

PDB and XML compiler documentation files remain build artifacts and are not part
of the user payload. The ZIP entry timestamp is fixed from `build/Version.props`,
and entries are written in name order to reduce avoidable package drift.

The word portable currently means no installer and no administrator requirement.
It does not promise that preferences and diagnostics remain beside the
executable. Both lanes use
`%LOCALAPPDATA%\Jules Carboni\C3\2\preferences.xml`; diagnostics use their
documented application-data path. Do not add a `portable.mode` marker until an
explicit portable-profile design covers identity, migration, locking, backup,
privacy, and side-by-side behavior.

Preview package names include their channel stage, for example
`C3-v2.0.0-alpha.1-win-x86-net40-portable.zip`. Package availability is not
inferred from a generated development feed; only assets attached to a verified
GitHub release are published.

A future Universal Setup binding must consume the same staged payload and hashes.
Installed binaries must be byte-identical to the corresponding portable payload;
setup must not maintain a second file list or rebuild product binaries. See the
[integration boundary](../integrations/universal-setup.md).
