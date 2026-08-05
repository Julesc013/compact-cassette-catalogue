# ADR 0014: Profile capability and representability

- Status: Accepted for C3 2.0 canonical convergence
- Date: 2026-08-06

## Context

A catalogue may be logically valid while a particular storage profile cannot
save it directly or an export profile can represent it only after a documented
normalization or loss. Treating those questions as one `IsValid` flag would
either weaken the logical model to the oldest format or allow a lossy write to
look safe.

## Decision

C3 answers three questions independently:

1. `ValidationResult` reports format-neutral logical validity.
2. Domain-owned `CatalogueProfileCapabilities` publishes what a named profile actually
   supports; absence of a capability is never interpreted as support.
3. `RepresentabilityResult` reports whether a direct save or export copy can be
   produced and whether it is lossless.

Representation issues use stable codes and paths and classify their effect as:

```text
Normalization     representable without information loss
InformationLoss   representable, but not lossless
Unsupported       operation must be refused
```

Direct save additionally requires the profile to advertise direct-save support.
An export may be lossy only when the caller explicitly accepts the reported
losses; this contract does not itself authorize a write.

The published `legacy-v1.1` capability projection advertises no durable entity
identity. The frozen `native-v2.0` profile advertises durable catalogue/entity
identity, field provenance, and relationship stability across reopen. It does
not advertise the qualified values or partial historical dates introduced by
ADR 0013 because those values require an explicit successor profile.

## Consequences

- Logical validation does not mention a persistence profile.
- The frozen Catalogue migration API remains unchanged; these cross-cutting
  contracts live beside validation and value semantics in `C3.Domain`.
- Adapters own the inspection that produces representability issues.
- UI and CLI surfaces render typed results; they do not rediscover format
  limits.
- A profile writer must refuse `Unsupported` content before touching a
  destination.
- Loss-aware export remains a separate user intent from direct save.
- The capability list can grow only with fixtures and adapter evidence.
- This contract does not modify the accepted Alpha 4 schemas or production
  writers.

## Alternatives rejected

- Use logical validation for format limits: makes the oldest format the product
  model.
- Let writers silently drop unsupported fields: creates undetectable data loss.
- Treat every normalization as loss: obscures safe canonical transformations.
- Infer capabilities from a filename extension: extensions are hints, not
  format or behavior evidence.
