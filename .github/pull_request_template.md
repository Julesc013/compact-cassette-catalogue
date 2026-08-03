## Outcome

Describe the user-visible result and why the change is needed.

## Target branch

- [ ] Ordinary 2.x work targets `dev`, or bounded 1.x work targets
      `maintenance/1.x`.
- [ ] If this changes `master`, it is an evidence-backed checkpoint promotion
      with the matching validation record, catalogue entry, and tag plan.

## Ownership and compatibility

- [ ] The change follows `docs/architecture/README.md` dependency rules.
- [ ] Catalogue-format behavior is unchanged, or a versioned specification,
      fixtures, migration, and compatibility notes are included.
- [ ] Both build lanes share the same feature behavior.
- [ ] A language port is mechanical and separate from redesign/behavior changes.
- [ ] Settings, update-channel, migration/export, recovery, and baseline-reader
      evidence is included where applicable.
- [ ] Changed UI has keyboard, accessibility, DPI, empty/error, and designer evidence.
- [ ] No private user data is included in fixtures, logs, or screenshots.

## Evidence

- [ ] `./build/verify.ps1 -Rebuild`
- [ ] Relevant manual workflow tested
- [ ] Designer opened for changed WinForms layouts
- [ ] Candidate/compatibility matrix updated where the claim changed
- [ ] Checkpoint lifecycle/catalogue evidence updated where promotion changed

List commands, environments, screenshots, or release-validation records:

```text

```
