# ADR 0011: Shared C# WinForms presentation boundary

- Status: Accepted for C3 2.0 Alpha 5
- Date: 2026-08-05

## Context

C3 has two VB.NET WinForms executable projects over one physical source tree. The
executables correctly own target-framework, CPU, manifest, configuration, and
startup differences, but the remaining global document seam constructs mutable
catalogue state, repositories, services, preferences, and session state in one
module. Forms then locate the main window or read those globals directly.

Alpha 5 must introduce an explicit workspace, command history, practical
undo/redo, and reusable interaction patterns without creating separate x86 and
x64 implementations, moving persistence into controls, or combining a complete
host conversion with the first replacement workflow.

## Decision

Create one `C3.Presentation.WinForms` class library targeting .NET Framework 4.0
with explicit C# 7.3. Both VB executable projects reference the same library.
The executable projects remain the lane-specific bootstrap owners through 2.0
unless later evidence supports a smaller, independently qualified conversion.

The dependency direction is:

```text
C3.WinForms.Net40 ----+--> C3.Presentation.WinForms --> C3.Catalogue --> C3.Domain
                      |                                      ^
C3.WinForms.Net48 ----+--------------------------------------+
                      +--> C3.Infrastructure ----------------+
```

`C3.Presentation.WinForms` may reference `C3.Catalogue`, `C3.Domain`,
`System.Windows.Forms`, and `System.Drawing`. It must not reference
`C3.Infrastructure`, XML, `DataSet`, concrete settings storage, update transport,
or filesystem APIs. The VB bootstrap composes Infrastructure adapters and passes
typed catalogue/application contracts into presentation.

The new library owns only real shared presentation responsibilities:

- one workspace controller and its observable presentation state;
- selection, filter/sort/view, editor-draft, compatibility, recovery, and
  background-operation projections;
- a bounded command history that records successful semantic catalogue commands
  and invokes their catalogue-owned inverse/redo behavior;
- reusable field, validation, list, inspector/editor, empty-state, error, status,
  and progress patterns; and
- C# WinForms controls and presenters used identically by both lane hosts.

The existing `CatalogueSession` remains the sole owner of document path,
persisted revision, dirty state, and change sequence. `DocumentState` observes
and projects that session; it does not cache a second authoritative copy. A
successful mutation marks the session dirty exactly once. Only a verified save
clears it.

Undo and redo operate on typed commands and catalogue-owned mutation services,
never on control snapshots, `DataRow` snapshots, or XML. Failed commands do not
enter history. A new successful command clears the redo branch. History is
cleared when the active document is replaced and is bounded so a long session
cannot grow memory without limit.

The first production proof is Brands. It must use one shared C# presenter/control
path in both executables and exercise listing, stable selection, create, edit,
validation, duplicate handling, referenced-delete refusal, dirty state,
undo/redo, empty/error states, keyboard operation, accessibility, DPI, and
representative performance. Catalogue rules remain in `C3.Catalogue.Brands` and
legacy/native storage remains in Infrastructure.

The authoritative Net40 project remains the designer owner. Designer-generated
files contain layout only and no build-lane conditionals. The current VB Brands
forms remain the behavioral oracle until the replacement proof passes; they are
then removed rather than retained as a second production workflow.

## Consequences

- One C# presentation implementation serves both supported runtime lanes.
- A wholesale VB startup/resources/manifest conversion is not coupled to the
  workspace redesign.
- Presentation code can be tested without launching either executable and
  without using real files or settings.
- The portable payload gains one assembly only when the library enters the
  production graph; the single payload manifest remains authoritative.
- The project is not a generic `Common`, `Core`, helper, or business-service
  layer. Types are added only for demonstrated workspace or feature ownership.
- Any retained VB bootstrap after Alpha 6 requires an explicit owner and exit
  gate before Beta 1.
