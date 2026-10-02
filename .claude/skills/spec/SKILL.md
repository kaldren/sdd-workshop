---
name: spec
description: Turns a feature idea into a new spec file in specs/ from the template. Use when the user wants to describe a new feature or runs /spec <idea>. Writes no code.
---

<!-- Защо: стъпка 1 от процеса — от идея до спецификация. Скилът прави процеса повторяем: всеки път същите стъпки, същият формат. -->

# /spec — idea → spec

Input: a short description of the feature (`$ARGUMENTS`).

1. Read `CLAUDE.md`, `specs/template.md` and the existing specs in `specs/`, so the new spec fits what already exists.
2. Pick the next number `NNN` (e.g. `001`) and a short kebab-case slug. Copy the template to `specs/NNN-<slug>.md`.
3. If something important is unclear (who uses it, what data, what is out of scope), **ask the user up to 5 short questions before writing**. Do not guess.
4. Fill in every section:
   - Requirements numbered `R1`, `R2`… — short, testable statements.
   - Acceptance criteria in Given / When / Then form.
   - Tasks prefixed with `[backend]` or `[frontend]`, each referencing its `R`s.
   - Remove the template's HTML comments.
5. Leave `**Status:** Draft`. **Do not write or change any code.**
6. Finish with the file path and: "Review the spec and change the status to `Ready for implementation` when you agree with it."
