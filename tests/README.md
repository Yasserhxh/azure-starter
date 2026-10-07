# tests

| Project | Purpose | Phase |
|---|---|---|
| `Modules/<Module>.Tests` | Unit tests per module (domain + application) | 2+ |
| `TodoApp.ArchitectureTests` | Enforce module boundaries (ArchUnitNET) — must exist/pass before any module code | 2 |
| `TodoApp.Api.IntegrationTests` | WebApplicationFactory + Testcontainers SQL Server | 2+ |
| `TodoApp.Functions.Tests` | Functions unit/integration tests | 6 |
| `TodoApp.TestCommon` | Shared fixtures (SQL container, WebAppFactory, builders) | 2 |
| `e2e` | Playwright end-to-end (sign-in → create → complete → sign-out) | 5 |
