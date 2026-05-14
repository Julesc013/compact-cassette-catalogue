# C3 1.2.0 Release Validation

Validation date: 14 May 2026

Validated commit:

```text
509c9ec29679e30dcdcb1f57d8874b850cee310c
Harden C3 1.2.0 portable release builds
```

## Build Results

| Check | Result |
| --- | --- |
| Main app `Release|x86` with VS2015/MSBuild 14.0.25420.1 | Passed |
| Main app `Release|x64` with VS2015/MSBuild 14.0.25420.1 | Passed |
| Installer `Release|AnyCPU` with VS2015/MSBuild 14.0.25420.1 | Passed |
| Uninstaller `Release|AnyCPU` with VS2015/MSBuild 14.0.25420.1 | Passed |
| `git diff --check` | Passed |

## PE Architecture Verification

| Artifact | Machine | PE kind | CorFlags | Result |
| --- | --- | --- | --- | --- |
| `Compact Cassette Catalogue\bin\x86\Release\Compact Cassette Catalogue.exe` | I386 | PE32 | `0x3` | 32-bit required |
| `Compact Cassette Catalogue\bin\x64\Release\Compact Cassette Catalogue.exe` | AMD64 | PE32+ | `0x1` | 64-bit |

## Launch Smoke

| Artifact | Result |
| --- | --- |
| x86 Release executable on modern Windows | Started, accepted close-window, exited without force |
| x64 Release executable on modern Windows | Started, accepted close-window, exited without force |

## Local Release Assets

The generated files are local release artifacts under `Releases\1.2.0`. They are intentionally ignored by git and are not committed.

```text
C3-v1.2.0-win-x64.exe
C3-v1.2.0-win-x64.exe.config
C3-v1.2.0-win-x64-portable.zip
C3-v1.2.0-win-x86.exe
C3-v1.2.0-win-x86.exe.config
C3-v1.2.0-win-x86-portable.zip
SHA256SUMS.txt
```

## SHA256

```text
257ec9d0ea86f268d8328d71041e63eb379fc1809c91593db29d883359db747c  C3-v1.2.0-win-x64.exe
5a12662ef1157d2ca4ca2f94e084c4bc9c7a72927d744d2419ecce8b21e8fb6e  C3-v1.2.0-win-x64.exe.config
9eb5ad6dc0deda0d20093f7b7768ceb7b7de11ed97d3ef071b6a673c68893d0e  C3-v1.2.0-win-x64-portable.zip
205ba251175d5a6fa20a3ace6127a00e5d10d73ad30581032c8f09b20ceb7222  C3-v1.2.0-win-x86.exe
5a12662ef1157d2ca4ca2f94e084c4bc9c7a72927d744d2419ecce8b21e8fb6e  C3-v1.2.0-win-x86.exe.config
cae5c5805e9e7d375cf94dbf1b9d6478f10426b69a32dc79ce062d3c2789f2ce  C3-v1.2.0-win-x86-portable.zip
```

The local `SHA256SUMS.txt` file was checked against the generated files and all hashes matched.

## Manual Test Gates

These checks are still release gates and must be completed before publishing a runtime-verified release.

| Check | Status |
| --- | --- |
| Full interactive x86 workflow test | Not run |
| Real catalogue save/reopen regression test | Not run |
| List views/filter/edit/delete regression test | Not run |
| Settings persistence across app restart | Not run |
| Manual update-check failure path with network blocked | Not run |
| Windows XP SP3 32-bit VM runtime test with .NET Framework 4.0 | Not run |
| GitHub release page upload verification | Not run |

## Release Boundary

- The x86 portable build is the Windows XP SP3 compatibility artifact.
- The x64 portable build is for 64-bit Windows only.
- Windows XP x64 support is unverified unless tested separately.
- The old network installer is not the official Windows XP release path.
