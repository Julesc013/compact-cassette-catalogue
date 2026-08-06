# C3 1.x visual inheritance contract

Status: authoritative for C3 1.3.0 Alpha 5 and later 1.x qualification

This contract defines what “visually faithful” means for the final original
VB.NET WinForms release. It preserves C3's recognizable working interface
while allowing its absolute geometry to be replaced by predictable native
layout behaviour.

## Immutable visual inheritance

Preserve:

- the cassette application icon in window, taskbar, About, and setup identity;
- native Windows menu, dialog, and window chrome;
- the main tape-editor scan order and information hierarchy;
- Model, Basic, Recorded sides, Notes, Side A, and Side B concepts;
- the symmetric Side A and Side B presentation;
- compact catalogue-management dialogs;
- left-filter/right-results browser windows;
- existing field order and terminology unless an approved text defect exists;
- neutral `SystemColors`, ordinary Windows controls, and system focus cues;
- the established compact data-entry density; and
- the original forms, resources, DataSet, catalogue schema, and classic style.

Allowed changes are control parentage, simple layout containers, margins,
alignment, window bounds, content-growth ownership, dedicated scroll fallback,
static command placement, tab order, access keys, empty-state presentation,
and explicitly reviewed visible-text corrections.

Do not introduce custom chrome, skins, glass imitation, gradients, custom
rendering, embedded fonts, decorative button graphics, repeated product
banners, a new navigation shell, or 2.0 frontend concepts.

## Fidelity model

The canonical visual comparison is Windows 7 at 96 DPI, Segoe UI 9 point, and
the approved default window size. Existing-control displacement should remain
within zero to two relative pixels unless a reviewed container migration
records a deliberate relationship change.

At other DPI/font profiles, preserve metrics and hierarchy rather than fixed
pixels. At other themes and operating systems, preserve semantics: native
rendering, system colours, readable text, visible focus, reachable commands,
and no clipping.

## C3 96-DPI metric grid

| Metric | Value |
| --- | ---: |
| Outer dialog or panel margin | 11 px |
| Label-to-control gap | 5 px |
| Related-control gap | 7 px |
| Unrelated-group gap | 11 px |
| First control below GroupBox title | 16 px |
| GroupBox left inset | 9 px |
| Final control above GroupBox bottom | 11 px |
| Command-button separation | 7 px |
| Standard command-button minimum | 75 x 23 px |
| Minimum interactive target | 16 x 16 px |
| Dependent-control indentation | 18 px |
| Localization allowance for text widths | at least 30% |

Input widths communicate expected content. Codes, years, and numeric values
remain compact; Brand and Model names receive medium widths; Notes, recording
titles, and list results receive useful expansion. Additional width belongs
primarily to document, list, and long-text surfaces.

## Typography and scaling

- Canonical design uses Segoe UI 9 point at the form root.
- Children inherit the root font unless a reviewed semantic reason exists.
- Remove unnecessary per-control font assignments during the relevant form
  migration, not in an unrelated bulk rewrite.
- Continue one `AutoScaleMode.Font` model across legacy application forms.
- Test XP's native substitution before considering any runtime font policy.
- Do not ship fonts or mix Font and DPI scaling modes.

System-DPI awareness is a separate adoption gate after all layout work and
real-DPI tests pass. It is accepted only if XP startup and all three target
lanes pass. PerMonitorV2 remains outside C3 1.3.

## Native OEM presentation

Use standard `Button`, `TextBox`, `ComboBox`, `ListView`, `MenuStrip`,
`StatusStrip`, `GroupBox`, and common-dialog behaviour. Use system warning,
error, and information icons with the correct semantics. Never use a question
icon for success.

Use `SystemColors`; do not hard-code foreground/background pairs. In High
Contrast, hide decorative artwork that competes with important text and avoid
transparency or watermark backgrounds. Setup may retain its original artwork
only within a fixed, separate identity column.

## Layout primitives

Use the smallest structure that owns the relationship:

| Relationship | Primitive |
| --- | --- |
| Simple fill surface | `Dock` and limited `Anchor` |
| Clear label/input or major row/column relationship | simple `TableLayoutPanel` |
| Command row | `FlowLayoutPanel` or a simple two-part footer |
| Filter/results browser | `SplitContainer` |
| Reduced-work-area fallback | dedicated `Panel` with `AutoScroll=True` |

Do not nest table layouts deeply. Reconstructed forms use
`Form.AutoScroll=False`. A scrollable panel contains an autosized,
`GrowAndShrink`, top-docked canvas; the canvas is not right/bottom anchored to
the scrolling panel.

## Primary document window: `frmMain`

```text
frmMain
└── tlpMainRoot
    ├── MenuStrip
    ├── header panel
    │   ├── left header: Find + Identification/navigation
    │   └── Actions
    ├── editor host
    │   ├── pnlEmptyCatalogue
    │   └── pnlEditorViewport [AutoScroll]
    │       └── editor canvas: metadata + Side A + Side B
    └── optional StatusStrip
```

Menu, Find, Identification, navigation, and Actions remain near preferred
size. The editor receives remaining height. Side A and Side B grow
symmetrically. Long text grows; compact values do not. Cap the useful editor
width on ultrawide screens and retain the upper-left content origin.

