# Compact Cassette Catalogue 2.0.0 Alpha 6

C3 2.0.0 Alpha 6 is an in-development checkpoint in the repository-owned 2.0
release train. Its scope, evidence, limitations, and package identities will be
recorded here before candidate freeze.

Current implementation establishes a complete immutable, format-neutral
catalogue graph in `C3.Catalogue`. Both the secure legacy-v1.1 load path and the
native-v2 profile now adapt into that graph, with complete relationship
validation for Brands, models, decks, Tapes, sides, and recordings. The legacy
projection is read-only and is the single mapping owner used by migration;
cross-profile writer convergence remains in progress and production mutation
still uses the qualified pre-Alpha-6 path.

The reverse native-v2 adapter now recreates the frozen persistence DTO graph
from canonical state. Characterization requires the adapted native XML bytes
and semantic fingerprint to remain identical to the original qualified graph.
The legacy model sequence counter is retained in canonical profile evidence and
included in its fingerprint. Frozen native-v2 cannot represent it, so migration
reports that loss and strict ordinary native adaptation refuses it.

Loss-aware v1.1 export now consumes the same canonical state directly. Native
DTO entry points remain only as compatibility shims and adapt before export.
The canonical export preserves the legacy model counter, is reopened through
the secure current reader, and remains subject to the exact historical-binary
reader/writer matrix.

This checkpoint is not published. See the
[execution plan](docs/planning/2.0-execution-plan.md) and
[validation record](release/validation/2.0.0-alpha.6.md).
