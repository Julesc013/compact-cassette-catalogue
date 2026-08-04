# C3 settings compatibility fixtures

These synthetic `user.config` files reproduce the public setting schemas and
defaults recorded in the named Git tags. They contain no user data.

| Fixture | Source tag | Assembly version | Historical schema distinction |
| --- | --- | --- | --- |
| `legacy/v1.0.0` | `v1.0.0` | `0.0.0.0` | message and directory only |
| `legacy/v1.1.1` | `v1.1.1` | `0.0.0.0` | Boolean update preference |
| `legacy/v1.1.2` | `v1.1.2` | `0.0.0.0` | String update policy plus last-check time |
| `legacy/v1.2.0-beta.1` | `v1.2.0b1` | `1.2.0.0` | current legacy schema with default `never` |

The fixtures preserve raw stored values because migration semantics depend on
them: Boolean `True` means check on startup and Boolean `False` means never. The
literal `My.Computer.FileSystem.SpecialDirectories.MyDocuments` is a historical
default expression, not a valid resolved directory.

Do not edit these files to match a new implementation. Add a new fixture and
provenance row for a newly discovered public variant.

The fixtures are read-only inputs to the guarded `My.Settings.Upgrade()` work in
[Gate 4 of the 1.3 recovery plan](../../docs/planning/1.3.0-recovery-plan.md#gate-4--settings-continuity)
and the [settings transition matrix](../../docs/compatibility/1x-evidence-matrix.md#settings-transitions).
They are not rewritten or converted in place.
