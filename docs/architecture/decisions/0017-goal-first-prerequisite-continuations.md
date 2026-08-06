# ADR 0017: Goal-first prerequisite continuations

Status: Accepted

Date: 6 August 2026

## Context

The 1.x interface exposes catalogue dependency order as modal dead ends. A user
who asks to add a Tape can be told to add a Cassette Model first; attempting that
can then be refused until a Brand exists. The user must abandon and restart the
original task, and independently committed prerequisite records may remain when
the final Tape is cancelled.

Brand and Model relationships are real catalogue invariants, but making users
manually traverse the entity graph is a presentation failure. A missing optional
Deck is also currently treated more strictly by parts of the interface than by
the catalogue service.

## Decision

C3 adopts one permanent interaction rule:

> Never require a user to abandon a task to create a prerequisite C3 can create,
> and never lose the user's draft while resolving it.

Expected prerequisites produce typed requirements and suggested actions, not
error dialogs. Application owns task continuations, version-bound compound
plans, allowed actions, and atomic commits. A frontend renders those contracts
in its own medium without recreating catalogue rules.

The blank-catalogue Tape workflow stages the required Brand and Cassette Model,
the requested Tape or batch, and explicitly selected optional data. Apply
commits the accepted plan once; Cancel creates nothing; Undo reverses the
accepted task as one semantic unit. Direct Brand and Model commands remain for
reference-data work.

Unknown, absent, estimated, and explicit values remain distinct. Recording
entry does not require a fake `Unknown` Deck entity and is not refused merely
because no Deck has been catalogued unless an accepted catalogue invariant
requires it.

## Consequences

- `C3.Application` will expose stable requirement, continuation, next-action,
  plan, result, and allowed-action contracts.
- WinForms uses actionable empty states and adjacent `New...` commands; routine
  successful creation uses non-modal accessible status.
- CLI machine output exposes the same refusal codes and next-action IDs without
  prompting or silently inventing records.
- A TUI, if accepted, projects the same continuation semantics.
- The first-Tape Beta scenario requires zero prerequisite dead ends, lost draft
  fields, or partial abandoned entities, plus one final commit and one undo.
- This decision does not introduce a generic workflow engine or weaken entity
  invariants.
