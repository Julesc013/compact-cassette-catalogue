# Compact Cassette Catalogue 1.3.0 Alpha 3

C3 1.3.0 Alpha 3 is the planned, intentionally unpublished legacy reliability
and classic setup revival checkpoint. The active source projects the exact
`1.3.0a3 / Alpha 3 / v1.3.0a3` identity.

Alpha 2 is complete and immutable: its three portable packages reproduced from
clean path-distinct builds, its annotated tag remains at evidence commit `E`,
and post-tag commit `P` records the unchanged feed, publication, and legacy
boundaries.

## Planned Alpha 3 assets

Canonical portable packages:

```text
C3-v1.3.0a3-win-x86-net40-portable.zip
C3-v1.3.0a3-win-x64-net48-portable.zip
C3-v1.3.0a3-win-arm64-net481-portable.zip
```

Optional offline classic setup bundles:

```text
C3-v1.3.0a3-win-x86-net40-setup.zip
C3-v1.3.0a3-win-x64-net48-setup.zip
C3-v1.3.0a3-win-arm64-net481-setup.zip
SHA256SUMS.txt
```

The portable ZIPs remain authoritative and independently usable. Setup consumes
the exact qualified application bytes; it never rebuilds, downloads, silently
updates, or rewrites C3. No standalone uninstaller is published.

## Intended functional scope

- Complete historical Gate 1 before changing application behaviour.
- Repair lifecycle, load/save, hostile-input, referential-integrity, counters,
  settings, and diagnostic defects one reproduction-backed outcome at a time.
- Run a systematic static, differential, randomized, XML, persistence-fault,
  and lifecycle defect hunt.
- Retain the classic installer/uninstaller project identities and recognizable
  WinForms wizard while replacing their unsafe online and placeholder engines.
- Build source-identical x86/net40, x64/net48, and native ARM64/net481 setup
  executables from shared linked VB.NET source without a runtime DLL.
- Install offline through closed XML manifests, same-volume staging,
  transactional commit, verification, rollback, owned registry/shortcuts, and a
  real self-relocating reversible uninstaller.
- Preserve catalogue XML, profile settings, unknown files, and all unowned data.

## Current status

The Alpha 3 scope decision, plan, manifest contracts, and defect ledger are
ratified. Identity projection does not claim that runtime repairs, setup engine,
target-machine qualification, tag, or Alpha 3 packages are complete.

The exact Windows XP SP3 x86, Windows 7 SP1 x64, and Windows 11 21H2 ARM64
runtime/setup matrices remain required. ARM64 binary inspection on an x64
builder is not native runtime proof.

## Release authority

An annotated `v1.3.0a3` tag and retained Alpha-labelled portable/setup ZIPs are
authorized only after every applicable Alpha 3 gate passes. Public Alpha
publication is not implied.

Every `v1.3.0b1` tag or Beta-labelled retained byte requires explicit human
approval. Public Beta publication requires separate approval. Stable feed
promotion and `legacy/1.x` movement remain separately gated.

The available public user release remains C3 1.2.0 Beta 1. The three-line
`VERSION` feed remains unchanged until stable publication succeeds.

See the [Alpha 3 plan](docs/planning/1.3.0-alpha.3.md),
[owner decision](docs/governance/1.3.0-alpha3-classic-setup-2026-08-05.md),
[defect ledger](docs/testing/1.3.0-alpha3-defect-ledger.md), and
[setup manifest contracts](docs/setup/1.3.0-manifest-contracts.md).
