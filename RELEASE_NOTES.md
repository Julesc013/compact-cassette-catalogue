# Compact Cassette Catalogue 1.3.0 Alpha 5

C3 1.3.0 Alpha 5 is the retained, intentionally unpublished owner-test preview
for the final original VB.NET WinForms line. Its exact identity is
`1.3.0a5 / Alpha 5 / v1.3.0a5`. It packages the completed repository-side
application and classic-setup repairs without claiming Beta status, public
publication, minimum-operating-system qualification, stable-feed change, or
`legacy/1.x` advancement.

## Owner-test disposition

Alpha 4 remains immutable and useful, but it is not a Beta candidate. Owner
testing found overlapping controls, broken resize relationships, and runtime-
created buttons moving independently of their semantic neighbours. Its
property-only “sizable/scroll-safe” regression did not measure real control
geometry.

Alpha 5 is the corrective Legacy Layout Stabilization checkpoint. It preserves
all Alpha 4 workflow, data, setup, and release-control work while replacing
runtime geometry with Designer-owned native WinForms layout under the
[visual inheritance contract](docs/ui/1x-visual-contract.md). Its packages are
provided for final owner visual, keyboard, accessibility, and workflow tests.
No Alpha 4 package or tag is replaced.

## Alpha test assets

Portable, no-install, no-admin packages:

```text
C3-v1.3.0a5-win-x86-net40-portable.zip
C3-v1.3.0a5-win-x64-net48-portable.zip
C3-v1.3.0a5-win-arm64-net481-portable.zip
```

Offline elevated per-machine classic setup packages:

```text
C3-v1.3.0a5-win-x86-net40-setup.zip
C3-v1.3.0a5-win-x64-net48-setup.zip
C3-v1.3.0a5-win-arm64-net481-setup.zip
SHA256SUMS.txt
```

Portable ZIPs remain authoritative and independently usable. Each setup ZIP
consumes the exact matching portable application/configuration bytes and adds
the lane-native `SETUP.exe` and `UNINSTALL.exe`. No standalone raw EXE or
uninstaller is an authoritative asset.

## Application repairs

- Pending tape edits now resolve through Apply, Discard, or Cancel before
  document transitions, selection changes, and close.
- Application close uses one nonrecursive gate; Save/Discard/Cancel and failed
  or cancelled Save As paths fail closed.
- Open validates one bounded, DTD-disabled temporary typed catalogue before
  changing active state.
- Save uses a same-directory temporary file, durable flush, exact reopen,
  backup, atomic replacement, cleanup, and external-revision detection.
- Named tape mapping preserves identifiers, sequence numbers, creation dates,
  peak level, bias, bias calibration, and level calibration.
- Referenced brands, models, and decks are protected; compatible display-name
  renames cascade safely.
- Bulk tape creation prevalidates and commits atomically with independent,
  monotonic, non-reused sequence numbers and row-derived counters.
- Model notes, deck choices, model/tape counts, and historical display-name
  references use their correct sources.
- Settings migration is durable and retryable without modifying old profiles;
  known update values normalize without resetting unrelated preferences.
- Console export uses a safe configured/Documents path and diagnostic,
  browser, update-check, and write failures remain nonfatal.

## Layout and startup stabilization

- Main, Tape New, all four entity browsers, compact add/edit dialogs, dense
  Deck dialogs, Console, Find Results, Statistics, Settings, and About now use
  Designer-owned role-appropriate container layouts.
- Dedicated viewport panels own scrolling; visible commands remain in
  persistent table/flow command regions and are no longer created or moved at
  runtime.
- Installer and uninstaller pages preserve the original artwork and workflow
  inside fixed artwork/content/navigation ownership rather than floating
  absolute panels.
- Fresh-process application and setup geometry matrices pass all retained
  size, scale, and maximum-text cells.
- Opt-in fail-safe startup milestones isolated the earlier no-window report to
  a forced-minimized verification path. The ordinary visible path passed 40/40
  repeated x86/x64 launches through first idle and normal close.

## Classic setup repairs

Classic setup is offline, version-bound, architecture-specific, elevated, and
per-machine. It uses closed payload and ownership manifests, Program Files,
HKLM uninstall registration, owned common shortcuts, staged verification,
repair/upgrade policy, reversible ownership-only uninstall, and preservation of
catalogues, settings, unknown files, and unowned system state.

An authenticated write-through ten-phase journal covers install, repair, and
uninstall. Installed state is committed last. A later setup invocation recovers
an interrupted transaction deterministically or refuses mutation, and retains
settled/failed evidence. The source regression matrix terminates child processes
at all ten phases of all three operations.

No network fetch, MSI, MSIX, ClickOnce, updater, service, scheduled task,
self-contained runtime, runtime DLL, or per-user installer is included.

## Lanes and qualification boundary

```text
win-x86-net40     VS2017 15.9 / x86 / .NET Framework 4.0
win-x64-net48     VS2022 17.14 / x64 / .NET Framework 4.8
win-arm64-net481  VS2026 / native ARM64 / .NET Framework 4.8.1
```

Repository-side source evidence includes 52 application characterization tests,
8 integrity tests, 6 settings/diagnostics tests, and 85 setup tests, plus the
genome, compatibility, lane, offline, package, setup-genome, accessibility,
documentation, and PowerShell 2 controls.

Alpha 5 distribution retention additionally requires one fetched-ref external
toolchain lock, Candidate-mode builds, closed package verification, and two
clean path-distinct source reproductions. The remaining independent promotion
gates are owner acceptance of this test set, the complete historical Gate 1 record, and
full runtime/setup qualification on exact Windows XP SP3 x86, Windows 7 SP1
x64, and Windows 11 build 22000 native ARM64 environments. A missing target row is a NO-GO,
never an inferred pass. Creating or publishing `v1.3.0b1` requires
the owner's later explicit approval. Stable is `v1.3.0`; the public `VERSION`
feed remains C3 1.2.0 Beta 1 meanwhile.

See the [Alpha 5 layout plan](docs/planning/1.3.0-alpha.5.md),
[Alpha 4 discovery record](release/validation/1.3.0-alpha.4-qualified.md),
[Beta 1 plan](docs/planning/1.3.0-beta.1.md),
[owner authorization](docs/governance/1.3.0-beta1-authorization-2026-08-06.md),
[defect ledger](docs/testing/1.3.0-alpha3-defect-ledger.md), and
[qualification matrix](docs/testing/1.3.0-qualification-matrix.md).
