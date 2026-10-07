# CI/CD workflows (Phase 7)

- `ci.yml` — build, unit + integration + web tests, analyzers, dependency
  vulnerability scan, Bicep lint. Runs on every PR.
- `deploy.yml` — OIDC federated login, `what-if`, deploy to staging slot,
  smoke-test `/health/ready`, swap.
