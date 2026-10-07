# Architecture overview

TodoApp is a **modular monolith**: one deployable host (`TodoApp.Api`) composing
independent modules, designed so any module can later be extracted into its own service
with minimal change (see [ADR 0001](../decisions/0001-modular-monolith.md)).

## C4 — Level 1: System context

```mermaid
C4Context
  title TodoApp — System Context
  Person(user, "User", "Creates and shares todo lists")
  Person(admin, "Admin", "Monitors system health")
  System(todoapp, "TodoApp", "Todo management with auth, sharing, reminders")
  System_Ext(entra, "Microsoft Entra ID", "OIDC single sign-on")
  System_Ext(email, "Email provider", "Confirmation, reset, reminders")
  Rel(user, todoapp, "Uses", "HTTPS / BFF cookies")
  Rel(admin, todoapp, "Monitors", "HTTPS")
  Rel(todoapp, entra, "Delegates SSO", "OIDC")
  Rel(todoapp, email, "Sends mail", "SMTP/API")
```

## C4 — Level 2: Containers

```mermaid
C4Container
  title TodoApp — Containers
  Person(user, "User")
  Container(web, "Web SPA", "React + TypeScript + Vite", "UI; talks to BFF only")
  Container(mobile, "Mobile app", "Flutter", "Bearer-token client")
  Container(api, "TodoApp.Api", ".NET 10 host", "BFF, modules, SignalR hub, middleware")
  Container(fn, "TodoApp.Functions", ".NET 10 isolated", "Reminders, event processing")
  ContainerDb(sql, "Azure SQL", "SQL Server", "One DB, one schema per module")
  Container(signalr, "Azure SignalR", "Service", "Real-time fan-out")
  Container(sb, "Service Bus", "Queue/Topic", "Async integration events")
  Container(kv, "Key Vault", "Secrets/certs")
  Container(ai, "App Insights", "Telemetry")
  Rel(user, web, "Uses", "HTTPS")
  Rel(web, api, "XHR", "HTTPS + cookies + antiforgery")
  Rel(mobile, api, "REST", "HTTPS + bearer")
  Rel(api, sql, "EF Core", "TDS")
  Rel(api, signalr, "Negotiates/pushes")
  Rel(api, sb, "Publishes events (outbox)")
  Rel(fn, sb, "Consumes events")
  Rel(fn, sql, "Reads/writes notifications schema")
  Rel(api, kv, "Reads secrets", "Managed Identity")
  Rel(api, ai, "Traces/metrics/logs", "OTLP")
  Rel(fn, ai, "Traces", "OTLP")
```

## Request flow (web, BFF)
1. Browser sends XHR to `TodoApp.Api` with the auth cookie + antiforgery token.
2. Correlation-id middleware assigns/propagates `X-Correlation-Id` (Phase 4).
3. Auth middleware resolves the user from the cookie; policies authorize.
4. The matched module endpoint executes application logic; EF Core hits its schema.
5. Side effects enqueue integration events to the module **outbox** (same transaction).
6. The outbox dispatches events (in-process now, Service Bus by config) → consumers / SignalR.
7. Response returns with `X-Correlation-Id`; telemetry is emitted via OpenTelemetry.

## Cross-cutting
- **BuildingBlocks** supplies `Result`, event base types, the outbox, and observability helpers.
- **Boundaries are enforced** by `tests/TodoApp.ArchitectureTests` (fails the build on violation).
- Modules: see [modules.md](./modules.md). Auth: see [../auth/auth-flows.md](../auth/auth-flows.md).
  Observability: see [../observability/monitoring.md](../observability/monitoring.md).
