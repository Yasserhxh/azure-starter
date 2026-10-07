# ADR 0005: Aspire for local dev orchestration and the OTLP dashboard

- **Status:** accepted
- **Date:** 2026-10-07
- **Deciders:** Yasser BOUAABANE

## Context and problem statement
The global baseline requires an OTLP dashboard in dev and a one-command local loop that
brings up the API, web, a SQL container, and Azurite. We want the dev wiring for
OpenTelemetry and health checks centralized rather than duplicated per project.

## Decision drivers
- Single-command local run with all dependencies and a telemetry dashboard.
- Idiomatic .NET 10 approach; minimal bespoke scripting.
- Dev-only concern that must not leak into production hosting.

## Considered options
1. **docker-compose + hand-rolled OTLP collector** — flexible but bespoke, more to maintain.
2. **Aspire AppHost + ServiceDefaults** — first-party orchestration, built-in OTLP dashboard,
   shared OpenTelemetry/health defaults.

## Decision
Add `src/TodoApp.AppHost` (Aspire AppHost) and `src/TodoApp.ServiceDefaults`. The AppHost
orchestrates API + web + SQL Server container + Azurite and launches the **Aspire Dashboard**
(persistent OTLP telemetry, Aspire 13.6). `ServiceDefaults` centralizes the OpenTelemetry
(traces/metrics/logs) and health-check wiring referenced by the host. This is a **dev-time**
concern: production telemetry goes to Application Insights via the Azure Monitor
OpenTelemetry Distro (ADR-adjacent, Phase 4), and production hosting is plain Azure (Phase 7),
not the AppHost.

> Implemented in **Phase 4** (observability), where `ServiceDefaults` gains its OTel content.
> Recorded now because it shapes the folder layout and the local-run commands in `CLAUDE.md`.

## Consequences
- **Positive:** one-command dev loop; OTLP dashboard out of the box; no duplicated telemetry
  wiring; standard, well-documented tooling.
- **Negative / trade-offs:** adds Aspire packages and two projects; contributors need the
  Aspire workload/CLI for `aspire run` (plain `dotnet run` on the API still works).
