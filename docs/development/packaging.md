# Packaging and distribution

Portable ZIPs are C3's authoritative distribution. Both lanes consume the same
verified build outputs and packaging script:

```powershell
.\build\package.ps1
```

The command rebuilds both active lanes, stages each payload under
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
It does not yet promise that settings and diagnostics remain beside the
executable; those continue to use the current user's application-data folders.
Do not add a `portable.mode` marker until the settings provider honors it.

A future setup integration must consume the same staged payload and hashes. It
must not maintain a second file list or rebuild product binaries.

