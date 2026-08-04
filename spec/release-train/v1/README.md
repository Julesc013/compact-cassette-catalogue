# C3 release-train controller v1

This schema constrains the repository's resumable C3 2.0 milestone pointer. The
controller is intentionally smaller than the release catalogue: it decides what
work is current; it does not restate package hashes, publication results, tag
objects, or C/E/P topology.

The semantic validator additionally requires:

- the exact sequence Alpha 1 through Alpha 6, then Beta 1;
- exactly one current milestone;
- every earlier milestone to be qualified and every later milestone pending;
- `lastQualifiedTag` to match the immediately preceding qualified alpha;
- current build identity to match the current milestone;
- qualified state to agree with the immutable release catalogue and tags; and
- any non-null `candidateCommit` to agree with the catalogue's recorded `C`.

The v1 publication policy fixes stable to
`metadata-only-rebuild-and-requalification`, as accepted by ADR 0010. This is a
new stable identity from accepted RC source, not renamed or byte-identical RC
artifacts.

The schema is strict UTF-8, rejects duplicate members, has a 256 KiB transport
limit, and permits no extension members. A future train shape requires a new
schema version rather than an ad hoc field.
