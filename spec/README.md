# Machine-readable contracts

C3 keeps versioned, reviewable wire and ledger contracts here. A schema defines
shape; the corresponding runtime or release validator still owns cross-field,
repository-history, endpoint, and artifact semantics.

`build/validate-json-document.ps1` is deliberately a small, fail-closed JSON
Schema 2020-12 subset that runs identically on Windows PowerShell 5.1 and
PowerShell 7. It supports:

- `$schema`, `$id`, `title`, `$defs`, and direct local `$ref` values of the form
  `#/$defs/<name>`;
- `type`, `const`, `enum`, `required`, `properties`, and
  `additionalProperties`;
- `items`, `minItems`, `maxItems`, and `uniqueItems`;
- `minLength`, `maxLength`, `pattern`, and the `uri` format;
- `minimum`, `maximum`, `allOf`, `oneOf`, `if`, `then`, and `else`.

Unknown keywords, formats, external/container/cyclic references, duplicate
schema or document members, malformed UTF-8, invalid schema keyword types, and
configured size-limit violations are errors. This is not a general-purpose JSON
Schema library. Adding a keyword or format to a C3 schema requires implementing
its exact semantics here and extending `build/test-json-validator.ps1` under
both supported PowerShell generations first.

Schema documents themselves are strict UTF-8 and bounded to 1 MiB. Each
consumer supplies its tighter or looser versioned document ceiling, such as
32 KiB for update manifests and 4 MiB for the long-lived release catalogue.

Current contracts:

- [`catalogue/v1.1.0/`](catalogue/v1.1.0/) — accepted legacy catalogue profile;
- [`release-catalog/v1/`](release-catalog/v1/) — checkpoint ledger; and
- [`update-feed/v1/`](update-feed/v1/) — update discovery and release assets.
