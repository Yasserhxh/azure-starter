# ADR 0002: Enforce boundaries with ArchUnitNET

- **Status:** accepted
- **Date:** 2026-10-07
- **Deciders:** Yasser BOUAABANE

## Context and problem statement
The modular monolith (ADR 0001) only holds if boundaries are enforced automatically, not
by convention. We need a test suite that fails the build when a module reaches into another
module's internals, when `Domain` references `Infrastructure`, or when anything bypasses
`*.Contracts`.

## Decision drivers
- Rules must run in CI and fail the build.
- Expressive, maintainable rule definitions that scale as modules are added.
- Active maintenance and .NET 10 support.

## Considered options
1. **NetArchTest** — lightweight, fluent, popular; less actively maintained; simpler rule set.
2. **ArchUnitNET** — richer rule model (layers, slices, dependency rules), actively maintained.

## Decision
We chose **ArchUnitNET** (`tests/TodoApp.ArchitectureTests`). It expresses our rules
cleanly: Domain must not depend on Infrastructure; a module's internal assembly must not be
referenced by any other module; references across modules go only through `*.Contracts`;
BuildingBlocks must not depend on modules. Tests load all module assemblies by naming
convention so a module scaffolded via `dotnet new azure-module` is covered automatically.

## Consequences
- **Positive:** boundaries are provable and break the build on violation; new modules are
  auto-covered; rules double as living documentation of the architecture.
- **Negative / trade-offs:** a learning curve for the ArchUnitNET API; a slightly heavier
  dependency than NetArchTest. These tests must exist and pass **before** any module code.
