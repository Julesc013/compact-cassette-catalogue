# Brands owner qualification matrix

This runbook defines the Brands interaction evidence that source inspection,
relative scaling, UI Automation, and off-screen rendering cannot prove. Under
ADR 0016, accumulate it during alpha development and run it as part of the
complete owner matrix against the exact frozen Beta 1 portable archives after
the automated gate and reproducibility comparison pass.

Do not reuse results after any candidate source, binary, configuration, payload,
or archive byte changes. A changed candidate receives new packages, hashes, and
a complete rerun.

## Evidence header

Record this header in the exact Beta 1 owner evidence before testing:

```text
Candidate source SHA:
x86 ZIP SHA-256:
x64 ZIP SHA-256:
x86 Desktop EXE SHA-256:
x64 Desktop EXE SHA-256:
Tester:
Date/time and time zone:
Windows edition/version/build:
Display(s) and native resolution:
Input devices:
Screen reader and version:
```

Extract each ZIP into a separate clean directory. Confirm its archive and
Desktop executable hashes before launch. Use copies of catalogue fixtures; never
modify the repository fixture in place.

## Lane and scale matrix

Run every row. A programmatic relative-scale pass does not replace these rows.

| Package | Display scale | Minimum size | Long data | Visual defects | Result |
| --- | ---: | --- | --- | --- | --- |
| `win-x86-net40` | 100% | pending | pending | | pending |
| `win-x86-net40` | 125% | pending | pending | | pending |
| `win-x86-net40` | 150% | pending | pending | | pending |
| `win-x86-net40` | 200% | pending | pending | | pending |
| `win-x64-net48` | 100% | pending | pending | | pending |
| `win-x64-net48` | 125% | pending | pending | | pending |
| `win-x64-net48` | 150% | pending | pending | | pending |
| `win-x64-net48` | 200% | pending | pending | | pending |

For each row:

1. launch the exact packaged Desktop executable;
2. open Brands with `F6`;
3. reduce the window to its minimum size and then maximize/restore it;
4. create or inspect a 100-character Brand name and at least 2,048 characters of
   notes;
5. verify the header, filter, list columns, inspector, validation, commands, and
   status remain visible, usable, and free of overlap or clipping that hides
   meaning;
6. verify selection remains stable while resizing; and
7. capture a screenshot for any defect and record its scale, lane, and action.

Dynamic per-monitor DPI behavior is not promised on operating systems that do
not provide it. If moving between monitors changes behavior, record the exact OS
and both monitor scales rather than generalizing the result.

## Keyboard-only workflow

Disconnect or stop using the pointing device for this section. Run once in each
lane at a scale that exposed no visual defect.

| Step | Expected result | x86 | x64 |
| --- | --- | --- | --- |
| Press `F6` in the main window | Brands opens and exposes a useful initial focus | pending | pending |
| Press `Ctrl+N` | New Brand draft opens; name receives useful focus | pending | pending |
| Use `Tab`/`Shift+Tab` | Focus order follows filter, list/editor, and commands without traps | pending | pending |
| Use mnemonics and type valid fields | Every visible operation is reachable without a pointer | pending | pending |
| Press the default action | Valid Brand applies once and becomes selected | pending | pending |
| Press `Enter` on one row | Selected Brand enters edit mode | pending | pending |
| Press `Escape` on an unchanged editor | Editor closes without changing the catalogue | pending | pending |
| Press `Ctrl+F`, type a notes term, then `F5` | Filter receives focus and applies | pending | pending |
| Clear the filter by mnemonic | Full list and stable selection return | pending | pending |
| Press `Ctrl+Z`, then `Ctrl+Y` | Semantic undo and redo update row and status | pending | pending |
| Press `Delete`, then choose No | No deletion occurs | pending | pending |

Record focus loss, invisible focus, duplicate actions, unexpected default
buttons, mnemonic collisions, and any route that requires a pointer.

## Draft, close, and data-safety workflow

Run once in each lane:

1. select a Brand and begin editing;
2. change its name without applying;
3. close Brands and choose **No** when asked to discard the draft;
4. verify the same editor and exact unapplied text remain;
5. close again and choose **Yes**;
6. verify the applied Brand value remains unchanged in the main catalogue;
7. create and apply a new Brand, close Brands, and verify the main catalogue is
   dirty;
8. save the catalogue, close C3, reopen the saved file, and verify the Brand;
9. undo back to a saved checkpoint where applicable and verify dirty state clears;
10. redo and verify dirty state returns.

Expected policy: a draft is never silently applied or discarded. Applied changes
belong to the in-memory catalogue and persist only after the catalogue is saved.
Alpha 5 does not introduce a second Brand autosave or hidden recovery store.

## Reference-protection and error workflow

Use a copy of
`fixtures/catalogues/v1.1.0/valid/populated.xml`. It contains the `MAX` Brand and
a cassette model that references it.

1. open the fixture copy in packaged C3;
2. open Brands, select `MAX`, and request deletion;
3. confirm the deletion preview;
4. verify `MAX` remains present and selected;
5. verify the error explains that cassette models still use the Brand;
6. verify no Undo entry was created for the rejected operation;
7. save as a new file, reopen it in the opposite lane, and verify the Brand and
   model remain intact.

An error that deletes the Brand, loses selection without explanation, creates a
false Undo entry, corrupts the saved file, or hides the reason is release-blocking.

## High contrast and screen reader

Run both lanes under Windows high contrast with a screen reader such as Narrator.

| Check | x86 | x64 | Notes |
| --- | --- | --- | --- |
| Text, selection, validation, status, and focus remain legible | pending | pending | |
| No meaning depends only on color | pending | pending | |
| Window, filter, list, fields, and buttons have useful announced names/roles | pending | pending | |
| List rows and selection changes are announced coherently | pending | pending | |
| Validation and protected-delete feedback can be discovered | pending | pending | |
| Undo/redo labels communicate the pending semantic action | pending | pending | |
| Reading/navigation order matches the visual task order | pending | pending | |

Record the assistive technology name/version and whether live status changes are
announced automatically or require navigation. A discoverable status is not the
same claim as an automatic live-region announcement.

## Acceptance

The Beta 1 Brands interaction evidence passes only when every required row is
completed and no release-blocking behavior, accessibility, data-safety, or
layout defect remains.
Attach defect identifiers and corrective candidate SHAs rather than changing a
failed result to pending.

After a pass, copy the completed tables or their exact evidence reference into
the Beta 1 validation record. Candidate qualification still requires the full
automated verifier, exact-package workflow/performance rerun, path-distinct
reproducible packages, and the repository C/E/P transaction.
