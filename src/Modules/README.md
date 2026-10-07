# Modules

Each module is an independent vertical slice with its own folder containing
`Domain`, `Application`, `Infrastructure`, `Endpoints` (all `internal`) plus a
small public `TodoApp.Modules.<Name>.Contracts` project.

Layout per module:

```
<Name>/
├── TodoApp.Modules.<Name>/            # internal: Domain, Application, Infrastructure, Endpoints
└── TodoApp.Modules.<Name>.Contracts/  # public API: interfaces, queries, integration-event contracts
```

Rules (enforced by `tests/TodoApp.ArchitectureTests`):
- Other modules reference this module **only** through its `.Contracts`.
- `Domain` never references `Infrastructure`.
- Own schema + DbContext (`<name>.*`); no cross-module joins or FKs — reference users by `UserId`.
- Async communication via integration events through the module's outbox.
- One `Add<Name>Module()` / `Map<Name>Module()` pair registers the module in the host.

Starter modules: **Identity** (Phase 3), **Todos** (Phase 2), **Notifications** (Phase 6),
**Monitoring** (Phase 4). New modules are scaffolded with `dotnet new azure-module -n <Name>`
(Phase 8). See `docs/architecture/modules.md`.
