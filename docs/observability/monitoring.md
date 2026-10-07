# Observability & monitoring

Implements the mandatory baseline from `~/.claude/CLAUDE.md`. Built in **Phase 4**;
specified here. Policy: **signal, not noise** — rely on OpenTelemetry auto-instrumentation,
hand-write logs only for business events, errors, and important state changes.

## Logging
- **Serilog** with structured message templates (no string interpolation in log calls).
- Sinks: console (dev), **SQL Server** sink, and OpenTelemetry. Async/batched; sampling in
  prod (configurable ratio, always keep errors). Log level changeable at runtime.
- Levels: `Information` = business events, `Warning` = recoverable anomalies,
  `Error` = failures needing attention, `Debug` = dev only.
- **Never** log secrets, tokens, passwords, recovery codes, or PII.

## Correlation
- Middleware assigns/propagates a correlation id, flows it to logs, traces, DB, queues,
  and downstream HTTP, and returns it as `X-Correlation-Id` on every response.

## Tracing (OpenTelemetry)
- Auto-instrumentation: inbound ASP.NET Core, outbound `HttpClient`, EF Core / `SqlClient`.
- Dev exporter: **Aspire Dashboard** (OTLP). Prod: **Azure Monitor OpenTelemetry Distro**
  → Application Insights.

## Metrics
- Request rate, latency p50/p95/p99, error rate, DB call duration.
- Business counters: todos created / completed, login success/failure **by method**.

## Error handling
- `IExceptionHandler` + **RFC 7807 ProblemDetails**. Errors logged once at the boundary
  with full context; clients never see stack traces, SQL, or internal paths.
- Frontend (React/Flutter) captures unhandled errors and posts to `POST /api/client-logs`
  with the correlation id.

## Health
- `/health/live` — process up. `/health/ready` — DB + critical dependencies.

## Monitoring page (admin-only)
Health status, error rate & latency, recent errors with **correlation-id search**, live
SignalR connection count, and links to the trace backend (Aspire dashboard in dev, App
Insights in prod). Served by the **Monitoring** module.

## Alerts (Phase 7)
Azure Monitor alerts + action group on error-rate spike, latency breach, failed health
checks, and dead-letter growth. Add new alerts here when created.
