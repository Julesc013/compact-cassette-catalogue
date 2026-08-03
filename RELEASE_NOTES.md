# Compact Cassette Catalogue 1.2.1 Beta 1

C3 1.2.1 Beta 1 is in development. It is the staged architecture and
reliability overhaul for the existing C3 product and catalogue format.

## Planned highlights

- Keep one C3 product, one shared source tree, and one catalogue format.
- Preserve the x86/.NET Framework 4.0 Windows XP compatibility lane.
- Introduce an x64/.NET Framework 4.8 modern Windows lane.
- Separate catalogue rules, infrastructure adapters, and WinForms interaction.
- Add transactional save/load behavior, diagnostics, fixtures, and regression
  tests.
- Replace duplicated global and form state with explicit ownership.

## Planned downloads

Portable ZIPs remain the authoritative distribution:

- `C3-v1.2.1-beta.1-win-x86-net40-portable.zip`
- `C3-v1.2.1-beta.1-win-x64-net48-portable.zip`
- `SHA256SUMS.txt`

## Requirements

- Windows XP SP3 or newer and .NET Framework 4.0 for the x86 compatibility
  build.
- Windows 7 SP1 or newer and .NET Framework 4.8 for the x64 modern build.

The x86 build is the compatibility choice. Windows XP x64 is not a supported
claim. Enhanced high-DPI behavior in the modern lane still depends on operating
system capabilities and is not promised uniformly on every Windows 7 system.

## Distribution status

Portable ZIPs are authoritative. The retired in-repository installer and
uninstaller are not part of C3 1.2.1. A future setup integration must consume the
same staged and hash-verified payload as the portable build.

## Verification required before publication

- Both build lanes compile from a clean checkout.
- Shared tests, catalogue round trips, and project parity pass.
- The x86 executable is PE32/I386 and the x64 executable is PE32+/AMD64.
- Manual New/Open/Save/Save As/Edit/Delete/Close workflows pass in both lanes.
- Windows XP SP3 x86 and Windows 7 SP1 x64 runtime checks are recorded honestly.
- Packaged assets are downloaded again and verified against published SHA-256
  hashes.
