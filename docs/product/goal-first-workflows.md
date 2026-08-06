# Goal-first creation workflows

Status: **Authoritative product interaction contract**

## Product rule

C3 understands catalogue dependency order so the user does not have to.

```text
one user intent
    -> one continuous task
    -> one reviewed compound plan
    -> one atomic commit
    -> one semantic undo entry
```

Expected missing data produces an action, not an error. Creating a prerequisite
preserves the parent draft, returns to the exact field, selects the created
entity by stable identity, and focuses the next incomplete field. Cancelling
before the final Apply creates no partial entities.

## Blank catalogue to first Tape

`Add Tape` is always available and always opens a useful task.

```text
Add Tape
  -> choose Model or Create cassette model...
       -> choose Brand or Create brand...
            -> return to Model with Brand selected
       -> return to Tape with Model selected
  -> review staged entities
  -> Apply once
```

The task may stage one Brand, one Cassette Model, one or more Tapes, optional
recording details, and an optional Deck. The review names every entity and any
representability warning before mutation. A relevant content-version change
invalidates and rebuilds the plan before commit.

Standalone `Add Brand`, `Add Cassette Model`, `Add Deck`, and `Add Tape`
operations remain available for experienced reference-data work. Goal-first
composition is not silent automatic data invention.

## Relationship categories

Required relationships block final commit while exposing the shortest valid
continuation. A Tape requires a Cassette Model; a Cassette Model requires a
Brand.

Optional contextual data does not create a dead end. A recording Deck offers
`Not specified`, `Unknown`, an existing Deck, or `Create Deck...` according to
the accepted value semantics. C3 never creates a fake entity named `Unknown`.

Unsafe operations remain refusals with useful next actions. A referenced Brand
offers `Show affected models`, `Reassign models...`, `Merge...`, or `Cancel`;
it is never silently deleted.

## Application contracts

Application owns small typed task coordinators, not a generic visual workflow
engine. Required contracts include:

- `OperationRequirement`: stable requirement ID, required/optional state,
  field path, resource key, satisfaction, and suggested action IDs;
- `SuggestedAction`: stable semantic action ID and availability reason;
- `TaskContinuation`: parent operation/draft IDs, return workspace and field,
  expected content version, and focus target;
- `CompoundOperationPlan`: staged changes, affected relationships,
  representability, warnings, expected version, and fingerprint;
- create-and-select results: created identity/projection, recommended selection,
  and return-focus target; and
- `AllowedActionSet`: the one semantic authority for availability across GUI,
  CLI, and any accepted TUI.

Initial concrete coordinators are `CreateModelTask`, `CreateTapeTask`,
`RecordTapeSideTask`, and `ReassignModelsTask`.

## Windows presentation

Expected prerequisites appear in the editor beside the incomplete field, in an
actionable empty state, or on the blank-catalogue start page. Modal dialogs are
reserved for destructive, conflicting, lossy, external, or close/save choices.

Ordinary success uses accessible non-modal status with `Undo`, `Add another`,
and `View` actions. Creation surfaces consistently provide `Apply`, `Apply and
add another`, and `Cancel`. Rapid entry also supports explicit duplicate and
bounded bulk previews. Smart defaults are visible, editable, and derived only
from preferences or named recent-task context.

The blank start state leads with `Add your first tape`, `Import or migrate a
catalogue`, and `Add reference data`. A prominent New command offers Tape,
Cassette Model, Brand, Deck, and Catalogue while keeping New Tape as the primary
collector action.

## Cross-frontend behavior

CLI output returns stable refusal codes and `nextActions`; it never prompts when
redirected and never silently creates prerequisites. An accepted TUI presents
the same choices and returns to the parent task. Frontends share semantic
behavior, not widgets.

## Milestone ownership

- Alpha 6 freezes required/optional/unknown and profile-representability
  semantics; it adds no production frontend.
- Alpha 7 implements requirement, next-action, continuation, allowed-action,
  compound-plan, and create-and-select Application contracts and proves
  Model-to-Brand continuation.
- Alpha 8 implements the blank-catalogue shell and Model-to-Brand Windows flow.
- Alpha 9 completes Tape-to-Model-to-Brand, recording/Deck uncertainty, rapid
  entry, duplicate, bulk preview, merge/reassign, and compound undo.
- Alpha 10 projects the same semantics through versioned CLI machine output.
- Alpha 11 projects them through a qualified TUI or records an explicit deferral.
- Alpha 12 automates and hardens the scenario; the exact Beta 1 candidate receives
  the consolidated owner usability and assistive-technology qualification.

## Beta acceptance scenario

The named `blank catalogue to first Tape` scenario covers a first-time pointer
user, keyboard-only user, screen-reader user, and experienced rapid-entry user
in both lanes. It measures completion, time, errors, backtracking, unexpected
dialogs, lost input, help use, confidence, and explicit commit decisions.

Acceptance requires zero prerequisite error dialogs, dead ends, lost fields, or
unintended partial entities; one final explicit compound commit; one compound
undo; automatic return and stable selection; a complete keyboard route; a
discoverable screen-reader route; and same-lane/opposite-lane save and reopen.
