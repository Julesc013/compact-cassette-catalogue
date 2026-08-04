# Universal Setup integration boundary

Status: **Planned; portable packages remain authoritative**

This boundary specializes the canonical
[distribution doctrine](../development/distribution.md). It creates no current
setup artifact or support claim.

[Universal Setup](https://github.com/Julesc013/universal-setup) is intended to own
generic install, verify, repair, rollback, and uninstall mechanics. C3 supplies a
declarative product binding after that contract is stable and audited.

## Ownership

C3 owns:

- product, channel, lane, architecture, and compatibility identity;
- the exact staged payload and SHA-256 values;
- branding, shortcuts, file-association policy, settings/profile policy, and
  supported upgrade/side-by-side relationships;
- catalogue, migration, backup, and recovery behavior; and
- release validation and user documentation.

Universal Setup owns:

- elevation and per-user/per-machine transaction mechanics;
- install planning, verification, repair, rollback, and uninstall journal;
- generic payload acquisition and integrity enforcement;
- generic shell integration adapters; and
- setup-specific logs and recovery.

It does not rebuild C3, carry a second file list, reinterpret channel policy,
migrate catalogues, or own C3 settings.

## Payload contract

Portable staging is the single payload authority. The machine-owned file list is
`release/profiles/portable-payload.v1.json`; a setup binding references the exact
rooted tree produced at `artifacts/staging/<lane>/C3-v<release-label>-<lane>-portable/`
and its hashes. Setup may add its own bootstrap and transaction metadata, but
installed C3 binaries must be byte-identical to the corresponding verified
portable payload.

Universal Setup must call or consume this staging contract. It must not parse a
ZIP as an indirect source of product facts, rebuild C3, or introduce a binding
with a second payload enumeration. No `bindings/universal-setup/` directory is
created until Universal Setup publishes a stable versioned schema that C3 can
validate mechanically.

If Universal Setup cannot consume that contract, portable distribution remains
the complete supported release. Setup availability is not allowed to delay a
safe portable repair or rollback path.

## Identity and lifecycle

- 1.x and 2.x have distinct install identities and may coexist.
- x86 and x64 lane transition behavior is explicit; architecture is never guessed
  from the current OS alone.
- Alpha, beta, and stable channels do not overwrite one another without an
  explicit policy and user choice.
- File associations are opt-in and point only to an installed, verified build.
- Repair never overwrites catalogues or user-authored profile data.
- Uninstall removes only files whose ownership is proven by the install journal.
- Rollback restores the previous verified payload and does not reverse a catalogue
  migration automatically.

## C and C++ boundary

Universal Setup may use C11 for its native engine/bootstrap according to its own
architecture. C3 does not import that code into catalogue/domain assemblies.
C++11 is considered only for an isolated, measured native requirement with a
narrow ABI; it is not a default integration language.

## Promotion gate

Before setup is published for a C3 release, prove clean install, launch, verify,
repair after controlled damage, upgrade, downgrade rejection, rollback, uninstall,
side-by-side 1.x/2.x, x86/x64 policy, no-admin/per-machine behavior as applicable,
long/Unicode paths, locked files, interruption recovery, and equality with the
portable hashes. Record setup evidence beside the same release candidate.

Do not add a guessed C3 binding file until Universal Setup's real schema and
versioning policy are stable enough to validate mechanically.
