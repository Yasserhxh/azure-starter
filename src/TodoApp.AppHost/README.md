# TodoApp.AppHost (Aspire)

Local dev orchestration (Phase 4, [ADR 0005](../../docs/decisions/0005-aspire-dev-orchestration.md)).
`aspire run` brings up the API + web + SQL Server container + Azurite and launches the
**Aspire Dashboard** (OTLP telemetry). Dev-time only — production hosting is plain Azure (Phase 7).
