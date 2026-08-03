# C3 update release manifest v1

`release.json` is the complete availability contract consumed by C3 2.x. It is
not a projection of the current branch version and it does not replace the root
three-line `VERSION` API retained for C3 1.x.

Every document is bounded to 32 KiB and has a closed object shape. Runtime
readers reject missing, duplicate, unknown, malformed, oversized, or
cross-channel data before making an update decision.

[`examples/published-beta.example.json`](examples/published-beta.example.json)
is a schema and tooling fixture. Its lengths and hashes are deliberately
illustrative; it is not a release feed or evidence that those assets exist.
[`examples/invalid-unpublished-assets.example.json`](examples/invalid-unpublished-assets.example.json)
is a negative fixture proving that `published: false` cannot coexist with asset
metadata.
[`examples/invalid-published-alpha.example.json`](examples/invalid-published-alpha.example.json)
proves that alpha cannot be published, while
[`examples/invalid-empty-build-identifier.example.json`](examples/invalid-empty-build-identifier.example.json)
proves that build metadata cannot contain empty dot-separated identifiers.
The `invalid-duplicate-*.example.json` fixtures prove that exact duplicate keys
are rejected at both root and nested package scope before schema projection.

## Publication states

An unpublished document records identity only and must contain exactly:

```json
{
  "published": false,
  "releaseUrl": null,
  "checksumManifest": null,
  "packages": []
}
```

Those values prevent a generated candidate from implying that downloadable
assets exist. The alpha channel is deliberately private and can never set
`published` to `true`; alpha development identity may use
`release/feeds/alpha/`. Future Beta, release-candidate, and stable candidate
generation uses `release/candidates/<release-label>/` so synchronization cannot
overwrite a previously promoted public feed.

A published document is promoted only after the exact tagged GitHub release and
assets exist. It contains:

- the canonical tagged release URL;
- the exact `SHA256SUMS.txt` filename, positive byte length, lowercase SHA-256,
  and GitHub asset URL;
- one or more unique portable package records;
- each package's lane, exact derived filename, positive byte length, lowercase
  SHA-256 digest, and exact GitHub asset URL.

The runtime derives the expected tag, filenames, and URLs from the validated
release identity and rejects disagreements. Build metadata is permitted in an
informational version but does not enter tag names or SemVer precedence. Each
build identifier is a nonempty, dot-separated sequence of ASCII letters,
digits, or hyphens, matching the runtime parser.

## Promotion transaction

1. Generate an unpublished candidate manifest from `build/Version.props`.
2. Qualify and tag the exact candidate.
3. Publish and independently download-verify its packages and checksum file.
4. Populate the self-contained manifest with those observed facts.
5. Promote that exact document to `release/feeds/beta/release.json` or
   `release/feeds/stable/release.json` last.

Changing a public feed before its assets exist is invalid. Running
`build/sync-version.ps1` never performs publication.

## Validation

Run `build/test-update-feed-contract.ps1` to verify candidate routing, direct
Windows PowerShell 5.1 compatibility, and byte-identical PowerShell 5.1/7
projection. `build/test.ps1 -Configuration Release` exercises the bounded .NET
4.0 reader and update decision service against valid and adversarial manifests.
