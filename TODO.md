# C3 work tracking

The canonical C3 2.0 dependency-ordered checklist is
[`docs/planning/2.0-execution-plan.md`](docs/planning/2.0-execution-plan.md).
Public milestone outcomes are summarized in [`ROADMAP.md`](ROADMAP.md).
Durable automation progress and the current milestone pointer live in
[`release/train/2.0.0.json`](release/train/2.0.0.json); C/E/P and package facts
remain in [`release/catalog.v1.json`](release/catalog.v1.json). Neither file is a
second task list.

Individual defects and assignments belong in GitHub Issues or, after its
integration contract is exercised, AIDE work units. This file intentionally does
not duplicate those lists.

The accepted post-Alpha-5 architecture is
[`docs/architecture/catalogue-and-application.md`](docs/architecture/catalogue-and-application.md)
and [ADR 0012](docs/architecture/decisions/0012-canonical-catalogue-before-application-frontends.md).
Alpha 6 begins with complete canonical shadow/round-trip convergence; it does
not begin Application, frontend, or partial canonical production mutation work.
