# Auth flows

Authoritative description of every authentication/authorization flow. Keep in sync with
code and re-run `security-review` on any change (`.claude/rules/auth.md`). Flows are
**specified here now**; implemented in Phase 3.

## Principles
- **React → BFF:** API performs OIDC; browser holds only `HttpOnly`/`Secure`/`SameSite`
  cookies + antiforgery. **Mobile/API → bearer tokens.**
- Multiple auth methods (local password, passkey, Entra) link to **one** user account.
- Entra ID is **single-tenant** by default. AuthZ is policy- and resource-based.

## Local sign-in (password + optional 2FA)

```mermaid
sequenceDiagram
  actor U as User (browser)
  participant API as TodoApp.Api (BFF)
  participant ID as Identity module
  U->>API: POST /auth/login (email, password) + antiforgery
  API->>ID: ValidateCredentials
  alt 2FA enabled
    ID-->>API: RequiresTwoFactor
    API-->>U: 200 { requiresTwoFactor: true }
    U->>API: POST /auth/login/2fa (code) + antiforgery
    API->>ID: ValidateTwoFactor
  end
  ID-->>API: Succeeded (principal)
  API-->>U: Set-Cookie (auth, HttpOnly) ; 204
```

## Passkey sign-in (WebAuthn, .NET 10 Identity)

```mermaid
sequenceDiagram
  actor U as User (browser)
  participant API as TodoApp.Api (BFF)
  participant ID as Identity module
  U->>API: GET /auth/passkey/assertion-options
  API->>ID: MakeAssertionOptions
  ID-->>API: options (challenge)
  API-->>U: options
  U->>U: navigator.credentials.get()
  U->>API: POST /auth/passkey/assertion (credential) + antiforgery
  API->>ID: VerifyAssertion
  ID-->>API: Succeeded
  API-->>U: Set-Cookie (auth) ; 204
```

## Entra ID external login (OIDC, single-tenant), linked to local account

```mermaid
sequenceDiagram
  actor U as User (browser)
  participant API as TodoApp.Api (BFF)
  participant E as Microsoft Entra ID
  participant ID as Identity module
  U->>API: GET /auth/entra/challenge
  API-->>U: 302 to Entra authorize
  U->>E: Authenticate (single-tenant)
  E-->>API: 302 /auth/entra/callback (code)
  API->>E: Exchange code (PKCE / secretless federated cred)
  E-->>API: id_token + claims (roles/groups)
  API->>ID: LinkOrProvision(externalLogin)  %% policy decided in Phase 3 ADR
  ID-->>API: app user (+ mapped roles)
  API-->>U: Set-Cookie (auth) ; 302 app
```

## Authorization model
- Policies (not role strings): `RequireAdmin`, resource handlers for list ownership/shares.
- Resource-based: a user may read/modify a list only if they own it or it is shared with
  them (read vs edit). IDOR is covered by tests (user A can never touch user B's data).
- Every endpoint authenticated by default; `[AllowAnonymous]` requires a documented reason.

## Hardening
Rate limiting on auth endpoints; lockout; email confirmation; recovery codes; secretless
Entra where possible; Key Vault for any unavoidable secret. Nothing sensitive is logged.

_Mobile bearer-token flow and Entra provision-vs-link policy are finalized in Phase 3 (ADR)._
