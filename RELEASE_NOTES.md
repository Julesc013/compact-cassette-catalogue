# Compact Cassette Catalogue 2.0.0 Alpha 5

C3 2.0.0 Alpha 5 is an in-development checkpoint in the repository-owned 2.0
release train. Its scope, evidence, limitations, and package identities will be
recorded here before candidate freeze.

This milestone introduces explicit workspace/document state, practical command
history and undo/redo, reusable validation/list/editor/error/progress patterns,
and one complete proven OEM+ workflow before broader shell replacement. Storage,
migration, and catalogue rules remain owned outside presentation code.

Implemented so far:

- accepted one shared net40/C# 7.3 `C3.Presentation.WinForms` boundary for both
  executable lanes;
- replaced module-owned mutable document/services with one instance-owned
  application composition root;
- introduced explicit document, selection, view, draft, compatibility, recovery,
  and background-operation state;
- added bounded checkpoint-aware command history with safe undo/redo branching;
  and
- expanded the canonical portable payload to include the shared presentation
  assembly from the same manifest in both lanes.

This checkpoint is not published. See the
[execution plan](docs/planning/2.0-execution-plan.md) and
[validation record](release/validation/2.0.0-alpha.5.md).
