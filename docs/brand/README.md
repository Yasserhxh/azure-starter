# Brand & design tokens

`design-tokens.json` (W3C Design Tokens format) is the **single source of truth** for
all design values. Components never hardcode colors, fonts, spacing, radii, or shadows —
they consume generated CSS variables (web) and a generated theme (Flutter).

## Files
- `design-tokens.json` — machine-readable tokens (primitives + semantic, light/dark).
- `brand-tokens.md` — human-readable summary of the current (placeholder) brand.
- `charte/` — drop the client's **charte graphique** here (brand PDFs/images).

## Apply a client's charte graphique
1. Put the brand assets in `docs/brand/charte/`.
2. Use the `expert-ui-design` skill to extract palette, typography, spacing, logo usage.
   **The charte overrides all creative defaults** — follow it exactly.
3. Update `design-tokens.json` (and `brand-tokens.md`) with the real values.
4. Regenerate tokens: `node tools/tokens/build.mjs` (Phase 5) → writes
   `src/web/src/styles/tokens.css` and the Flutter theme.
5. Rebuild the web/mobile apps. No component code should change.

Until a charte is provided, the repo ships a neutral, distinctive **placeholder brand**.
