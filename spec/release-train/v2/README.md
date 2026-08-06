# C3 release-train controller v2

Version 2 expands the repository-owned C3 2.0 programme to the accepted Alpha
1-12 sequence followed by Beta 1. It supersedes the active use of v1 without
editing or deleting the v1 schema that governed qualified Alpha 1-4 and the
initial Alpha 5 implementation.

The controller owns only milestone order, active pointer, state, preceding tag,
and publication policy. The release catalogue owns lifecycle/artifact evidence;
the execution plan owns milestone requirements; Git tags and C/E/P history own
immutable qualification facts.

The semantic validator additionally requires:

- the exact sequence Alpha 1 through Alpha 12, then Beta 1;
- exactly one current milestone;
- every earlier milestone qualified and every later milestone pending;
- `lastQualifiedTag` to identify the immediately preceding checkpoint using
  the historical readable Alpha 1-4 spelling or the canonical compact Alpha
  5-12 spelling selected by the release-tag resolver;
- current build identity to match the current milestone;
- qualified state to agree with the release catalogue and annotated tags; and
- any non-null `candidateCommit` to agree with the catalogue's recorded `C`.

Version 2 changes programme shape only. It does not reinterpret qualified tags,
change C/E/P topology, publish an Alpha, alter update feeds, or turn planned
milestones into release evidence.

The schema is strict UTF-8, rejects duplicate and extension members, and retains
the 256 KiB transport ceiling. Another train shape requires another explicit
schema version.
