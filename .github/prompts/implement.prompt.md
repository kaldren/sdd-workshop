---
name: implement
description: Implements an approved spec by delegating to backend-agent and frontend-agent, then verifies it against its Acceptance criteria.
argument-hint: Path to the spec, e.g. specs/001-time-entries.md
agent: agent
tools: ['agent', 'read', 'search', 'edit', 'execute', 'todo']
---

<!-- Защо: стъпки 2 и 3 от процеса — имплементация и проверка. Главният агент координира, специализираните агенти пишат кода, а spec-ът е мярката за „готово“. -->

# /implement — spec → code

Implement the spec given by the user. Follow `AGENTS.md`.

1. **Check the gate.** Read the spec. If it is `specs/template.md` or its status is not `Ready for implementation`, stop and tell the user why.
2. **Plan.** If Tasks are missing or not tagged `[backend]` / `[frontend]`, add them and show them to the user before continuing.
3. **Backend.** Run the `backend-agent` as a subagent with the spec path and its `[backend]` tasks.
4. **Frontend.** When the backend is done, run the `frontend-agent` as a subagent with the spec path and its `[frontend]` tasks.
5. **Build and test.** Run `dotnet build` and `dotnet test`. Fix or send back failures before going on.
6. **Verify.** Go through the Acceptance criteria one by one. Check off only the ones you actually verified (test, HTTP call or running app). List the ones you could not verify and why.
7. **Spec problems.** If at any step the spec turns out wrong or incomplete, stop and propose the change to the spec. Do not improvise in code.
8. **Close.** When all Tasks and Acceptance criteria are checked, set `**Status:** Implemented`. End with a short summary per requirement (`R1` … `Rn`): done / not done.
