# TodoApp — project guide

Todo app (lists, todos, sharing, due-date reminders) built as the reusable **Azure starter**.
Stack: .NET 10 / C# 14, ASP.NET Core modular monolith, EF Core + Azure SQL, React+TS+Vite,
Flutter, Azure (Functions, SignalR, Service Bus, Key Vault, App Insights), Bicep.

> Global rules in `~/.claude/CLAUDE.md` and skills apply and are **not** repeated here.
> Being built in phases (see `template-prompt.md`); some commands land in a later phase.

## Commands
- Build: `dotnet build TodoApp.slnx -warnaserror`
- Unit tests: `dotnet test` · Integration: `dotnet test tests/TodoApp.Api.IntegrationTests` (needs Docker)
- Architecture tests: `dotnet test tests/TodoApp.ArchitectureTests`
- Web: `npm --prefix src/web run dev` · `npm --prefix src/web test` (Phase 5)
- E2E: `npm --prefix tests/e2e run test:e2e` (Playwright, Phase 5)
- Run locally (API + web + SQL + Azurite + Aspire/OTLP dashboard): `aspire run` (Phase 4)
- API only: `dotnet run --project src/TodoApp.Api`
- Regenerate design tokens: `node tools/tokens/build.mjs` (Phase 5)
- Add a migration: `dotnet ef migrations add <Name> --project src/Modules/<M>/TodoApp.Modules.<M>` (Phase 2+)

## Structure
- `src/TodoApp.Api` — host: composition root, BFF, SignalR, middleware
- `src/BuildingBlocks` — shared kernel (Result, events, outbox, observability); no business logic
- `src/Modules/<M>` — Identity, Todos, Notifications, Monitoring (see below)
- `src/TodoApp.Functions` — Azure Functions (isolated) · `src/web` — React · `src/TodoApp.AppHost`/`ServiceDefaults` — Aspire
- `tests/` — module unit, ArchitectureTests, Api.IntegrationTests, Functions.Tests, e2e, TestCommon
- `infra/` — Bicep · `tools/tokens` — token build · `docs/` — architecture, auth, brand, observability, decisions, runbooks

### Module rules (enforced by `tests/TodoApp.ArchitectureTests`)
1. Reference other modules **only** through their `*.Contracts` project.
2. Each module owns one schema + one DbContext; no cross-module joins or FKs (reference by `UserId`).
3. Async between modules goes through the module's transactional **outbox**, never direct calls.
4. `Domain` never references `Infrastructure`; architecture tests must pass before any module code.

Details: `docs/architecture/modules.md` and `docs/architecture/overview.md`.

## Auth
- React = **BFF** (httpOnly/SameSite cookies + antiforgery). Mobile/API = **bearer tokens**.
- Identity local accounts + passkeys + TOTP 2FA; **Entra ID single-tenant** linked login.
- Policy- and resource-based authz (own-or-shared); never role-string checks. See `docs/auth/auth-flows.md`.

## Brand
Brand tokens live in `docs/brand/`. If `docs/brand/charte/` contains a charter, it **overrides
all design defaults** (see the `expert-ui-design` skill). Never hardcode colors/fonts/spacing —
regenerate tokens from `docs/brand/design-tokens.json`.

## Docs to keep in sync (authority: `.claude/rules/docs-sync.md`)
| Change | Update |
|---|---|
| Auth flow | `docs/auth/auth-flows.md` + re-run `security-review` |
| Module / contract / event | `docs/architecture/modules.md` |
| New endpoint | API/OpenAPI docs |
| New Azure resource | `infra/` + `docs/architecture/overview.md` |
| New alert/metric | `docs/observability/monitoring.md` |
| Significant decision | ADR in `docs/decisions/` |
| Release | `CHANGELOG.md` |
