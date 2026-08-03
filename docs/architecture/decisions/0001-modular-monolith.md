# ADR 0001: Use a modular monolith

- Status: Accepted
- Date: 2026-08-04

## Context

C3 is a small offline-first Windows desktop product with a mature VB.NET
WinForms codebase, a public XML catalogue format, and a strong backwards-
compatibility requirement. The current application mixes UI state, catalogue
rules, persistence, diagnostics, and external services in forms and a global
module.

Splitting every concern into its own project would increase build and navigation
cost without adding meaningful isolation. Keeping everything in one executable
would leave the important dependency boundaries unenforced.

## Decision

Use three physical production modules and four build projects:

1. `C3.Catalogue`, a framework-4.0-compatible domain library.
2. `C3.Infrastructure`, a framework-4.0-compatible adapter library.
3. `C3.WinForms`, one physical UI source tree compiled by Net40/x86 and
   Net48/x64 project files.

Organize projects by dependency boundary and folders by product feature.

## Consequences

- Core catalogue behavior is testable without Windows UI automation.
- XML/DataSet migration is isolated behind a store contract.
- Both Windows lanes share one implementation and one file format.
- Future native clients can reuse specifications and fixtures, not implementation
  binaries.
- Project files require a parity check because two projects enumerate the same
  old-style WinForms source tree.
- Some legacy code will temporarily remain in the UI module while it is migrated
  feature by feature. That is preferable to a big-bang rewrite.

## Rejected alternatives

- One project with platform-conditional target frameworks: fragile for old
  WinForms designers, generated resources, settings, and configuration.
- A project per feature or concern: excessive ceremony and cross-project churn.
- A catch-all `Core` or `Platform` module: ownership becomes ambiguous over time.
- A complete rewrite: insufficient behavioral coverage and unnecessary format
  compatibility risk.

