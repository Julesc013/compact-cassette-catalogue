# Shared interaction patterns

Alpha 5 establishes a small presentation vocabulary shared by both desktop
lanes. These types live in `C3.Presentation.WinForms`; they describe interaction
state and may use WinForms, but cannot own catalogue, storage, XML, preferences,
update, or migration policy.

## Ownership

| Concern | Owner | Presentation responsibility |
| --- | --- | --- |
| Field labels, help, required markers, and input limits | `FieldDefinition` | Bind one definition to every editor instance |
| Catalogue validation and reference rules | `C3.Catalogue` service | Project typed failures beside fields and into a summary |
| Filtered/sorted items | Feature presenter plus catalogue query | Expose count and a useful empty state |
| Stable selection | `WorkspaceState.Selection` | Rebind by identity after refresh; never by row index |
| Current details | `InspectorPresentation<T>` | Project the selected value without owning it |
| Editor draft | Feature editor model plus `WorkspaceState.EditorDraft` | Track unapplied input separately from document dirty state |
| Success, warning, and failure feedback | `FeedbackPresentation` | Explain outcome and next valid action |
| Long-running work | `ProgressPresentation` and workspace background state | Report progress/cancellation without blocking paint |
| Mutation history | Semantic feature commands and `WorkspaceController` | Execute, undo, and redo services; never snapshot controls |

## Command contract

One accepted editor application creates one semantic command. A command calls
the existing catalogue service that owns the rule, retains only the minimum
before/after domain values required for reversal, and returns a failure without
entering history when the service rejects it.

Undo and redo call the same service API as normal work. They therefore retain
validation, reference protection, and storage-adapter behavior instead of
bypassing the catalogue owner. A mutation from a transitional legacy surface is
not reversible: it clears history and invalidates the saved checkpoint so C3
never offers a partial or unsafe undo chain.

## Brands proof

`BrandWorkspacePresenter` is the first implementation of this grammar. It owns
the Brands view, stable selection, editor draft, feedback, and field-error
projection. `BrandService` remains the sole owner of normalization, duplicate
detection, immutable legacy code, and reference-protected deletion.

The reversible commands are deliberately feature-specific:

- create stores the normalized created Brand and its original timestamp;
- update stores the prior Brand and preserves its immutable code/timestamp;
- delete stores the exact deleted Brand and can restore it; and
- any rejected execute, undo, or redo leaves history at its prior position.

The shared WinForms surface must consume this presenter. It must not grow a
second validation implementation in event handlers.
