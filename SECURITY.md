# Security Policy

## Supported versions

Security fixes are made for the newest published C3 release and the current
development branch. Older binaries may be useful on legacy systems but do not
receive separate security maintenance.

## Reporting a vulnerability

Do not post an unpatched vulnerability, malicious catalogue, private path, or
crash report in a public issue. Use GitHub's
[private vulnerability report](https://github.com/Julesc013/compact-cassette-catalogue/security/advisories/new)
and include:

- affected C3 version and build lane;
- Windows version and .NET Framework version;
- the smallest safe reproduction;
- expected impact; and
- whether the report or sample contains personal information.

If a catalogue demonstrates the issue, remove unrelated personal data before
attaching it. C3 catalogues and diagnostic reports may contain names, recording
details, local file paths, and system information.

## Security boundaries

C3 is an offline-first desktop application. Catalogue XML is untrusted input:
the supported reader prohibits DTDs and external entity resolution, enforces a
size limit, validates into temporary state, and replaces the active document
only after successful parsing and normalization.

Do not add HTTP fallback, disabled certificate validation, arbitrary in-process
plugins, implicit macro execution, or silent catalogue conversion as a security
workaround.
