# Modules

Each module owns its data (one schema + one DbContext), exposes a public `*.Contracts`
project, and communicates with other modules **only** through Contracts (sync) or
integration events via its outbox (async). No cross-module joins or foreign keys;
other modules reference users by `UserId` only. Boundaries are enforced by
`tests/TodoApp.ArchitectureTests`.

> Keep this file in sync when a module's responsibility, contracts, events, or schema
> change (see `.claude/rules/docs-sync.md`).

## Identity — *Phase 3*
- **Responsibility:** users, local accounts, passkeys, TOTP 2FA, external (Entra) logins, roles.
- **Schema:** `identity.*`
- **Contracts:** `IUserDirectory` (look up a user by id/email), `ICurrentUser`.
- **Publishes:** `UserRegistered`, `UserDeactivated`.
- **Consumes:** —
- **Owner:** _TBD_

## Todos — *Phase 2*
- **Responsibility:** lists, todos (title, notes, due date, priority, completed), per-user
  ownership, optional list sharing (read/edit), UTC audit fields.
- **Schema:** `todos.*`
- **Contracts:** `ITodoListQueries` (read-only projections for other modules).
- **Publishes:** `TodoListShared`, `TodoCreated`, `TodoCompleted`, `TodoDueDateChanged`.
- **Consumes:** `UserDeactivated` (cascade cleanup).
- **Owner:** _TBD_

## Notifications — *Phase 6*
- **Responsibility:** due-date reminder scheduling and delivery log.
- **Schema:** `notifications.*`
- **Contracts:** `INotificationQueries`.
- **Publishes:** `ReminderSent`.
- **Consumes:** `TodoDueDateChanged`, `TodoListShared`, `TodoCompleted`.
- **Owner:** _TBD_

## Monitoring — *Phase 4*
- **Responsibility:** admin observability APIs (health, error rate, latency, recent errors
  by correlation id, live SignalR connections). Reads telemetry; owns no business data.
- **Schema:** none.
- **Contracts:** `IObservabilitySnapshot`.
- **Publishes / Consumes:** — (observes only).
- **Owner:** _TBD_

---

## Adding a module
`dotnet new azure-module -n <Name>` (Phase 8) scaffolds the standard layout, Contracts
project, schema/DbContext, `Add<Name>Module()`/`Map<Name>Module()`, a test project, and
an entry here. The architecture tests pick it up automatically.
