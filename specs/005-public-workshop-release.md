# Public workshop release

**Status:** Implemented

## Goal

Publish the spec-driven development workshop demo under the neutral project name `sdd-workshop`. Review the files and publishable history for company references and credentials before making the repository public.

## Requirements

1. R1: Rename the local project directory to `/Users/kaloyan/Projects/sdd-workshop` and use `sdd-workshop` as the GitHub repository name. Keep the `Timesheet.*` solution and application names unchanged.
2. R2: Review tracked files, hidden configuration, paths, images, and Git metadata for references to the former company-specific identity. Remove any such references from the release and clean generated `bin` and `obj` artifacts that retain the former absolute path.
3. R3: Scan release files and the complete publishable Git history with Gitleaks. Review credential and private-key candidates without exposing secret values. Do not publish while a confirmed secret or unresolved finding remains.
4. R4: Run `dotnet build` and `dotnet test` successfully from the renamed directory without changing application behavior or adding application dependencies.
5. R5: Create the public repository `kaldren/sdd-workshop` with the description `Spec-Driven Development with AI — workshop demo`, configure `origin`, and push only `main` and its verified history. Do not push agent or Codex internal refs. If the destination already exists, stop without changing it.
6. R6: Verify the public visibility and remote commit against the local `main`, and report the repository link and verification results. Check off tasks and acceptance criteria only after verification.

## Out of scope

- Application features, UI changes, transport contracts, or changes to existing feature specifications.
- New NuGet packages or UI libraries.
- Publishing local build artifacts, credentials, scanner reports, or internal agent refs.
- Rewriting clean Git history or modifying an existing destination repository.

## Acceptance criteria

- [x] Given the approved release scope, when the directory is renamed, then the checkout is at `/Users/kaloyan/Projects/sdd-workshop` and its solution and application names are unchanged (R1).
- [x] Given the release files and history, when company identity and image checks finish, then no former company references remain in the publishable material and stale generated artifacts have been cleaned (R2).
- [x] Given the release files and publishable history, when Gitleaks and manual review finish, then no confirmed secrets or unresolved findings remain (R3).
- [x] Given the renamed checkout, when `dotnet build` and `dotnet test` run, then both succeed (R4).
- [x] Given an available destination name, when the repository is created and pushed, then `kaldren/sdd-workshop` is public with the specified description and only the intended branch is published (R5).
- [x] Given the published repository, when its visibility and commit are inspected, then it is public and its `main` commit matches local `main`; the final report includes the URL and check results (R6).

## Technical notes

- Publication verified at `https://github.com/kaldren/sdd-workshop`: public visibility, requested description, default branch `main`, and only `refs/heads/main` on the remote. The first verified local/remote commit was `39bd3f2b83b58482a6929dd4e799cef87943c1e2`; the completion record is rescanned, pushed, and checked again before the final report.

- Pre-publication verification: Gitleaks 8.30.1 reported zero findings for the files and all reachable local history; company-reference checks of paths, contents, commit metadata, and both Blazor icons passed. `dotnet build Timesheet.sln` passed with zero warnings and errors; all 23 existing tests passed from the renamed directory.

- The user approved this release scope and destination before implementation.
- Preserve the existing main-branch history when the release checks pass. Scan all reachable local history as an additional check, but publish only `main`.
- Use a standalone Gitleaks tool outside the repository; keep redacted audit reports in a temporary directory.
- Run authenticated GitHub operations with the existing CLI login; never copy credentials into the checkout.
- Complete and verify publication before marking the final acceptance criteria or status as implemented; push the verified completion record and recheck the final commit.

## Tasks

- [x] [release] Rename the local directory and clean stale build artifacts (R1, R2).
- [x] [audit] Inspect files, hidden configuration, paths, images, and history for former company references (R2).
- [x] [audit] Run Gitleaks on release files and history and resolve or review every finding (R3).
- [x] [verify] Build the solution and run the existing tests from the renamed checkout (R4).
- [x] [publish] Create the public repository, configure origin, and push only main (R5).
- [x] [verify] Verify remote visibility, description, branch list, and commit equality, then record completion and report the results (R6).
