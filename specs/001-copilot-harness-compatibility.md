# Copilot harness compatibility

**Status:** Ready for implementation

## Goal

Update the repository customizations so the workshop workflow works with the current GitHub Copilot Agent Host in VS Code. Preserve the spec-driven process and the `/implement` entry point while making the documented guarantees match what the harness actually enforces.

## Requirements

1. R1: Replace the deprecated `.github/prompts/implement.prompt.md` workflow with a manually invoked Agent Skill at `.github/skills/implement/SKILL.md`, available as `/implement` and retaining the existing spec gate, backend-then-frontend delegation, build/test, verification and close steps.
2. R2: `backend-agent` and `frontend-agent` must reliably load the repository instructions when invoked as subagents and must remain intended for subagent use rather than manual selection.
3. R3: `README.md` and `AGENTS.md` must describe the current Copilot customization layout and invocation flow, and must distinguish instruction-based directory boundaries from technically enforced tool permissions.
4. R4: The existing setup spec must not be marked fully implemented while an Acceptance criterion is unchecked; its status and final task must reflect the verified state.
5. R5: Baseline template exceptions for the setup smoke endpoint, Weather transport type, template test and template UI states must be explicit so they do not conflict with the conventions for subsequent business features.
6. R6: All Copilot customization frontmatter and referenced paths must be valid for the current VS Code Copilot harness.

## Out of scope

- New timesheet business features or changes to application behavior.
- Redesigning the template Weather page or adding the full design system to the setup-only UI.
- New NuGet packages, MCP servers, hooks or plugins.
- Changing the API or Web ports, runtime configuration or start scripts.

## Acceptance criteria

- [ ] Given the Copilot Agent Host is selected in VS Code, when repository customizations are opened, then `implement` is discovered as an Agent Skill and no implementation prompt file remains.
- [ ] Given `/implement specs/NNN-*.md`, when the spec is not `Ready for implementation`, then the workflow stops without changing application code.
- [ ] Given an approved spec with backend and frontend tasks, when `/implement` runs, then it delegates to `backend-agent` before `frontend-agent`, and both agents receive the repository instructions.
- [ ] Given the agent picker is opened, when repository agents are listed, then `spec-agent` is user-selectable and the backend/frontend implementation agents are reserved for subagent invocation.
- [ ] Given a reader follows `README.md`, when they inspect the repository, then the documented files, invocation flow and permission model match the current Copilot harness configuration.
- [ ] Given `specs/000-solution-setup.md`, when its status is inspected, then it is `Implemented` only if every Acceptance criterion and the final verification task are checked; otherwise it remains ready for verification.
- [ ] Given the setup-only template artifacts, when they are compared with `AGENTS.md` and the frontend instructions, then their documented exceptions are unambiguous and do not weaken the rules for later business features.

## Technical notes

- Agent Skills use `.github/skills/<name>/SKILL.md`; the skill name must match its folder and use lowercase letters, digits and hyphens.
- Preserve explicit invocation semantics with `disable-model-invocation: true`; the workshop continues to start implementation through `/implement <spec path>`.
- Use `include-custom-instructions: true` for implementation agents invoked as subagents. Use `user-invocable: false` to keep them out of the manual agent picker.
- Tool allowlists restrict tool categories, not writable repository paths. Directory scope remains a behavioral instruction that must be reviewed through tool activity and diffs.
- Verify discovery with the VS Code Agent Customizations view or Agent Debug Logs; static file validation alone is insufficient.

## Tasks

- [ ] [harness] Create `.github/skills/implement/SKILL.md` and remove the deprecated prompt file (R1, R6).
- [ ] [harness] Configure backend and frontend agents for repository-aware subagent invocation (R2, R6).
- [ ] [docs] Update `README.md` and `AGENTS.md` for Agent Skills, current invocation and permission semantics (R3, R5).
- [ ] [docs] Reconcile the status, final task and remaining Acceptance criterion in spec 000 (R4).
- [ ] [docs] Document narrow setup-template exceptions without weakening later feature rules (R5).
- [ ] [verify] Validate frontmatter, paths and repository customization discovery (R6).
- [ ] [verify] Run build/tests and verify the remaining setup criterion before marking either spec Implemented (R4).
