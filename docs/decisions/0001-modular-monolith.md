# ADR 0001: Modular monolith with extract-to-service path

- **Status:** accepted
- **Date:** 2026-10-07
- **Deciders:** Yasser BOUAABANE

## Context and problem statement
We need an architecture that is simple to build, test, and deploy now, but that will not
trap us in a big ball of mud as modules grow — and that allows peeling a module off into
its own service later without a rewrite.

## Decision drivers
- One team, one deployable, low operational overhead to start.
- Strong internal boundaries so modules stay independent.
- A credible path to extract a module into a service when scaling/ownership demands it.

## Considered options
1. **Layered monolith** (horizontal layers) — simple, but weak feature boundaries; tends to leak.
2. **Microservices from day one** — strong isolation, heavy ops/latency/complexity cost up front.
3. **Modular monolith** — one host, independent modules with enforced boundaries.

## Decision
We chose the **modular monolith**. One deployable host (`TodoApp.Api`) composes modules
(Identity, Todos, Notifications, Monitoring). Each module has `Domain`, `Application`,
`Infrastructure`, `Endpoints` (all `internal`) plus a public `*.Contracts` project. Data is
isolated: **one schema + one DbContext per module**, no cross-module joins or FKs, users
referenced by `UserId` only. Sync calls go through other modules' Contracts; async via
integration events through a per-module transactional **outbox**. A small `BuildingBlocks`
shared kernel holds only cross-cutting primitives. Each module self-registers via
`AddXxxModule()`/`MapXxxModule()`.

## Consequences
- **Positive:** fast local dev; enforced boundaries (ADR 0002); clear ownership; cheap to run.
- **Negative / trade-offs:** discipline required to avoid shortcut coupling; in-process events
  are not durable across process crashes without the outbox (hence the outbox, ADR 0004).
- **Extraction path:** because a module only exposes Contracts, owns its schema, and talks
  async via events, extracting it means: (1) move the module + its schema to a new host,
  (2) flip its integration-event transport from in-process to Service Bus (config switch,
  ADR 0004), (3) replace in-process Contracts calls with a remote client implementing the
  same Contracts interface, (4) point the new service at its schema (already isolated).
  No domain rewrite; the seams already exist.
