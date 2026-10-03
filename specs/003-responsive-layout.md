# Responsive layout

**Status:** Implemented

## Goal

Make the UI mobile friendly and fix the app shell so the navigation menu is anchored to the left edge of the screen instead of appearing in the middle. On small screens the menu collapses behind a hamburger icon. This is for every user of the app, on phones, tablets and desktops.

## Requirements

1. R1: On screens 641px wide and above, the navigation sidebar is fixed to the left edge of the viewport at full height, and the content fills the remaining width to the right of it.
2. R2: The app shell is not centered by the loading placeholder. The loader centering classes must not apply to the rendered layout.
3. R3: On screens below 641px, the sidebar is hidden and a top bar with a hamburger button is shown; the button opens and closes the menu.
4. R4: On screens below 641px, selecting a menu link closes the menu, and the hamburger button has an accessible label and `aria-expanded` state.
5. R5: Page content has no horizontal page scroll at 320px width; the content padding is reduced on small screens.
6. R6: Tables stay inside a horizontally scrollable container on small screens, and form fields and the submit button are full width below 641px.
7. R7: The navigation links and the hamburger button have touch targets of at least 44px high.

## Out of scope

- New pages, new menu items, or changes to existing menu items and their labels.
- Changes to colors, typography or the design tokens in `DESIGN.md`.
- Card/stacked table layout on mobile, dark mode, or a collapsible sidebar on desktop.
- New icon packs or UI libraries; the hamburger uses the Bootstrap `navbar-toggler-icon`.
- Backend or API changes.

## Acceptance criteria

- [x] Given a viewport of 1280px, when any page is opened, then the sidebar touches the left edge, spans the full height, and the content starts to its right (R1, R2).
- [x] Given the app has finished loading, when the DOM of `#app` is inspected, then it has no flex centering or `min-vh-100` classes (R2).
- [x] Given a viewport of 390px, when any page is opened, then only the top bar with the hamburger button is visible and the menu is closed (R3).
- [x] Given a viewport of 390px, when the hamburger is tapped, then the menu opens; when a link is tapped, then the app navigates and the menu closes (R3, R4).
- [x] Given a viewport of 320px, when the Time entries page is opened, then the page has no horizontal scroll, the table scrolls inside its card, and the form fields and button are full width (R5, R6).
- [x] Given any viewport, when menu links and the hamburger are measured, then each is at least 44px high (R7).

## Technical notes

- Files: `src/Timesheet.Web/wwwroot/index.html`, `Layout/MainLayout.razor(.css)`, `Layout/NavMenu.razor(.css)`, `wwwroot/css/app.css`, `Pages/TimeEntries.razor`.
- Root cause for R2: `#app` in `index.html` has `d-flex align-items-center justify-content-center min-vh-100 gap-2`. Move these classes to a wrapper around the loading placeholder, so Blazor's render replaces them.
- Breakpoint 641px matches the existing layout CSS.
- Use only Bootstrap classes and the tokens from `DESIGN.md`; no inline styles and no new packages.
- Update the "Layout" section of `DESIGN.md` with the responsive behavior only if the implementation needs it.

## Tasks

- [x] [frontend] Fix `#app` centering in `index.html` (R2).
- [x] [frontend] Anchor the sidebar left at full height on desktop in `MainLayout` (R1).
- [x] [frontend] Add the mobile top bar, hamburger toggle, auto-close on link click and accessibility attributes in `NavMenu` (R3, R4).
- [x] [frontend] Add responsive padding, full-width form controls and touch target sizes (R5, R6, R7).
- [x] [frontend] Verify at 320px, 390px and 1280px widths (R1–R7).
