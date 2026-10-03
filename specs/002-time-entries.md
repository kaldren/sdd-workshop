# Time entries

**Status:** Implemented

## Goal

Allow a user to record time spent on a project and see the recorded entries. This is the minimal business feature for the workshop demo.

## Requirements

1. R1: The user can create a time entry with date, project, hours and an optional description.
2. R2: Date and project are required, and hours must be greater than 0 and no more than 24; invalid input returns validation errors.
3. R3: The user can view all time entries in a table.
4. R4: The page shows loading, empty and error states according to `DESIGN.md`.

## Out of scope

- Editing, deleting, filtering or approving entries.
- Authentication, users, project management and reporting.

## Acceptance criteria

- [x] Given valid values, when the user submits a time entry, then it is saved and shown in the table.
- [x] Given missing or invalid values, when the user submits the form, then validation errors are shown and nothing is saved.
- [x] Given saved entries, when the page is opened, then all entries are displayed.
- [x] Given loading, no data or an API failure, when the page is opened, then the matching state is shown.

## Technical notes

- Add `GET /api/time-entries` and `POST /api/time-entries` using EF Core InMemory.
- Keep request and response DTOs as `record`s in `Timesheet.Shared`.
- Add a Blazor page at `/time-entries` and a navigation link.
- Follow `DESIGN.md`; add no packages.

## Tasks

- [x] [backend] Add the entity, shared DTOs and database set (R1, R3).
- [x] [backend] Add and test the GET and POST endpoints, including validation (R1, R2, R3).
- [x] [frontend] Add the time-entry page with form, table and navigation link (R1, R3).
- [x] [frontend] Add validation and loading, empty and error states (R2, R4).
