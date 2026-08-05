# Compact Cassette Catalogue 1.3.0 Alpha 1

C3 1.3.0 Alpha 1 is an intentionally unpublished source checkpoint. It freezes
the recovered maintenance foundation for the final original C3 release line.
It is not a user release and does not yet contain the planned data-safety and
integrity repairs.

## Subsequent stable-matrix decision

After the immutable Alpha 1 checkpoint, the owner ratified exactly three Beta
and stable release lanes: `win-x86-net40`, `win-x64-net48`, and native
`win-arm64-net481`. The earlier four-lane Alpha build remains truthful
historical evidence; it is not the stable publication matrix. VS2015/MSBuild 14
remains the historical 1.2 reconstruction oracle, while VS2017/MSBuild 15 is the
authoritative 1.3 x86/net40 compiler. Stable assets are portable ZIPs only; no
installer or updater is part of C3 1.3. See the
[ratified owner decision](docs/governance/1.3.0-three-lane-matrix-2026-08-05.md).

## Three-lane preparation status

The bounded correction is implemented and mechanically validated: the manifest
contains exactly three lanes, native ARM64 compiles as `0xaa64`, compiler and
reference inputs are recorded by hash, the genome has zero approved difference,
and deterministic packages contain only the five portable allow-listed files.
Builder smoke executes only compatible architectures; exact-hash target proof
is reserved for XP x86, Windows 7 x64, and Windows 11 ARM64.

This is preparation, not Beta qualification. The installed builders are one
servicing release behind the decision-date pins, the candidate lock is
intentionally unfrozen, historical Gate 1 and runtime repairs remain open, and
no target-machine workflow matrix has been claimed. No new tag, binary release,
feed change, or `legacy/1.x` promotion follows from this result.

## What Alpha 1 establishes

- Direct development ancestry from safe post-release tip `58a5b7d...`.
- Exact production-genome authority from package source `509c9ec...`, with
  `v1.2.0b1` / `2413e913...` retained as the qualified release oracle.
- Archival preservation of the superseded 1.2.1/refactor attempt.
- A hard compatibility constitution for the original VB.NET WinForms app,
  DataSet catalogue, settings, UI resources, and XML format 1.1.0.
- A complete severity-classified ledger of legacy repair candidates.
- Separate Alpha 1, Beta 1, and stable gates.
- Historical catalogue and settings fixtures with strict provenance validation.
- A mechanical baseline-genome manifest and reviewed exception mechanism.
- Standalone XML characterization tests.
- Four source-identical build lanes using the one original project:
  x86/net40, x64/net40, x64/net48, and x64/net481.
- Binary verification for CPU, target framework, settings parity, and the
  absence of new runtime DLLs.
- Exact download/hash verification for the official 1.2.0 Beta 1 executables.

## Validation status

Alpha 1 requires the complete automated maintenance-foundation suite to pass.
On the current preparation host:

- the baseline genome passes with zero approved differences;
- the compatibility corpus validates ten releases and five supported producers;
- all eight XML characterization tests pass;
- all four lanes compile and pass PE/framework/settings/payload checks; and
- both official baseline executables match their recorded sizes and SHA-256.

All four diagnostic applications created their main windows. In the recorded
full run, net48 and net481 closed normally; both net40 lanes reproduced the
known recursive close-cancellation defect and were terminated by the bounded
smoke cleanup. Alpha records this baseline defect for Beta repair.

VS2017/MSBuild 15 is available for diagnostic net40 builds on this host. The
required historical VS2015/MSBuild 14.0.25420.1 reconstruction remains a Beta 1
entry gate and is not represented as passed.

## Not included

- No runtime lifecycle, persistence, tape, counter, referential, settings, or
  diagnostic repair.
- No 1.3 portable package.
- No public GitHub prerelease.
- No update-feed promotion.
- No `legacy/1.x` promotion.
- No new Windows support claim.

Known baseline data-safety defects remain open and block Beta 1 until addressed
through the documented one-defect/one-regression commit sequence.

## Update feed and downloads

The repository-root three-line `VERSION` file intentionally remains:

```text
1.2.0
Release
14/05/2026
```

Existing clients therefore continue to see the available 1.2.0 release. Alpha
1 is identified only by annotated source tag `v1.3.0a1`; no binaries are attached.

## Next milestones

Beta 1 (`v1.3.0b1`) completes the baseline reconstruction gate, every approved
runtime repair, settings continuity, authoritative builds, deterministic
packages, and the full compatibility/manual matrix. Stable (`v1.3.0`) is then a
metadata-only transition from accepted Beta source followed by a complete new
build, qualification, publication, download verification, and final feed
promotion.

See `docs/planning/1.3.0-beta.1.md`, `docs/planning/1.3.0-stable.md`, and
`TODO.md` for the complete work breakdown.
