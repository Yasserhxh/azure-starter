# Build my golden starter template: "TodoApp Starter"

You are building my reusable **starter template**. Every future project will be created from it with `dotnet new`, so quality matters more than speed.
The demo domain is a Todo app, but the real product is the **foundation**: auth, security, TDD, observability, monitoring, Azure, brand tokens, docs, and Claude Code setup.

Follow my global `~/.claude/CLAUDE.md`, `~/.claude/rules/*`, and skills (`tdd-workflow`, `security-review`, `expert-ui-design`, `api-design`, `backend-patterns`, `frontend-patterns`, `database-migrations`). Don't repeat their content in project files; reference them.

## How to work
- Work in the **phases** below. At the end of each phase: all tests green, build clean (zero warnings), run a fresh-context code review, update the docs that the phase touched, give me a 5-line summary, suggest a commit message, and **STOP for my approval**.
- Suggest `/compact` between phases to keep context low.
- Before choosing versions or APIs, check current docs with the `microsoft-learn` MCP. Use the **current .NET LTS**, current stable packages, and central package management.
- When a decision is hard to reverse, ask me first. Record every architecture decision as an ADR in `docs/decisions/`.

## Architecture: modular monolith
- **One deployable host** (`TodoApp.Api`) composes independent **modules**. Starting modules: `Identity` (users, auth), `Todos` (lists, todos, sharing), `Notifications` (due-date reminders), `Monitoring` (admin observability APIs).
- **Each module** has its own folder with `Domain`, `Application`, `Infrastructure`, `Endpoints` (vertical slices inside are fine), plus a small public `*.Contracts` project. Everything except Contracts is `internal`; use `InternalsVisibleTo` only for that module's tests.
- **Data:** one Azure SQL database, but **one schema and one DbContext per module** (`identity.*`, `todos.*`, `notifications.*`), with migrations per module. No cross-module joins, no cross-module foreign keys. Other modules reference users by `UserId` only.
- **Communication:** synchronous calls only through the other module's `Contracts` (interfaces/queries); asynchronous through **integration events** with a transactional **outbox** per module. Dispatch in-process at first, with a switch to Azure Service Bus by configuration.
- **Shared kernel:** a small `BuildingBlocks` (Result type, domain/integration event base, outbox, correlation/observability helpers). No business logic in it.
- **Enforced, not just conventions:** architecture tests (NetArchTest or ArchUnitNET) fail the build if a module references another module's internals, if Domain references Infrastructure, or if anything references a module except through Contracts.
- Each module registers itself through one `AddXxxModule()` / `MapXxxModule()` pair, so the host stays a thin composition root.
- Record this as ADR 0001, including how to extract a module into its own service later.

---

## Phase 0: Plan (no code)
1. Confirm the current .NET LTS and the key package versions via `microsoft-learn`.
2. Propose the final folder tree, the module boundaries (what each module owns, its contracts, and the events it publishes/consumes), the auth architecture, and the template parameters (see Phase 8).
3. Ask me these questions in one message:
   - React only, or React + a Flutter mobile client (Phase 9)?
   - Entra ID single-tenant or multi-tenant by default?
   - Confirm the BFF pattern (cookies) for React and bearer tokens for mobile.
4. Wait for my answers.

## Phase 1: Skeleton, Claude setup, and docs
Create this structure (adjust only with a reason):

