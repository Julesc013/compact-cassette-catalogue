# OEM+ desktop experience contract

“OEM+” means a coherent, restrained, first-party-quality Windows experience. It
does not mean custom chrome, novelty controls, or hiding advanced capability.
The UI earns polish through consistency, state clarity, accessibility, recovery,
and speed.

## Workspace model

The target shell has five stable regions:

```text
+---------------------------------------------------------------+
| Menu / primary commands / catalogue identity                  |
+-------------+------------------------------+------------------+
| Navigation  | Searchable, sortable list    | Inspector/editor |
| and views   | or task workspace            | and validation   |
+-------------+------------------------------+------------------+
| Status: mode, dirty state, selection, work, recovery, errors  |
+---------------------------------------------------------------+
```

This is a behavioral layout, not a requirement for one custom control. On small
or legacy displays, panels may collapse or open as focused dialogs while keeping
the same command and editor contracts.

## Interaction grammar

- Lists browse and select; editors own drafts; commands own mutations.
- Create and edit use the same field definitions and validation rules.
- Validation appears adjacent to the field and in an ordered summary.
- Apply commits one command; Cancel discards the draft; closing with changes is
  explicit.
- Delete, bulk edit, import, migration, and loss-aware export show impact before
  execution.
- Undo and redo use domain commands, not snapshots of controls.
- Search and filters show that they are active and offer one obvious reset.
- Empty states explain the next valid action instead of presenting inert space.
- Errors state what failed, what remained safe, and what the user can do next.

## Native visual system

- Use Windows system fonts, colors, focus cues, metrics, and standard control
  behavior wherever they meet the need.
- Centralize semantic spacing, sizing, icon, and typography tokens; forms do not
  invent local magic numbers for shared patterns.
- Prefer layout containers and content-driven sizing. Never place build-lane
  conditionals in designer files.
- Icons have one semantic meaning, accessible names, high-contrast behavior, and
  raster/vector variants appropriate to the supported renderer.
- Animation is optional, brief, and never required to understand state.

## Accessibility and input

Every workflow is complete with keyboard alone. Tab order follows visual order;
mnemonics do not collide within a surface; Enter/Escape behavior is consistent;
focus returns to the initiating context after dialogs; and selected/invalid/dirty
state is not conveyed by color alone.

Controls expose useful accessible names, roles, values, and descriptions. Text
can grow without clipping. System high contrast and reduced visual effects are
respected. Screen-reader and keyboard evidence is recorded for each release
candidate rather than inferred from control choice.

## DPI and compatibility

Shared source provides one logical experience, but evidence is lane-specific.
Test representative 100%, 125%, 150%, and 200% scaling, mixed long/short values,
high contrast, and minimum supported display sizes. The net48 lane may opt into
newer framework DPI behavior at its runtime edge. C3 does not promise dynamic
per-monitor DPI behavior on operating systems that do not provide it.

## Performance budgets

Budgets are measured using named synthetic catalogues and hardware profiles.
Required metrics include cold/warm launch, open/save, list population, filter
latency, editor activation, bulk-command preview/apply, memory high-water mark,
and recovery scan. Long operations are cancellable when cancellation is safe and
never block paint without visible progress.

Numeric thresholds are accepted only after baselines are captured; arbitrary
targets must not drive unsafe caches or duplicated state.

## UI replacement gate

A replacement shell or form is promoted only when it passes:

- workflow parity against the current implementation;
- no direct `DataSet`, XML, settings-provider, or update transport access;
- designer open/save in the authoritative toolchain;
- keyboard, accessibility, DPI, empty/error, and long-data matrices;
- dirty-state, close, undo/redo, and recovery behavior;
- both build lanes from the same physical feature source; and
- representative performance comparison.

Until promotion, the current form remains the production oracle. After promotion,
remove the obsolete implementation promptly so behavior is not duplicated.
