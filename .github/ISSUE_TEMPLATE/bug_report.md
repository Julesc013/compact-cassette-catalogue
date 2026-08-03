---
name: Bug report
about: Report a reproducible C3 defect
title: "[Bug] "
labels: bug
assignees: ''
---

## What happened?

Describe the result, the expected result, and whether catalogue data was lost or
changed unexpectedly.

## Reproduction

1.
2.
3.

Does the problem occur with a new blank catalogue? **Yes / No / Not tested**

## Environment

- C3 version and stage:
- Update channel: legacy-1x / alpha / beta / stable / none
- Build lane: `win-x86-net40` / `win-x64-net48`
- Windows version and architecture:
- .NET Framework version, if known:
- Catalogue source producer and format, if known:
- Operating mode: legacy / native / convert-copy / export / read-only / not applicable
- First 2.0 settings launch, repeated launch, or new profile:

For migration/export/recovery defects, state the last completed step, whether the
original and backup remained unchanged, and whether the relevant output opens in
the baseline C3 1.x reader. Remove private paths and catalogue content.

## Evidence

Paste the exact error text and attach a screenshot or sanitized diagnostic report
if useful. Do not upload a personal catalogue, private path, or security exploit.
Use the private process in `SECURITY.md` for vulnerabilities.
