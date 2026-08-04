# Security Policy

## Supported versions

Security fixes are considered for the active C3 1.x maintenance line on
`maintenance/1.x` and the current 2.0 development line on `dev`. `legacy/1.x`
and `master` record qualified 1.x and 2.x checkpoints respectively; ancestry on
either ledger does not by itself make an unpublished checkpoint a supported
public release. Older binaries may remain useful on legacy systems but do not
receive separate security maintenance unless a release notice says otherwise.

## Reporting a vulnerability

Do not post an unpatched vulnerability, malicious catalogue, private path, or
crash report in a public issue. Use GitHub's
[private vulnerability report](https://github.com/Julesc013/compact-cassette-catalogue/security/advisories/new)
and include:

- affected C3 version, update channel, and build lane;
- Windows version and .NET Framework version;
- the smallest safe reproduction;
- expected impact; and
- whether the report or sample contains personal information.

For migration/import issues, also identify the source producer/format, legacy or
native operating mode, operation (open, convert, export, recovery), and whether
the original remained unchanged.

If a catalogue demonstrates the issue, remove unrelated personal data before
attaching it. C3 catalogues and diagnostic reports may contain names, recording
details, local file paths, and system information.

## Security boundaries

C3 is an offline-first desktop application. Catalogue XML is untrusted input:
the supported reader prohibits DTDs and external entity resolution, enforces a
size limit, validates into temporary state, and replaces the active document
only after successful parsing and normalization.

Migration files, extension envelopes, reference packs, update metadata, support
bundles, setup bindings, and future plugin protocol messages are also untrusted.
They require bounded parsing, explicit size/complexity limits, canonical path
handling, integrity/provenance checks appropriate to the source, and temporary
state before committing user-visible changes.

Native-v2 conversion writes a new destination and never silently replaces the
legacy original. A recovery or rollback action may modify only files whose
ownership and revision are proven. Setup/repair must not mutate catalogues or
user-authored profile data.

Do not add HTTP fallback, disabled certificate validation, arbitrary in-process
plugins, implicit macro execution, or silent catalogue conversion as a security
workaround.

Executable extensions, if ever accepted, run out of process behind a versioned
capability protocol. AIDE and Universal Setup remain external development/setup
boundaries and are not trusted runtime authorities for catalogue content.