The empty-catalogue and editor panels are Designer-declared mutually exclusive
layers in the same cell. Their bounds are not copied at runtime.

A status strip may later show noncritical current-window state such as file,
tape count, position, and modified status. It follows structural stabilization
and is not an Alpha 5 requirement. Version/copyright belongs in About.

## Dense tape editor: `frmTapeNew`

```text
frmTapeNew
├── pnlTapeViewport [Dock Fill, AutoScroll]
│   └── preferred-width canvas
│       ├── Model/Basic/Recorded/Notes/Bulk stack
│       ├── Side A
│       └── Side B
└── persistent command bar: Add Deck… | flexible space | Add Tape | Cancel
```

Default Windows 7 use should not scroll. The viewport is fallback for the
minimum work area, large fonts, long text, and 150-200% scaling. Add Model is a
Designer control beside the stretching Model choice. Add Deck, Add Tape, and
Cancel stay outside the scrolling region.

## Entity browsers

`frmTapes`, `frmModels`, `frmBrands`, and `frmDecks` share this role:

```text
SplitContainer
├── Panel1: fixed/minimum filter viewport
└── Panel2
    ├── ListView [Dock Fill]
    └── status and Designer-declared Add/Refresh/Edit/Delete footer
```

The filter pane is compact and user-adjustable; the results pane receives most
additional space. Selection remains visible without focus, double-click maps
to Edit, and ordinary data fits default columns. Empty results explain the
next action. Column-state persistence and a column chooser remain separately
governed because they change the settings contract.

## Compact dialogs

Brand and Model add/edit dialogs use `FixedDialog`, `AutoSize`,
`GrowAndShrink`, `CenterParent`, no maximize/minimize buttons, and no taskbar
entry. One simple label/input table and bottom command row is sufficient.

`Cancel` has no mnemonic. It uses `DialogResult.Cancel` and the form's
`CancelButton`; Esc cancels. A dialog-opening command uses an ellipsis, such as
`Add Brand…`; the final commit command does not, such as `Add Brand`.

Deck add/edit forms use a preferred-width content canvas in a scrollable
viewport plus a persistent command bar. Compact checkboxes and numeric fields
do not stretch; Notes and selected descriptive fields may.

## Conditional forms and setup

Console and Find Results are sizable only when their output/results surfaces
fill useful space. Statistics changes only for a reproduced clipping or
reachability defect. Settings and About remain autosized fixed dialogs.

Installer/uninstaller pages use a stable fixed wizard surface: fixed artwork
column, flexible page content, and persistent bottom navigation. A list or
licence control may scroll within a page, but the page does not. Use specific
commit labels such as Install, Repair, and Remove. The desktop-shortcut choice
is unselected by default if that option is presented.

## Density and grouping

Retain major semantic GroupBoxes. Avoid more than one nested GroupBox level
where spacing or a small section label is sufficient. Aim for no more than
roughly seven immediately perceptible groups on one surface. Side A/B internal
Recording, Configuration, and Contents groups remain initially; any lighter
alternative is an owner visual choice after structural proof.

## Visible text and commands

Audit spelling, sentence-style capitalization, field-label colons, ellipses,
concise validation, consistent Add/Edit/Delete/Save terminology, and unique
access keys within each scope. Correct display text without silently rewriting
historical catalogue keys. Stable stored values may map to corrected captions
through `CatalogueChoice`.

## Testable presentation policy

Geometry belongs in Designer-declared hierarchies. `CatalogueWorkflow.vb`
retains workflow/planning policy and may define pure presentation state such
as:

```text
CatalogueUiState
    HasCatalogue
    HasTapes
    HasCurrentTape
    HasPendingTapeEdit
    IsCatalogueDirty
    CanUpdateTape
    CanDeleteTape
    ShowEmptyState
```

One refresh operation applies state to menus, commands, navigation, empty
state, and editor availability. List refresh captures stable keys and restores
surviving selection, focus, and top item rather than row numbers. Noticeable
commands use wait feedback and duplicate-command protection with restoration
in `Finally`.

Persisted bounds, splitter distances, and list columns are desirable but
deferred because they reopen `My.Settings`. Any later proposal must include
migration/corrupt-value tests and clamp restored state to the current work
area.

## Accessibility contract

- Every interactive control is keyboard reachable in task order.
- Each dialog has one safe default command and Esc cancellation.
- Initial focus and post-detour focus are deliberate.
- Access-key labels precede and target their controls.
- Disabled sections have an understandable reason.
- No information is conveyed by colour alone.
- List selection remains visible when focus moves.
- Validation summarizes issues and focuses the first offending control.
- ErrorProvider spacing cannot cover a field or adjacent Add button.
- Common controls and system colours remain the accessibility substrate.

Retain UIA/MSAA tree snapshots, keyboard-only evidence, screen-reader
walkthroughs, and High Contrast Black/White screenshots for Alpha 5 review.

## Immutable references

Reference assets and capture manifests live under
[`docs/ui/reference/`](reference/README.md). A reference proves only the facts
declared by its manifest. It must not be silently replaced when a branch moves.

