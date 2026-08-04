# Compatibility corpus contract v1

This schema defines the evidence inventory behind C3's bounded 1.x
compatibility claim. The canonical document is
`fixtures/compatibility/1x/corpus.v1.json`.

The corpus separates three facts that must not be conflated:

- a public artifact existed;
- an artifact emitted a particular catalogue-format identifier; and
- a producer is inside the maintained 1.x compatibility baseline.

Official binaries remain downloadable from their immutable GitHub release URLs
and are hash-pinned, but are not copied into Git while the repository licence is
unresolved. Privacy-safe XML and settings fixtures are committed. Historical
timestamps in generated catalogue fixtures are normalized explicitly so the
corpus is deterministic.

`build/validate-compatibility-corpus.ps1` owns cross-field, Git-tag, URL, file,
and hash validation beyond the JSON shape. Exact-binary reader tests use the
separate opt-in baseline harness after artifacts have been fetched and verified.