```
/
├── CLAUDE.md                    # project-level, ≤ 60 lines (spec below)
├── README.md                    # run, test, configure secrets, deploy, open monitoring, create a project from the template
├── CHANGELOG.md
├── .claude/
│   ├── settings.json            # project permissions (build/test/lint allowed; secrets denied)
│   └── rules/
│       ├── auth.md              # project auth rules (paths: auth-related files)
│       └── docs-sync.md         # which doc to update when what changes
├── docs/
│   ├── brand/
│   │   ├── README.md            # how to drop a charte graphique here and regenerate tokens
│   │   ├── charte/              # put brand PDFs/images here (gitkeep)
│   │   ├── brand-tokens.md      # human-readable tokens (placeholder brand for now)
│   │   └── design-tokens.json   # W3C Design Tokens format: single source of truth
│   ├── architecture/
│   │   ├── overview.md          # C4-style context + containers, request flow
│   │   └── modules.md           # each module: responsibility, contracts, events, schema, owner
│   ├── auth/auth-flows.md       # every login flow, with sequence diagrams (Mermaid)
│   ├── observability/monitoring.md  # logs, traces, metrics, alerts, monitoring page
│   ├── decisions/0000-template.md   # ADR template + first ADRs
│   └── runbooks/                # incident steps: login failures, DB down, SignalR issues
├── src/
│   ├── TodoApp.Api/                     # host: composition root, BFF, SignalR hub, middleware
│   ├── BuildingBlocks/                  # shared kernel: Result, events, outbox, observability
│   ├── Modules/
│   │   ├── Identity/
│   │   │   ├── TodoApp.Modules.Identity/            # Domain, Application, Infrastructure, Endpoints (internal)
│   │   │   └── TodoApp.Modules.Identity.Contracts/  # public API of the module
│   │   ├── Todos/          (same layout)
│   │   ├── Notifications/  (same layout)
│   │   └── Monitoring/     (same layout)
│   ├── TodoApp.Functions/               # Azure Functions (isolated) using module Contracts
│   └── web/                             # React + TypeScript + Vite
├── tests/
│   ├── Modules/<Module>.Tests/          # unit tests per module
│   ├── TodoApp.ArchitectureTests/       # enforces module boundaries
│   ├── TodoApp.Api.IntegrationTests/    # WebApplicationFactory + Testcontainers SQL Server
│   ├── TodoApp.Functions.Tests/
│   └── e2e/                             # Playwright
├── infra/                       # Bicep: main.bicep, modules/, env params, scripts/
├── tools/tokens/                # script: design-tokens.json → CSS variables (+ Flutter theme)
├── .github/workflows/           # ci.yml, deploy.yml
├── .template.config/            # dotnet new template config (Phase 8)
├── Directory.Build.props        # nullable, warnings as errors, analyzers
├── Directory.Packages.props     # central package management
├── global.json
└── .editorconfig
```

**Project CLAUDE.md spec (≤ 60 lines):**
- What the app is (2 lines) and the stack.
- Commands: build, test (unit / integration / web / e2e), run locally (API + web + Azurite + SQL container + Aspire Dashboard), regenerate tokens, add a migration.
- A structure map: one line per folder, plus the module rules in 4 lines (contracts only, own schema, events via outbox, architecture tests must pass) and a pointer to `docs/architecture/modules.md`.
- Auth summary in 3 lines, with a pointer to `docs/auth/auth-flows.md`.
- Brand: "Brand tokens live in `docs/brand/`. If `docs/brand/charte/` contains a charter, it overrides all design defaults (see the expert-ui-design skill). Never hardcode colors; regenerate tokens."
- A **"Docs to keep in sync"** table: when X changes → update Y (auth → auth-flows.md; new/changed module, contract, or event → modules.md; new endpoint → API docs; new resource → infra + overview; new alert → monitoring.md; decision → ADR; release → CHANGELOG).
- Project-specific rules only. Don't repeat global rules.

## Phase 2: BuildingBlocks + Todos module (strict TDD)
Start with `BuildingBlocks` and the architecture tests (they must exist and pass before any module code), then build the Todos module.
Todo features: lists, todos (title, notes, due date, priority, completed), per-user ownership, optional sharing of a list with another user (read/edit), audit fields (UTC).
- Follow the `tdd-workflow` skill: show the failing tests before each implementation.
- Rich domain model with invariants, value objects where meaningful, and a result pattern or domain exceptions (pick one and record it in an ADR).

## Phase 3: Identity module: authentication & authorization (most important)
Support **multiple auth methods linked to one user account**:
1. **ASP.NET Core Identity** local accounts: email + password, email confirmation, lockout, password policy, TOTP 2FA with recovery codes, password reset. Add **passkeys** if the current .NET LTS supports them in Identity (verify first).
2. **Microsoft Entra ID SSO** (OIDC) as an external login, linked to the Identity user (auto-provision or link-on-first-login; ask me which). Map Entra app roles/groups to app roles.
3. **React:** BFF pattern. The API handles OIDC, the browser only gets secure httpOnly SameSite cookies, with antiforgery protection.
4. **Mobile/API clients:** bearer tokens (Identity API endpoints or Entra tokens; propose one).
5. Roles: `User`, `Admin`. Policies, not role-string checks. Resource-based authorization: a user can only access their own lists or lists shared with them.
6. Entra app registration: a script in `infra/scripts/` (az CLI or Microsoft Graph Bicep). Prefer **secretless** auth (certificate or managed identity as federated credential) and verify the current options. Any unavoidable secret goes in Key Vault.

