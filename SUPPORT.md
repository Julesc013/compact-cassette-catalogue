# Support

Use the [versioned documentation index](docs/README.md) and project
[wiki](https://github.com/Julesc013/compact-cassette-catalogue/wiki) for current
usage material, and
[GitHub Issues](https://github.com/Julesc013/compact-cassette-catalogue/issues)
for reproducible bugs and focused feature requests.

Before reporting a bug:

1. confirm the C3 version and build lane in the About or build information;
2. reproduce with the latest available release if practical;
3. keep a backup of the affected catalogue;
4. note the exact workflow and error text; and
5. remove private data from catalogues, screenshots, and diagnostics.

Include the package lane (`win-x86-net40` or `win-x64-net48`), update channel,
Windows version, .NET Framework version, catalogue format/source producer, and
whether the problem also occurs with a new blank catalogue.

Only GitHub-release assets identified as `supported` or `preview` by their
release evidence are publicly distributed C3 builds. An internal alpha tag,
branch build, loose executable, future reserved profile, or planned setup
binding is not a supported download. See the
[distribution doctrine](docs/development/distribution.md) for the complete status
vocabulary and artifact contract.

For C3 2.0 migration work, also include:

- catalogue operating mode (legacy, convert-copy, native, export, or read-only);
- the operation and last completed step;
- whether the original and backup remain unchanged;
- any migration/loss/recovery report identifier after removing private paths; and
- whether C3 1.x can still open the relevant legacy/output copy.

For settings issues, say whether this was first 2.0 launch, a repeated launch, a
new Windows profile, or a side-by-side 1.x/2.x profile. Never attach `user.config`
without reviewing it for private paths and values.

Security vulnerabilities must follow [SECURITY.md](SECURITY.md), not a public
issue. Support is community-maintained and no response-time guarantee is made.
