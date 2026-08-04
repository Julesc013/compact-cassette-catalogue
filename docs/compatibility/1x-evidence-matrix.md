# C3 1.x compatibility evidence matrix

Status: **Alpha 2 automated laboratory complete; owner/OS workflow evidence remains pending**

The canonical machine owner is
[`fixtures/compatibility/1x/corpus.v1.json`](../../fixtures/compatibility/1x/corpus.v1.json).
This document explains its policy and evidence; it cannot widen the corpus.

## Exact support baseline

C3 2.0 supports catalogue files produced by these public C3 1.x checkpoints:

| Producer | Public status | Runtime/CPU | Catalogue format | Automated artifact rows |
| --- | --- | --- | --- | ---: |
| `v1.0.0` | stable | .NET Framework 4.6 / AnyCPU | `1.1.0` | 1 |
| `v1.1.0` | stable | .NET Framework 4.6 / AnyCPU | `1.1.0` | 1 |
| `v1.1.1` | stable | .NET Framework 4.6 / AnyCPU | `1.1.0` | 1 |
| `v1.1.2` | stable | .NET Framework 4.6 / AnyCPU | `1.1.0` | 1 |
| `v1.2.0b1` | maintained public Beta 1 | .NET Framework 4.0 / x86 and x64 | `1.1.0` | 2 |

The public prereleases `v1.0.0b1` through `v1.0.0b4` and `v1.1.0b1` are
inventoried with exact source and artifact identities, but are not independent
support targets. They emit the same `1.1.0` format and precede a supported
stable producer. This avoids implying that every historical preview is a
permanent maintenance baseline.

Format identifiers `1.0.0`, `1.0.1`, and `1.0.2` are real, but their hash-pinned
producers are public C3 `0.1.0`, `0.2.0`, and `0.3.0` binaries bundled under the
`v0.5.4` release. They are archival pre-1.x provenance, not part of the C3 1.x
compatibility promise. C3 2.0 currently rejects them distinctly instead of
guessing a migration. Supporting them later requires a separate accepted import
profile and fixtures.

## Executable catalogue matrix

The maintained-machine Alpha 2 run completed six exact artifact rows:

```text
production LegacyXmlCatalogueStore writer
  -> v1.0.0 reader/writer
  -> production LegacyXmlCatalogueStore reader

production LegacyXmlCatalogueStore writer
  -> v1.1.0 reader/writer
  -> production LegacyXmlCatalogueStore reader

production LegacyXmlCatalogueStore writer
  -> v1.1.1 reader/writer
  -> production LegacyXmlCatalogueStore reader

production LegacyXmlCatalogueStore writer
  -> v1.1.2 reader/writer
  -> production LegacyXmlCatalogueStore reader

production LegacyXmlCatalogueStore writer
  -> v1.2.0b1 x86 reader/writer
  -> production LegacyXmlCatalogueStore reader

production LegacyXmlCatalogueStore writer
  -> v1.2.0b1 x64 reader/writer
  -> production LegacyXmlCatalogueStore reader
```

Before executing an old binary, the harness verifies the corpus byte length and
SHA-256, then checks its embedded product/stage/catalogue constants. It invokes
only the known `varGlobals` schema and `DataSet` read/write path in a disposable
process of the correct architecture; it does not launch forms or perform an
update check. The resulting file must reopen through C3 2.0's secure production
adapter with the expected brand, model, deck, and tape counts.

Both current lanes build and carry a byte-identical `C3.Infrastructure.dll`, so
the v1.1 adapter has one implementation owner. The historical `v1.2.0b1` x86 and
x64 artifacts are nevertheless tested independently. Packaged UI workflow and
minimum-OS results remain separate manual evidence, not inferred from this
library-level matrix.

## Settings matrix

| Historical profile | Producers | Automated evidence |
| --- | --- | --- |
| message Boolean + directory String | 1.0.x and 1.1 Beta 1 | exact fixture, bounded reader, unchanged source bytes |
| plus update Boolean | 1.1.0 and 1.1.1 | exact fixture, Boolean-to-policy mapping, unchanged source bytes |
| update String + last-check DateTime | 1.1.2 and 1.2 Beta 1 | two exact fixtures, empty/date handling, unchanged source bytes |

The importer also proves missing, malformed, oversized, nested, newer-invalid
fallback, locked/unavailable, repeated initialization, failed-checkpoint retry,
and cross-instance merge behavior. An imported/not-found/invalid outcome is
checkpointed atomically; transient discovery/access/save failures remain
retryable. No legacy `user.config` is changed or deleted.

## Update and promotion matrix

The inventory distinguishes no updater, unconditional legacy text-feed checks,
Boolean-gated checks, scheduled checks, and the hardened 1.2 behavior. The
three-line root/legacy feed remains on 1.x. C3 2.x uses bounded channel manifests
and never publishes an alpha manifest.

Repository tests cover update identity precedence, malformed/unsafe manifests,
channel-bound endpoints, unavailable network sources, candidate/tag/post
promotion dry runs, feed ownership, and create-only SHA-bound transport. These
are protocol tests; no test rewrites or contacts the live legacy feed.

## Behavior classification

### Required behavior

- read and write the supported unqualified XML `Catalogue`/`1.1.0` profile;
- preserve represented brand/model/deck/tape values across the legacy round trip;
- keep product identity metadata informational and independent from file format;
- normalize derived counters from actual rows rather than trust stale values;
- preserve legacy settings sources while mapping known values into the C3-owned profile; and
- isolate 1.x and 2.x update publication so an unpublished preview is never advertised.

### Tolerated legacy quirks

- supported prerelease artifacts sometimes report `Release` internally;
- `v1.2.0b1` reports product `1.2.0 / Release` despite its Beta 1 tag/release; and
- historical informational date strings were culture-dependent.

These are accepted as input metadata. They do not define current product stage
or parsing culture.

### Defects corrected, not preserved

- 1.1.2's elapsed-time sign causes scheduled checks to run too frequently;
- several 1.x clients treat any unequal remote version as newer;
- historical XML loading was not hardened against hostile structure;
- historical saving was not transactional or externally revision-aware; and
- historical mutable counters and UI globals could diverge from actual rows.

Regression tests preserve the corrected safety/result contract, not the defect.

## Explicitly pending evidence

Before public Beta 1, the owner still validates the exact frozen packages for:

- Windows XP SP3 x86 and Windows 7 SP1 x64 minimum-OS launch/workflows;
- packaged cross-lane create/open/edit/save/reopen behavior;
- first/repeated startup against isolated copies of real historical profiles;
- side-by-side 1.x/2.x operation and rollback; and
- accessibility/DPI behavior relevant to the public compatibility claim.

Native-v2 conversion, v1.1 loss-aware export, and deterministic migration are
Alpha 4 work and cannot be inferred from the completed legacy-format matrix.
