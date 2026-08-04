# Permanent branch contract v1

`build/branches.json` is the single machine-readable owner of C3's permanent
branch identities. Build and release automation reads that document instead of
repeating branch names.

The contract distinguishes roles, not publication channels:

- `currentGeneration.qualified` is the qualified current-generation ledger;
- `currentGeneration.integration` is its moving integration branch;
- `legacyGeneration.qualified` is the qualified supported legacy ledger; and
- `legacyGeneration.integration` is its bounded maintenance branch.

All four names must be valid, distinct Git branch names without file/directory
prefix collisions. A future major-generation transition updates this contract
through a separately qualified governance change; tags and feeds remain
independent publication records.
