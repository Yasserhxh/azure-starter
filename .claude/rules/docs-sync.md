---
description: Which documentation to update when code changes. Apply on every change that alters behavior, structure, contracts, or infrastructure.
paths:
  - src/**
  - infra/**
  - tools/**
  - .github/**
---

# Docs to keep in sync

Documentation is part of "done". When you change the left column, update the right
column **in the same change** before saying a task is complete.

| When you change… | Update… |
|---|---|
| An auth flow (login, 2FA, passkey, external login, BFF) | `docs/auth/auth-flows.md` (+ Mermaid) and re-run `security-review` |
| A module's responsibility, contract, or published/consumed event | `docs/architecture/modules.md` |
| A module's schema or DbContext | `docs/architecture/modules.md` + add an EF migration |
| A new or changed HTTP endpoint | API docs / OpenAPI + the module's section in `modules.md` |
| A new Azure resource or topology | `infra/` Bicep + `docs/architecture/overview.md` |
| A new metric, log event, trace, or alert | `docs/observability/monitoring.md` |
| Brand / design tokens | edit `docs/brand/design-tokens.json`, regenerate tokens (`tools/tokens`) — never hardcode |
| An architecturally significant or hard-to-reverse decision | add an ADR in `docs/decisions/` |
| Anything user-visible or released | `CHANGELOG.md` (Keep a Changelog format) |
| A new runbook-worthy failure mode | add/update `docs/runbooks/` |

If a change touches something with no obvious doc home, add a note and flag it rather
than leaving it undocumented.
