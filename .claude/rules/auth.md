---
description: Project auth rules. Apply when touching auth-related files.
paths:
  - src/Modules/Identity/**
  - src/TodoApp.Modules.Identity*/**
  - src/TodoApp.Api/**/Auth/**
  - src/TodoApp.Api/Program.cs
  - infra/scripts/**entra**
  - docs/auth/**
---

# Project auth rules

Global security rules (`~/.claude/CLAUDE.md`) and the `security-review` skill still
apply. This file adds **project-specific** decisions only.

## Transport
- **React = BFF.** The API performs OIDC; the browser receives only `HttpOnly`,
  `Secure`, `SameSite=Lax` (or `Strict`) cookies. No tokens in JS/localStorage.
- **Mobile / API clients = bearer tokens.** Never set auth cookies for them.
- All state-changing BFF requests require **antiforgery** (double-submit / header token).

## Identity
- ASP.NET Core Identity local accounts + **native .NET 10 passkeys** + TOTP 2FA with
  recovery codes. Email confirmation, lockout, and password policy are on by default.
- **Entra ID = single-tenant** external login linked to the local Identity user.
  (Provision-vs-link policy is decided in Phase 3 and recorded as an ADR.)
- Multiple auth methods map to **one** user account.

## Authorization
- Policy-based only. **Never** check role strings inline (`User.IsInRole("Admin")`).
  Define policies (`RequireAdmin`, resource handlers) and use `[Authorize(Policy = ...)]`.
- **Resource-based authorization** for lists/todos: a user may act only on resources they
  own or that are shared with them. Enforced server-side; UI hiding is not security.
- Every endpoint authenticated by default; `[AllowAnonymous]` requires a stated reason.

## Hardening
- Rate-limit auth endpoints (login, 2FA, password reset, passkey) — abuse returns 429.
- Secrets: dev → user-secrets; prod → Key Vault. Prefer secretless Entra (federated
  credential / managed identity). Never log tokens, cookies, passwords, or recovery codes.

## When auth changes
Update `docs/auth/auth-flows.md` (Mermaid sequence diagrams) and re-run `security-review`.
See `.claude/rules/docs-sync.md`.
