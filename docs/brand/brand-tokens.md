# Placeholder brand

Human-readable summary of `design-tokens.json`. This is a **placeholder** until a client
charte graphique is dropped in `charte/` and the tokens are regenerated.

## Color
| Token | Light | Dark | Use |
|---|---|---|---|
| `brand.primary` | `#5B5BD6` | `#8E8CF0` | Primary actions, focus, links |
| `brand.accent` | `#0FB5A6` | `#2DD4BF` | Highlights, success-ish accents |
| `neutral.*` | `#FFFFFF → #0B0B12` scale | inverted | Surfaces, text, borders |
| `semantic.success` | `#1A8F57` | `#3DD68C` | Positive state |
| `semantic.warning` | `#B45309` | `#F59E0B` | Caution |
| `semantic.danger` | `#C0352B` | `#F87171` | Errors, destructive |

## Typography
- Sans: `Inter, system-ui, sans-serif`. Mono: `"JetBrains Mono", ui-monospace, monospace`.
- Scale (rem): 0.75 / 0.875 / 1 / 1.125 / 1.25 / 1.5 / 1.875 / 2.25 / 3.
- Weights: 400 / 500 / 600 / 700.

## Spacing & radius
- Spacing scale (rem): 0 / 0.25 / 0.5 / 0.75 / 1 / 1.5 / 2 / 3 / 4.
- Radius: sm `0.375rem`, md `0.5rem`, lg `0.75rem`, full `9999px`.

## Elevation
- Three shadow levels (sm/md/lg) tuned for light and dark surfaces.

Accessibility: all text/background pairings must meet **WCAG 2.2 AA** contrast. Verify
after any token change.
