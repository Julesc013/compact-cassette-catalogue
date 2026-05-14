# Compact Cassette Catalogue 1.2.0

C3 1.2.0 is a compatibility and repair release focused on a stable portable distribution path for Windows XP-era systems and newer Windows versions.

## Highlights

- Retargeted the main application to .NET Framework 4.0.
- Added explicit x86 and x64 release build configurations.
- Kept x86 as the Windows XP SP3 compatibility build.
- Made the app offline-first by default.
- Disabled automatic update checking by default.
- Fixed update-check settings and weekly/monthly scheduling behavior.
- Improved update-check failure handling so startup network/TLS failures do not block app use.
- Hardened catalogue file-version detection when opening XML catalogues.
- Updated documentation and release metadata for 1.2.0.

## Download

Use the x86 portable build for maximum compatibility.

Recommended assets:

- `C3-v1.2.0-win-x86.exe`
- `C3-v1.2.0-win-x86-portable.zip`
- `C3-v1.2.0-win-x64.exe`
- `C3-v1.2.0-win-x64-portable.zip`
- `SHA256SUMS.txt`

## Requirements

- Windows XP SP3 or newer for the x86 compatibility build.
- 64-bit Windows for the x64 build.
- .NET Framework 4.0.
- 32 MB RAM minimum.
- 2 MB disk space minimum.

## Notes For Windows XP Users

C3 stores catalogues locally as XML files and does not require network access to run.

Online update checking may fail on some old Windows installations because GitHub HTTPS connections may require newer TLS support than the operating system provides. If that happens, open the releases page manually in a browser.

Windows XP x64 support is unverified unless tested separately. Use the x86 build for the Windows XP SP3 compatibility path.

## Installer Status

The portable x86 build is the official 1.2.0 Windows XP compatibility distribution path. The older network installer is not the recommended XP installation path unless separately repaired and tested.

## Verification Status

- VS2015/MSBuild 14 Release x86 build: passed.
- VS2015/MSBuild 14 Release x64 build: passed.
- PE architecture verification: passed.
  - x86: PE32, I386, 32-bit required.
  - x64: PE32+, AMD64, 32-bit not required.
- Limited launch smoke: passed for x86 and x64 on modern Windows.
- Release assets and SHA256 sums: generated locally under `Releases\1.2.0`.
- Full interactive workflow test: not run.
- Windows XP SP3 runtime test: not run.
