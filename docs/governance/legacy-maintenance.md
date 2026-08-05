# Legacy 1.x maintenance policy

This policy governs the final original C3 line from development baseline
`58a5b7d21daf19e1b6112d44efb887c7d8ea9500` through C3 1.3.0 and any
exceptional 1.3.1 correction.

## Purpose and authority

C3 1.3 exists to remove known data-loss, lifecycle, integrity, and settings
continuity defects while preserving the original VB.NET WinForms program and
catalogue format. It is not a feature train and is not a staging ground for 2.x
architecture.

The authority order is:

1. the genetic baseline generated from package source `509c9ec...`;
2. the qualified behaviour and data contract at `v1.2.0b1` / `2413e913...`;
3. the documentation-preserving development ancestry through `58a5b7d...`;
4. the compatibility oracle supplied by `v1.1.2`;
5. the ratified post-Alpha three-lane build/distribution decision;
6. accepted reproductions and regression tests; and
7. archived refactor discoveries as patch intent only.

If an attractive change conflicts with a higher authority, it is deferred to
2.x.

## Allowed change classes

`dev/1.x` accepts only:

- a reproduced data-loss, corruption, security, startup, or lifecycle repair;
- a referential/counter correction required for internal consistency;
- settings migration required to preserve an existing user preference;
- source-identical build, packaging, or verification work;
- compatibility fixtures and tests;
- release identity, documentation, and evidence; or
- a narrowly necessary platform correction for an advertised package.

The following are not legacy maintenance:

- new user features or fields;
- UI redesign, broad DPI work, or form ownership changes;
- production project splitting, source relocation, or service/repository layers;
- a new catalogue format or identifier migration;
- a new updater protocol;
- third-party runtime dependencies; and
- opportunistic cleanup unrelated to a reproduced defect.

## Distribution boundary

C3 1.3 publishes exactly three portable classic WinForms ZIPs: x86/net40,
x64/net48, and native ARM64/net481. MSI, MSIX, ClickOnce publication,
bootstrapper, installer, uninstaller, background updater, self-contained
runtime, and new application DLL graph are outside the core release. Historical
setup projects remain evidence only. A future Universal Setup binding consumes
already-qualified payload hashes as a separate programme and cannot block or
redefine portable stable.

## Change discipline

Every runtime correction must be independently reviewable:

1. record the reproduction and affected historical versions;
2. add or identify a failing regression;
3. patch the smallest original file set;
4. run the genome gate and focused regression;
5. run the complete preparation/qualification suite;
6. update the salvage ledger and milestone evidence; and
7. commit one outcome with no unrelated formatting or designer regeneration.

Never combine multiple runtime defects merely because they touch the same form.
Never replace a whole source file when a bounded edit is possible.

## Severity and deferral

| Severity | Meaning | Release rule |
| --- | --- | --- |
| S0 | confirmed catalogue loss/corruption or exploitable security issue | Blocks every preview and stable package |
| S1 | common workflow can lose edits, overwrite external work, or prevent startup | Blocks Beta 1 and stable |
| S2 | referential, counter, field-mapping, or compatibility error with recovery | Blocks stable; normally blocks Beta 1 |
| S3 | bounded usability or diagnostic defect without data risk | May defer with explicit owner decision |

Deferral records the reason, affected workflow, workaround, target line, and
owner decision. Silence is not deferral.

## Branch, tag, and feed rules

- `dev/1.x` owns unqualified maintenance work.
- `legacy/1.x` advances only to a fully qualified checkpoint.
- Alpha tags may identify intentionally unpublished engineering checkpoints on
  `dev/1.x`; they do not advance `legacy/1.x` or the update feed.
- Beta tags require a feature/compatibility-complete candidate and the Beta 1
  gate. A public beta uses already-qualified retained packages.
- `v1.3.0` identifies the stable qualification commit, not an arbitrary moving
  branch tip.
- The root three-line `VERSION` remains at 1.2.0 until stable 1.3.0 assets pass
  post-publication verification. Feed promotion is the last release action.
- Annotated tags and published evidence are immutable. Corrections add history.

## After stable

After C3 1.3.0, both permanent 1.x branches normally remain at the same verified
checkpoint. A 1.3.1 release requires an S0/S1 security, data-loss, startup, or
platform regression. All other requests move to `dev/2.x` or are declined.
