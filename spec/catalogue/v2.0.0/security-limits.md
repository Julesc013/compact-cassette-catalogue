# Native-v2 security and complexity limits

Every conforming reader, including the CLI, applies these limits before model
construction:

| Boundary | Limit |
| --- | ---: |
| File bytes | 64 MiB |
| XML characters | 64 MiB |
| XML depth | 16 |
| Elements | 1,000,000 |
| Attributes on one element | 8 |
| Characters in one scalar | 1 MiB |
| Brands | 100,000 |
| Cassette models | 250,000 |
| Deck models | 100,000 |
| Deck units | 100,000 |
| Tapes | 1,000,000 |
| Recordings | 2,000,000 |

DTD processing is prohibited, external resolution is disabled, entity expansion
has a zero budget, and namespace/structure validation is exact. Symbolic links,
reparse points, archive entries, network locations, and external media are not
followed by format parsing. File-system orchestration separately validates the
chosen source and destination.

Failure is typed as file-not-found, too-large, unsafe XML, invalid structure,
unsupported format, invalid value, duplicate identity, unresolved reference,
external modification, access denied, I/O failure, or verification failure. A
failure never swaps active state and never deletes a file C3 did not create and
prove it owns.
