# ADR 0003: Result pattern for expected failures, exceptions for the exceptional

- **Status:** accepted
- **Date:** 2026-10-07
- **Deciders:** Yasser BOUAABANE

## Context and problem statement
Application and domain operations have *expected* failure modes (validation errors, not
found, forbidden, conflict). Modeling these as exceptions is costly, hides control flow,
and makes endpoints hard to map to HTTP status codes consistently.

## Decision drivers
- Make expected outcomes explicit in method signatures.
- Clean, uniform mapping to RFC 7807 ProblemDetails at the API boundary.
- Keep exceptions meaningful (truly exceptional / programmer errors / infra faults).

## Considered options
1. **Exceptions for all failures** — simple, but conflates expected and exceptional; slow.
2. **Result pattern** — `Result`/`Result<T>` carrying success or a typed error.

## Decision
We use a **Result pattern** in `BuildingBlocks` for application/domain flow: operations
return `Result`/`Result<T>` with a typed `Error` (code + message + category:
Validation/NotFound/Conflict/Forbidden/Unexpected). Endpoints map errors to ProblemDetails
centrally. Exceptions are reserved for the genuinely exceptional — invariant violations
that indicate bugs, and infrastructure failures (DB down, etc.), caught once at the boundary
(Phase 4). Domain invariants may throw a `DomainException` guard; crossing-the-boundary
*expected* failures return `Result`.

## Consequences
- **Positive:** explicit failures; consistent HTTP mapping; fewer exception stack costs.
- **Negative / trade-offs:** more verbose call sites; requires a small Result API and
  discipline to not smuggle exceptions through. Railway-style helpers mitigate verbosity.
