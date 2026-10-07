# Runbooks

Step-by-step incident response. Each runbook: **symptom → diagnosis → mitigation →
verification → follow-up**. Use the correlation id to pivot across logs, traces, and the
monitoring page.

Planned (filled out as the relevant phase lands):
- `login-failures.md` — spike in failed logins / lockouts / Entra callback errors (Phase 3).
- `database-down.md` — Azure SQL unreachable or failing `/health/ready` (Phase 4/7).
- `signalr-issues.md` — real-time updates not delivered, negotiate failures (Phase 6).
- `reminders-not-sent.md` — Functions/Service Bus backlog or dead-letter growth (Phase 6).

Template:

```markdown
# <Incident>
## Symptom
## Detect (dashboards, alerts, queries)
## Diagnose (likely causes, how to confirm)
## Mitigate (immediate actions, rollback)
## Verify (how you know it's resolved)
## Follow-up (ADR, postmortem, prevention)
```
