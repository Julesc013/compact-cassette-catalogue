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
