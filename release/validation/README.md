# Release validation records

Each file records evidence for one exact candidate identity and source commit.
Records are never renamed to make an old candidate look like a newer release.

| Record | Classification |
| --- | --- |
| `1.2.0-beta.1.md` | Historical 1.x candidate/release evidence |
| `1.2.1-beta.1.md` | Superseded unpublished local candidate evidence |
| `2.0.0-alpha.1.md` | Active C3 2.0 development candidate evidence |

A superseded record keeps its original hashes and limitations. A new identity
requires a clean build, new packages, new hashes, and a new validation record.
