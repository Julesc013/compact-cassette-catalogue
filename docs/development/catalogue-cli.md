# Catalogue CLI

`c3.exe` is C3's headless validator and migration shell. It targets
.NET Framework 4.0/AnyCPU and is shipped unchanged in both portable lanes. The
CLI delegates every operation to `C3.Infrastructure`; it owns argument parsing,
console output, and process exit codes, but no XML syntax, mapping, transaction,
or loss policy.

```powershell
c3 validate <catalogue>
c3 migrate --dry-run <legacy-catalogue>
c3 migrate <legacy-catalogue> <native-copy>
c3 recover <migration-recovery-journal>
c3 export-legacy <native-catalogue> <legacy-copy>
```

Exit codes are stable automation contracts:

| Code | Meaning |
| ---: | --- |
| `0` | Operation completed or input is valid. |
| `2` | Input or requested transition was safely rejected. |
| `3` | Unexpected runtime or I/O failure. |
| `64` | Command-line usage error. |

Migration is always convert-copy. It refuses the same source/destination path,
existing outputs, reports, and journals. A successful migration emits the native
copy plus deterministic `.migration.json` and `.migration.txt` reports. Recovery
continues only when the legacy source hash and native destination revision still
match the journal.

Legacy export first computes the same loss preview used by future UI. Its v1.1
copy never receives native-only fields, and the adjacent `.export-loss.json`
lists every omitted or flattened concept. Existing output is never replaced.

The repository gate runs eight positive and negative CLI scenarios. Exact
historical reader proof for exported v1.1 files remains owned by
`build/test-compatibility-baselines.ps1`.
