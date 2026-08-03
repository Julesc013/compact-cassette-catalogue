# Releasing C3

A release is an evidence-backed promotion of one commit, not a rebuild performed
while drafting GitHub release text.

## Prepare

1. Choose the version and stage in `build/Version.props`.
2. Run `build/sync-version.ps1` and commit every generated projection.
3. Update `CHANGELOG.md`, `RELEASE_NOTES.md`, and the versioned validation file.
4. Ensure the working tree is clean and tag the exact candidate under test.

## Verify

Run the full compatibility gate on the Visual Studio 2019 runner, then complete
the manual workflows and minimum-OS checks in
[testing.md](testing.md). Record evidence against the full commit SHA.

Any failed or unverified required check leaves the release blocked. Do not turn
an unverified minimum-OS target into a support claim through wording alone.

## Package

```powershell
.\build\package.ps1 -SkipBuild
```

Do not modify staged files after packaging. The script creates deterministic
lane ZIPs and `SHA256SUMS.txt`, then verifies exact contents and hashes.

## Publish and verify again

1. Create one GitHub release for both build lanes.
2. Upload both ZIPs and `SHA256SUMS.txt` without renaming them.
3. Mark prerelease stages as GitHub prereleases.
4. Download every published asset into a clean directory.
5. verify downloaded SHA-256 values and launch the downloaded builds; and
6. add release URL, final hashes, and download verification to the validation
   record.

An installer may later consume the verified portable payload. It may not rebuild
the product or maintain a second authoritative file list.
