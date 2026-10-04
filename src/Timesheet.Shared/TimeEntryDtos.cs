namespace Timesheet.Shared;

public sealed record CreateTimeEntryRequest(
    DateOnly? Date,
    string? Project,
    decimal? Hours,
    string? Description);

public sealed record UpdateTimeEntryRequest(
    DateOnly? Date,
    string? Project,
    decimal? Hours,
    string? Description);

public sealed record TimeEntryResponse(
    int Id,
    DateOnly Date,
    string Project,
    decimal Hours,
    string? Description);
