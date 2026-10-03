# Timesheet — project rules

<!-- Защо: GitHub Copilot (и други AI агенти) зарежда този файл автоматично във всеки чат. Това е „конституцията“ на проекта — правила, които важат винаги, без да ги повтаряме в промпта. -->

## Project

Timesheet POC: users log the hours they spend on projects.

- Stack: .NET 10, ASP.NET Core Minimal API, Blazor WebAssembly, EF Core InMemory, xUnit.
- `src/Timesheet.Api` — API (http://localhost:5100)
- `src/Timesheet.Web` — UI (http://localhost:5200)
- `src/Timesheet.Shared` — DTOs used by both Api and Web
- `tests/Timesheet.Api.Tests` — API tests
- Run: `./run.sh` (or `./run.ps1`). Test: `dotnet test`.

## Spec-Driven Development rules

<!-- Защо: това е сърцето на SDD. Без тези правила AI пише код по догадки, а не по договор. -->

1. No code without a spec. Every change starts from a file in `specs/`.
   - A new spec is a copy of `specs/template.md` with the same sections in the same order.
   - File name: `specs/NNN-<kebab-case-name>.md`, where `NNN` is the next free number (`001`, `002`…). Numbers are never reused or changed.
2. Implement only specs with `**Status:** Ready for implementation`. `Draft` means a human has not approved it yet.
3. If the spec is wrong, unclear or incomplete — stop and propose a change to the spec. Fix the spec first, then the code.
4. Do nothing listed in **Out of scope**, and nothing the spec does not ask for.
5. Reference requirement numbers (`R1`, `R2`…) in test names and commit messages.
6. Check off Tasks and Acceptance criteria only after they are actually done and verified.

## Where things live

| File | Answers |
| --- | --- |
| `specs/NNN-*.md` | **What** we build (`specs/template.md` is the template, not a spec) |
| `DESIGN.md` | **How it looks** — the design system for every UI change |
| `.github/agents/` | **Who** does the work — `spec-agent` (idea → spec), `backend-agent`, `frontend-agent` |
| `.github/prompts/` | **How** we work — `/implement` (spec → code) |
| `.github/instructions/` | Rules for specific folders, e.g. UI rules for `src/Timesheet.Web` |

## Conventions

- DTOs are `record`s in `Timesheet.Shared`; Api and Web never define their own copies.
- API endpoints are grouped per feature; UI follows `DESIGN.md` exactly.
- No new NuGet packages or UI libraries unless the spec says so.
