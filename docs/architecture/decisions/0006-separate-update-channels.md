# ADR 0006: Separate legacy and 2.x update channels

- Status: Accepted
- Date: 2026-08-04

## Context

Published 1.x executables fetch the raw root `VERSION` file and parse its first
line as a numeric version. The build previously generated that file from current
source metadata. Reclassifying development as 2.0 would therefore advertise an
unpublished preview to every checking 1.x client.

## Decision

Treat the root `VERSION` as a legacy public API and keep it identical to the
`legacy-1x` channel feed. Current build metadata generates only the selected 2.x
development feed and binary projections. C3 2.x checks its own configured
channel.

Add explicit stable, beta, and alpha channel locations before they are used.
Only alpha is populated from the current development version. A feed is promoted
after its exact release assets exist and pass post-download verification.

## Consequences

- A source version bump cannot notify stable legacy users accidentally.
- Build identity and published availability are intentionally different facts.
- Release automation must validate and promote channel metadata independently.
- Root `VERSION` cannot be retired until remaining 1.x clients have an alternate
  maintenance strategy or their support window closes.

## 2026-08-04 amendment

Generated alpha metadata with `published: false` is build identity, not a
promoted update feed. Under ADR 0008, qualified alphas remain unpublished and do
not promote channel availability. A 2.x updater must honor explicit publication
state and complete prerelease identity; the legacy three-line numeric comparison
is not sufficient for Alpha/Beta/RC discovery.
