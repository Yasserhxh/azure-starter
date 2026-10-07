# infra

Bicep infrastructure-as-code (Phase 7). Managed Identity + RBAC everywhere;
Key Vault for any unavoidable secret; private endpoints as a prod parameter.

```
infra/
├── main.bicep          # subscription/resource-group composition
├── modules/            # one Bicep module per Azure resource
├── env/                # per-environment parameter files (dev, staging, prod)
└── scripts/            # Entra app registration, helpers (Phase 3/7)
```

Resources: App Service or Container Apps (decision deferred to Phase 7), Azure SQL
(Entra-only auth), SignalR Service, Function App (Flex Consumption), Service Bus,
Storage, Key Vault, App Configuration, App Insights + Log Analytics, alerts, budget.
