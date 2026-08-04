# Brands workspace design and evidence

The Alpha 5 Brands workspace is C3's first production OEM+ workflow. One
net40-compatible C# form, presenter, and semantic command set serves both the
x86/net40 and x64/net48 hosts. The previous VB browse/create/edit Brand forms
were removed when the shared implementation entered the build so there is one
production UI owner.

## Layout and behavior

The focused workspace applies the five-region product grammar at dialog scale:

1. the heading identifies the feature and persistence behavior;
2. the notes filter is explicit and has an obvious reset;
3. the list supports stable code-based single or multiple selection;
4. the inspector and create/edit surface share the same fields; and
5. the command/status regions expose undo, redo, count, feedback, draft state,
   and catalogue dirty state.

The form is resizable, uses layout containers, inherits the Windows message-box
font, uses system colors and controls, and scales from a logical 720 by 450
minimum using `AutoScaleMode.Dpi`. Code is editable only for a new Brand because
the legacy Brand code is stable identity. Deleting several Brands remains a
sequence of semantic commands: eligible Brands are removed, protected Brands
remain selected with exact errors, and Undo restores successful deletions one at
a time.

## Keyboard contract

| Input | Result |
| --- | --- |
| `F6` in the main window | Open Brands |
| `Ctrl+B` in the main window | Open Brands with a new draft |
| `Ctrl+N` | Begin a new Brand draft |
| `Enter` on one selected row | Edit that Brand |
| `Delete` | Preview deletion of the selected Brands |
| `Ctrl+Z` / `Ctrl+Y` | Undo / redo when no editor draft is active |
| `Ctrl+F` | Focus and select the notes filter |
| `F5` | Refresh/apply the filter |
| `Escape` | Cancel the editor, with confirmation for changed input |

Mnemonics cover the visible filter, editor, and command actions. A draft is
separate from document dirty state and cannot be silently discarded by closing
the workspace.

## Evidence state

Automated evidence currently proves:

- the designer and code-behind remain separate project-owned files;
- no former Brand form can re-enter the VB host unnoticed;
- the form resides in the one shared presentation assembly;
- both executable lanes compile the same presentation DLL;
- DPI autoscaling, resizability, logical minimum size, key routing, accessible
  control names, multi-selection, and field limits are mechanically checked;
- create/update/delete, field and duplicate rejection, reference protection,
  selection across refresh, partial multi-delete, dirty checkpoints, undo/redo,
  and exact timestamp restoration are characterized; and
- off-screen browse and create-state renders have been inspected on the current
  maintained development host.

This is implementation evidence, not yet Alpha 5 qualification. The exact frozen
candidate still requires Visual Studio 2017 designer open/save, packaged workflow
execution in both lanes, measured representative performance, 100/125/150/200%
DPI, keyboard-only, high-contrast, screen-reader/accessibility, long-text,
minimum-size, and dirty-close matrices. Those results belong in the Alpha 5
validation record and cannot be inferred from source or automated contracts.
