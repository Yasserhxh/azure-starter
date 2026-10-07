# TodoApp — Azure starter template

A production-grade **`dotnet new` starter template** delivered as a working Todo app. The
real product is the foundation: modular-monolith architecture, auth & security, TDD,
observability, Azure infra, brand tokens, docs, and Claude Code setup — all from the first
commit. The Todo domain is a removable sample.

> **Status:** built in phases (see `template-prompt.md`). This README documents the target;
> a section is marked with the phase that wires it up where it isn't functional yet.

- Stack: **.NET 10 / C# 14**, ASP.NET Core modular monolith, EF Core + Azure SQL, React + TypeScript + Vite,
  Flutter, Azure (Functions, SignalR Service, Service Bus, Key Vault, App Insights), Bicep.
- Architecture: `docs/architecture/overview.md` · Decisions: `docs/decisions/`

## Prerequisites
- .NET SDK **10.0.301+** (pinned in `global.json`)
- Docker Desktop (integration tests + local SQL/Azurite)
- Node.js 20+ and npm (web, Phase 5)
- Aspire CLI/workload for `aspire run` (Phase 4); Azure CLI + Bicep for deploy (Phase 7)

## Run
```bash
# API only (works today)
dotnet run --project src/TodoApp.Api        # http://localhost:5080

# Full local stack: API + web + SQL + Azurite + Aspire/OTLP dashboard  (Phase 4)
aspire run
```

## Test
```bash
dotnet build TodoApp.slnx -warnaserror      # zero-warning gate
dotnet test                                 # all .NET tests
dotnet test tests/TodoApp.ArchitectureTests # module-boundary rules
dotnet test tests/TodoApp.Api.IntegrationTests   # WebAppFactory + Testcontainers SQL (Docker)
npm --prefix src/web test                   # Vitest + RTL (Phase 5)
npm --prefix tests/e2e run test:e2e         # Playwright (Phase 5)
```

## Configure secrets
- **Never commit secrets.** Dev: .NET user-secrets (`dotnet user-secrets set <Key> <Value>
  --project src/TodoApp.Api`) or a gitignored `.env` (an `.env.example` is added in Phase 3).
- Prod: **Azure Key Vault** via Managed Identity; prefer secretless Entra (federated credential).
- Reading `.env`/`secrets.json`/`*.Local.json`/`*.pfx`/`*.pem` is denied in `.claude/settings.json`.

## Deploy (Phase 7)
GitHub Actions `deploy.yml`: OIDC federated login → `what-if` → deploy to the staging slot →
smoke-test `/health/ready` → swap. Infra is Bicep in `infra/` (Managed Identity + RBAC).

## Open the monitoring page
Admin-only page: health, error rate, latency, recent errors with correlation-id search, live
SignalR connections, and a link to the trace backend (Aspire dashboard in dev, App Insights in
prod). Served by the Monitoring module (Phase 4). See `docs/observability/monitoring.md`.

## Create a new project from this template (Phase 8)
```bash
dotnet new install ./                                  # install from this repo
dotnet new azure-starter -n Acme.Orders \
  --include-functions true --include-signalr true \
  --include-mobile true --entra-tenant-mode single
# add a module later:
dotnet new azure-module -n Billing
```
`sourceName` renames namespaces, projects, and folders; `--no-sample` strips the Todo demo.

## Apply a client's charte graphique
1. Drop the brand assets in `docs/brand/charte/`.
2. Extract palette/type/spacing with the `expert-ui-design` skill (the charte overrides all defaults).
3. Update `docs/brand/design-tokens.json`, regenerate (`node tools/tokens/build.mjs`), rebuild.
   No component code changes. Full steps: `docs/brand/README.md`.

## License
TBD.