Mandatory tests: login flows, lockout, 2FA, external login linking, **IDOR tests** (user A can never read or modify user B's data), admin-only endpoints, antiforgery, and rate limiting on auth endpoints. Run the `security-review` skill on this phase. Document every flow in `docs/auth/auth-flows.md` with Mermaid sequence diagrams.

## Phase 4: Observability baseline + monitoring page
Implement everything from my global "New projects from scratch: mandatory baseline": Serilog (SQL Server sink + OTel), correlation ID middleware, OpenTelemetry (ASP.NET Core, HttpClient, SqlClient/EF Core), `IExceptionHandler` + ProblemDetails, `/health/live` + `/health/ready`, custom metrics (todos created/completed, login success/failure by method), `POST /api/client-logs`, and admin monitoring APIs. Follow my logging policy: signal, not noise.

## Phase 5: React frontend (expert design)
- Use the `expert-ui-design` skill. Use a **placeholder brand** defined only in `docs/brand/design-tokens.json`.
- `tools/tokens` generates `src/web/src/styles/tokens.css` (light/dark modes) from the JSON. **Components never hardcode colors, fonts, or spacing**, so swapping a brand means editing the tokens and regenerating.
- Pages: sign-in (local + "Sign in with Microsoft" + 2FA step), lists, todo board, sharing, account security (2FA, linked logins, passkeys), and an **admin monitoring page** (health, error rate, latency, recent errors with correlation-ID search, live SignalR connections).
- Error boundaries, a global error reporter, an API client with a correlation ID, accessibility, and responsive down to 360 px.
- Tests: Vitest + RTL + MSW, plus Playwright e2e for sign-in → create todo → complete → sign-out.

## Phase 6: Real-time + background work
- **Azure SignalR Service** (Default mode): live updates when a shared list changes. Authorized hub, user identity from claims, group membership checked server-side.
- **Notifications module + Azure Functions** (isolated, Flex Consumption): the Todos module publishes integration events through its outbox; Notifications consumes them; a timer trigger sends due-date reminders via a Service Bus queue. Idempotent, with dead-letter handling and telemetry correlated with the API.
- Tests for both.

## Phase 7: Azure infrastructure + CI/CD
- Bicep modules: App Service (staging slot) or Container Apps (ask), Azure SQL (Entra-only auth), SignalR Service, Function App (Flex Consumption), Service Bus, Storage, Key Vault, App Configuration, Application Insights + Log Analytics, alerts and action group, budget. Managed Identity + RBAC everywhere; private endpoints as a prod parameter.
- `ci.yml`: build, unit + integration + web tests, analyzers, vulnerability scan, Bicep lint.
- `deploy.yml`: OIDC federated login, `what-if`, deploy to staging slot, smoke test `/health/ready`, swap.

## Phase 8: Turn it into a `dotnet new` template
- `.template.config/template.json`: `sourceName: "TodoApp"` (renames namespaces, projects, and folders), short name `azure-starter`, and parameters such as `--include-functions`, `--include-signalr`, `--include-mobile`, `--entra-tenant-mode`.
- Keep the Todo domain as a working sample, removable with `--no-sample` if feasible (propose how).
- Add a second **item template** `azure-module`: `dotnet new azure-module -n Billing` creates a new module with the standard layout, Contracts project, its own schema/DbContext, registration methods, a test project, and an entry in `docs/architecture/modules.md`. The architecture tests must pick it up automatically.
- **Verify:** install the template locally, create a project in a temp folder with a different name, then build, run all tests, and confirm the project CLAUDE.md, `.claude/`, and docs were renamed correctly. Fix anything that fails.
- README section: "Create a new project from this template" with exact commands, and "Apply a client's charte graphique" in steps.

## Phase 9 (only if I said yes in Phase 0): Flutter client
Feature-first structure, theme generated from the same `design-tokens.json`, sign-in (local + Entra via the chosen token flow), lists/todos, SignalR live updates, a global error handler + reporter with correlation ID, and unit/widget/integration tests.

---

**Start with Phase 0 now.**
