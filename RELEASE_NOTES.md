# Compact Cassette Catalogue 2.0.0 Alpha 6

C3 2.0.0 Alpha 6 is an in-development checkpoint in the repository-owned 2.0
release train. Its scope, evidence, limitations, and package identities will be
recorded here before candidate freeze.

Current implementation establishes a complete immutable, format-neutral
catalogue graph in `C3.Catalogue`. The native-v2 profile now adapts into that
graph before fingerprinting, with complete relationship validation for Brands,
models, decks, Tapes, sides, and recordings. Direct legacy projection and
cross-profile round-trip convergence remain in progress; production mutation
still uses the qualified pre-Alpha-6 path.

This checkpoint is not published. See the
[execution plan](docs/planning/2.0-execution-plan.md) and
[validation record](release/validation/2.0.0-alpha.6.md).
