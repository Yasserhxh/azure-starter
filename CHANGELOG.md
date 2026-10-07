# Changelog

All notable changes to this template are documented here. Format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/); this project will adopt
[Semantic Versioning](https://semver.org/) at its first tagged release.

## [Unreleased]

### Added — Phase 1: skeleton, Claude setup, and docs
- Repository skeleton: solution (`TodoApp.slnx`), folder tree for host, modules, functions,
  web, tests, infra, tools, and docs.
- Minimal buildable host `src/TodoApp.Api` (.NET 10) as the composition root.
- Root build config: `global.json` (SDK 10.0.301), `Directory.Build.props` (nullable,
  warnings-as-errors, analyzers), `Directory.Packages.props` (central package management),
  `.editorconfig`, `.gitignore`, `.config/dotnet-tools.json`.
- Claude Code project setup: `.claude/settings.json` (build/test/lint allowed, secrets denied)
  and `.claude/rules/` (`auth.md`, `docs-sync.md`).
- Documentation scaffolding: architecture (C4 overview + modules), auth flows (Mermaid),
  observability/monitoring, brand tokens (W3C `design-tokens.json` placeholder), runbooks.
- Project `CLAUDE.md` and this `README.md`.
- ADRs 0001–0005: modular monolith, ArchUnitNET boundaries, Result pattern, transactional
  outbox, Aspire dev orchestration (+ 0000 template).

### Decided (Phase 0)
- Target **.NET 10 LTS / C# 14**; clients **React + Flutter**; **Entra single-tenant** default;
  **BFF cookies** for web, **bearer tokens** for mobile/API.

[Unreleased]: https://example.com/compare/HEAD
