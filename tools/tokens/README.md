# tools/tokens

Token build script (Phase 5). Reads `docs/brand/design-tokens.json` (W3C Design
Tokens format — the single source of truth) and generates:

- `src/web/src/styles/tokens.css` — CSS custom properties, light + dark modes
- Flutter theme (Phase 9)

Swapping a brand = edit the JSON and regenerate. Components never hardcode design
values. See `docs/brand/README.md`.
