---
name: implement
description: Implement an approved Timesheet spec by delegating its backend and frontend tasks, validating the result, and updating the spec. Use only for specs marked Ready for implementation.
argument-hint: Path to the spec, e.g. specs/001-time-entries.md
disable-model-invocation: true
---

# Implement an approved spec

Follow the [repository rules](../../../AGENTS.md).

1. Read the spec path supplied by the user. If it is `specs/template.md` or its status is not `Ready for implementation`, stop without changing application code and explain why.
2. If Tasks are missing or are not assigned to the relevant implementation role, update the spec and show the resulting plan before changing application code.
3. Run `backend-agent` as a subagent with the spec path and its `[backend]` tasks. Wait for it to finish and resolve any reported spec problem before continuing.
4. Run `frontend-agent` as a subagent with the spec path and its `[frontend]` tasks only after the backend work is complete. If the spec has no tasks for one of the roles, skip that role.
5. Run `dotnet build` and `dotnet test`. Fix in-scope failures or send them back to the responsible subagent. Do not continue while either command fails.
6. Verify every Acceptance criterion through a test, HTTP call, running application, or documented manual check. Check off only criteria actually verified and list anything that remains unverified.
7. If the spec is wrong, unclear, or incomplete at any point, stop and propose a spec change instead of improvising in code.
8. Set the status to `Implemented` only after all Tasks and Acceptance criteria are checked. Finish with a short result for each requirement (`R1` … `Rn`).

If the harness cannot invoke the required custom subagent, stop and report the missing capability instead of performing that role under a different identity.
