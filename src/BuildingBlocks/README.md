# BuildingBlocks (shared kernel)

Small, cross-cutting primitives only — **no business logic**.

Planned contents (Phase 2): `Result`/`Result<T>`, domain & integration event base
types, the transactional outbox abstraction, and correlation/observability helpers.

Rules: every module may depend on BuildingBlocks; BuildingBlocks depends on nothing
in `src/Modules`. Enforced by `tests/TodoApp.ArchitectureTests`.
