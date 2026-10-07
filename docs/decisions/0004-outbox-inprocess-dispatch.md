# ADR 0004: Transactional outbox with switchable in-process / Service Bus dispatch

- **Status:** accepted
- **Date:** 2026-10-07
- **Deciders:** Yasser BOUAABANE

## Context and problem statement
Modules communicate asynchronously via integration events (ADR 0001). Events must not be
lost if the process crashes after committing a state change, and we want to start simple
(in-process) but move to Azure Service Bus without reworking producers/consumers.

## Decision drivers
- Atomicity: an event is persisted in the **same transaction** as the state change.
- No lost events; at-least-once delivery with idempotent consumers.
- A transport switch (in-process ↔ Service Bus) by configuration only.

## Considered options
1. **Direct in-process dispatch** — simplest; loses events on crash; no durability.
2. **Publish straight to Service Bus inline** — dual-write problem (DB + broker not atomic).
3. **Transactional outbox** — write events to an outbox table in the same DB transaction; a
   dispatcher relays them afterwards.

## Decision
Each module has a per-schema **outbox table**. Handlers enqueue integration events into the
outbox within the same EF Core transaction as the state change. An `IIntegrationEventDispatcher`
relays outbox rows to subscribers. The dispatcher implementation is chosen by configuration:
**in-process** dispatch first (same host), swappable to **Azure Service Bus** without changing
event contracts or handlers. Consumers are **idempotent** (dedupe by event id) and support
dead-lettering (Phase 6).

## Consequences
- **Positive:** no dual-write; durable, at-least-once delivery; transport is a config flip,
  which is also the async half of the module-extraction path (ADR 0001).
- **Negative / trade-offs:** outbox table + relay to build and operate; consumers must be
  idempotent; delivery is at-least-once (never exactly-once), so handlers must tolerate dupes.
