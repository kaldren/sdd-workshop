# Design system

<!-- Защо: без дизайн система всеки екран, генериран от AI, изглежда различно. Този файл е единственият източник за визията — frontend-agent го следва буквално. -->

This is a standard, not code. The tokens below are added to `src/Timesheet.Web/wwwroot/css/app.css` by the first spec that builds UI. After that, every screen uses only these tokens and Bootstrap classes.

Style: corporate blue, clean and calm — a business tool, not a marketing page. No logos or brand names in the UI.

## Colors

<!-- Защо: точни стойности вместо „синьо“. AI не трябва да избира цветове сам. -->

```css
:root {
  /* Brand */
  --color-primary:   #00338D; /* dark blue — sidebar, primary buttons, headings */
  --color-secondary: #005EB8; /* links, hover, active nav item */
  --color-accent:    #0091DA; /* highlights, focus ring */
  --color-purple:    #483698; /* charts / tags only */
  --color-teal:      #00A3A1; /* charts / tags only */

  /* Neutrals */
  --color-text:       #1E1E1E;
  --color-text-muted: #6B7280;
  --color-border:     #D9DEE7;
  --color-surface:    #F4F6FA; /* page background, table header */
  --color-background: #FFFFFF; /* cards, inputs */

  /* States */
  --color-success: #00A3A1;
  --color-warning: #F1C44D;
  --color-danger:  #E5243B;

  /* Map to Bootstrap so its classes (btn-primary, text-danger…) use our palette */
  --bs-primary: var(--color-primary);
  --bs-primary-rgb: 0, 51, 141;
  --bs-secondary: var(--color-secondary);
  --bs-success: var(--color-success);
  --bs-warning: var(--color-warning);
  --bs-danger: var(--color-danger);
  --bs-body-color: var(--color-text);
  --bs-body-bg: var(--color-surface);
  --bs-border-color: var(--color-border);
  --bs-body-font-family: "Open Sans", Arial, sans-serif;
}
```

## Typography

- Font: **Open Sans** (Google Fonts, weights 400 / 600 / 700), fallback `Arial, sans-serif`.
- Scale: h1 28px / h2 22px / h3 18px / body 15px / small 13px. Headings 600, color `--color-primary`.
- Hours and dates use `font-variant-numeric: tabular-nums` so columns line up.

## Spacing and shape

- Spacing scale: 4 / 8 / 12 / 16 / 24 / 32 px (Bootstrap `1`–`5` utilities).
- Border radius: 4px. Shadows: only `shadow-sm` on cards.

## Layout

- Dark blue sidebar (`--color-primary`) with white nav text; content area on `--color-surface`.
- Content max-width 1200px. Each page: h1 title → optional short description → content.
- At most **one** primary button per screen; other actions are `btn-outline-primary` or links.

## Components

Use Bootstrap classes only.

| Component | Rule |
| --- | --- |
| Button | `btn btn-primary` for the main action, `btn-outline-primary` for others, `btn-outline-danger` for delete (with confirmation). |
| Form | Label above the field (`form-label`), validation message below it (`invalid-feedback`). Required fields marked with `*`. |
| Table | `table table-hover` inside a card; header on `--color-surface`; numbers right-aligned. |
| Card | `card shadow-sm` with white background for grouped content. |
| Badge | `badge` with a state color for statuses. |

## States

<!-- Защо: AI често прави само „щастливия път“. Тези три състояния са задължителни. -->

Every screen that loads data shows:
- **Loading** — a Bootstrap `spinner-border` with text.
- **Empty** — a short message and, where it makes sense, the primary action.
- **Error** — `alert alert-danger` with a human-readable message, never a stack trace.

## Don'ts

- No inline `style="…"` and no hardcoded colors outside the tokens above.
- No new UI libraries, icon packs or CSS frameworks.
- No animations beyond Bootstrap defaults.
