# Using the Brands workspace

This page describes the in-development C3 2.0 Alpha 5 Brands workflow. Alpha 5
is intentionally unpublished; public Beta instructions will be refreshed from
the exact accepted packages.

Open the workspace with **View > View Brands** or `F6`. To start with a blank
Brand editor, use **Edit > Add Brand** or `Ctrl+B` in the main window.

## Browse and filter

The list shows the stable two-letter code, Brand name, and notes. Select one row
to inspect or edit it. Hold `Ctrl` while selecting to choose several Brands for
deletion. The filter searches Brand notes; choose **Clear** to return to the full
list.

## Create or edit

Choose **New brand** or press `Ctrl+N`, enter a name and two-letter code, then
choose **Apply**. A Brand code cannot change after creation because it identifies
legacy catalogue relationships. Editing therefore changes the name and notes
only.

Validation appears beside the responsible field and in the workspace summary.
Rejected input does not change the catalogue or enter undo history. **Cancel**
or `Escape` discards the draft only after confirmation when its inputs changed.

## Delete, undo, and save

Deletion always presents an impact confirmation. A Brand used by a cassette
model remains safe and C3 explains why it could not be deleted. When several
Brands are selected, eligible Brands may be deleted while protected ones remain
selected with their errors.

Use `Ctrl+Z` and `Ctrl+Y` to undo or redo completed Brand commands. Each
successful create, update, or individual deletion is one history entry. Undo
changes the in-memory catalogue; it does not bypass normal Brand validation.

Brand commands do not save the catalogue automatically. Return to the main
window and save the catalogue to make the current state durable.
