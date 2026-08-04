# C3 compatibility corpus

`1x/corpus.v1.json` is the machine-readable owner for public 1.x producer tags,
source commits, release artifacts, observed catalogue formats, settings/update
profiles, supported-baseline membership, and normalized fixture provenance.

The corpus intentionally does not commit historical executables. Their official
GitHub release URLs, byte lengths, and SHA-256 values are pinned. Use:

```powershell
.\build\fetch-compatibility-baselines.ps1
.\build\build.ps1 -Configuration Release
.\build\test-compatibility-baselines.ps1 -SkipBuild
```

The fetch step downloads only supported producer binaries into ignored
`artifacts/compatibility/official`. The test step verifies every binary before
loading it in a disposable architecture-matched Windows PowerShell process. It
does not launch the historical UI or permit network access from C3. It proves:

```text
current v1.1 writer
  -> each supported hash-pinned 1.x reader
  -> that reader's DataSet writer
  -> current secure production reader
```

The normalized XML files below `catalogues/v1.1.0/historical` are deterministic,
privacy-safe writer-schema fixtures. Their declared normalization replaces only
volatile/culture-sensitive timestamps and line endings. The canonical invalid,
security, and culture fixtures remain beside the ordinary v1.1 fixtures.

Do not broaden support by adding a tag only to prose. Add its immutable source
and artifact evidence, privacy-safe fixtures, executable matrix row, and corpus
classification together.
