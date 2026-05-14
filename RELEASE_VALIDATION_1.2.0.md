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
169b02f9b2e0637422c52459f9512ed484bc63e67b3f90b4bd6c754daee75909  C3-v1.2.0-win-x64-portable.zip
205ba251175d5a6fa20a3ace6127a00e5d10d73ad30581032c8f09b20ceb7222  C3-v1.2.0-win-x86.exe
5a12662ef1157d2ca4ca2f94e084c4bc9c7a72927d744d2419ecce8b21e8fb6e  C3-v1.2.0-win-x86.exe.config
d9283e0e9243dd518b810869bfdae0007aa686e9394e709abd9834fd1b6dfeb7  C3-v1.2.0-win-x86-portable.zip
```

The local `SHA256SUMS.txt` file was checked against the generated files and all hashes matched.

## Manual Test Gates

These checks record the final manual validation state before publishing.

| Check | Status |
| --- | --- |
| Full interactive x86 workflow test | Passed by manual regression test |
| Real catalogue save/reopen regression test | Passed by manual regression test |
| List views/filter/edit/delete regression test | Passed by manual regression test |
| Settings persistence across app restart | Passed by manual regression test |
| Manual update-check failure path with network blocked | Passed by manual regression test |
| Windows XP SP3 32-bit VM runtime test with .NET Framework 4.0 | Not run |
| GitHub release page upload verification | Not run |

## Release Boundary

- The x86 portable build is the Windows XP SP3 compatibility artifact.
- The x64 portable build is for 64-bit Windows only.
- Windows XP x64 support is unverified unless tested separately.
- The old network installer is not the official Windows XP release path.
