---
name: Frontend rules
description: UI rules for the Blazor app — follow the design system.
applyTo: 'src/Timesheet.Web/**'
---

<!-- Защо: тези правила се добавят автоматично всеки път, когато AI пипа файл в src/Timesheet.Web — дори без frontend-agent. Така DESIGN.md се спазва винаги. -->

- Follow `DESIGN.md` exactly: only its color tokens, typography, Bootstrap classes and component rules.
- Every page that loads data has loading, empty and error states.
- No inline styles, no hardcoded colors, no new UI libraries.
