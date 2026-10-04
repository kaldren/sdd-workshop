# Edit and delete time entries

**Status:** Implemented

## Goal

Let a user correct or remove a time entry they already created. Editing happens in a pop-up, and deleting requires an explicit confirmation so entries are not removed by accident. This extends the time entries page from `specs/002-time-entries.md`.

## Requirements

1. R1: Each row in the time entries table has an Edit and a Delete action.
2. R2: Clicking Edit opens a modal pop-up pre-filled with the entry's date, project, hours and description.
3. R3: Saving the edit modal updates the entry; the same validation rules as creation apply (date and project required, hours greater than 0 and no more than 24), and errors are shown in the modal without saving.
4. R4: Cancelling or closing the edit modal discards changes and leaves the entry unchanged.
5. R5: Clicking Delete opens a confirmation pop-up asking whether the user is sure, with two buttons: "Yeah, I'm sure!" and "Don't do it!".
6. R6: Clicking "Yeah, I'm sure!" deletes the entry and removes it from the table; clicking "Don't do it!" closes the pop-up and keeps the entry.
7. R7: If an entry no longer exists when updating or deleting, the API returns 404 and the UI shows an error message.
8. R8: If the update or delete request fails, the UI shows an `alert alert-danger` with a human-readable message and the table stays unchanged.

## Out of scope

- Bulk edit or bulk delete, undo, and soft delete or audit history.
- Editing or deleting from any page other than `/time-entries`.
- Authentication, permissions and ownership of entries.
- Filtering, sorting or reporting.

## Acceptance criteria

- [x] Given entries in the table, when the page is opened, then each row shows Edit and Delete actions (R1).
- [x] Given an entry, when the user clicks Edit, then a modal opens pre-filled with its current values (R2).
- [x] Given the edit modal with valid values, when the user saves, then the entry is updated, the modal closes and the table shows the new values (R3).
- [x] Given the edit modal with missing or invalid values, when the user saves, then validation errors are shown in the modal and nothing is saved (R3).
- [x] Given the edit modal with changed values, when the user cancels or closes it, then the entry is unchanged (R4).
- [x] Given an entry, when the user clicks Delete, then a confirmation pop-up shows the buttons "Yeah, I'm sure!" and "Don't do it!" (R5).
- [x] Given the confirmation pop-up, when the user clicks "Yeah, I'm sure!", then the entry is deleted and removed from the table (R6).
- [x] Given the confirmation pop-up, when the user clicks "Don't do it!", then the pop-up closes and the entry remains (R6).
- [x] Given an entry id that does not exist, when PUT or DELETE is called, then the API returns 404 and the UI shows an error message (R7).
- [x] Given an API failure, when the user saves an edit or confirms a delete, then an error alert is shown and the table is unchanged (R8).

## Technical notes

- Depends on `specs/002-time-entries.md`.
- Add `PUT /api/time-entries/{id}` (returns `TimeEntryResponse`, 400 with validation errors, 404 if missing) and `DELETE /api/time-entries/{id}` (204 on success, 404 if missing), grouped with the existing time entries endpoints.
- Add `UpdateTimeEntryRequest` as a `record` in `Timesheet.Shared`; reuse the validation rules from creation instead of duplicating them.
- UI: use Bootstrap modal markup driven by Blazor state (no JavaScript libraries or new packages). Delete button is `btn-outline-danger`, Edit is `btn-outline-primary`; the confirmation uses a danger-styled "Yeah, I'm sure!" button and an outline "Don't do it!" button. Follow `DESIGN.md`, including the form conventions and no more than one primary button per screen.
- Button labels "Yeah, I'm sure!" and "Don't do it!" must match exactly.

## Tasks

- [x] [backend] Add `UpdateTimeEntryRequest` to `Timesheet.Shared` (R3).
- [x] [backend] Add and test `PUT /api/time-entries/{id}` with validation and 404 handling (R3, R7).
- [x] [backend] Add and test `DELETE /api/time-entries/{id}` with 404 handling (R6, R7).
- [x] [frontend] Add Edit and Delete actions to each table row (R1).
- [x] [frontend] Add the edit modal with pre-filled form, validation and cancel (R2, R3, R4).
- [x] [frontend] Add the delete confirmation modal with "Yeah, I'm sure!" and "Don't do it!" buttons (R5, R6).
- [x] [frontend] Show error alerts for failed or 404 update and delete requests (R7, R8).
