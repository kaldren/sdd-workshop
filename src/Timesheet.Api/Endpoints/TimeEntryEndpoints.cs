using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Timesheet.Api.Data;
using Timesheet.Shared;

namespace Timesheet.Api.Endpoints;

public static class TimeEntryEndpoints
{
    public static IEndpointRouteBuilder MapTimeEntryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/time-entries");

        group.MapGet("", GetTimeEntries);
        group.MapPost("", CreateTimeEntry);
        group.MapPut("/{id:int}", UpdateTimeEntry);
        group.MapDelete("/{id:int}", DeleteTimeEntry);

        return app;
    }

    public static async Task<Ok<List<TimeEntryResponse>>> GetTimeEntries(
        TimesheetDbContext db,
        CancellationToken cancellationToken = default)
    {
        var entries = await db.TimeEntries
            .OrderBy(entry => entry.Date)
            .ThenBy(entry => entry.Id)
            .Select(entry => new TimeEntryResponse(
                entry.Id,
                entry.Date,
                entry.Project,
                entry.Hours,
                entry.Description))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(entries);
    }

    public static async Task<IResult> CreateTimeEntry(
        CreateTimeEntryRequest request,
        TimesheetDbContext db,
        CancellationToken cancellationToken = default)
    {
        var errors = ValidateTimeEntry(request.Date, request.Project, request.Hours);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        var entry = new TimeEntry
        {
            Date = request.Date!.Value,
            Project = request.Project!.Trim(),
            Hours = request.Hours!.Value,
            Description = request.Description
        };

        db.TimeEntries.Add(entry);
        await db.SaveChangesAsync(cancellationToken);

        var response = new TimeEntryResponse(
            entry.Id,
            entry.Date,
            entry.Project,
            entry.Hours,
            entry.Description);

        return TypedResults.Created("/api/time-entries", response);
    }

    public static async Task<IResult> UpdateTimeEntry(
        int id,
        UpdateTimeEntryRequest request,
        TimesheetDbContext db,
        CancellationToken cancellationToken = default)
    {
        var entry = await db.TimeEntries.FindAsync([id], cancellationToken);
        if (entry is null)
        {
            return Results.NotFound();
        }

        var errors = ValidateTimeEntry(request.Date, request.Project, request.Hours);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        entry.Date = request.Date!.Value;
        entry.Project = request.Project!.Trim();
        entry.Hours = request.Hours!.Value;
        entry.Description = request.Description;

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(new TimeEntryResponse(
            entry.Id,
            entry.Date,
            entry.Project,
            entry.Hours,
            entry.Description));
    }

    public static async Task<IResult> DeleteTimeEntry(
        int id,
        TimesheetDbContext db,
        CancellationToken cancellationToken = default)
    {
        var entry = await db.TimeEntries.FindAsync([id], cancellationToken);
        if (entry is null)
        {
            return Results.NotFound();
        }

        db.TimeEntries.Remove(entry);
        await db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    private static Dictionary<string, string[]> ValidateTimeEntry(
        DateOnly? date,
        string? project,
        decimal? hours)
    {
        var errors = new Dictionary<string, string[]>();

        if (date is null)
        {
            errors[nameof(CreateTimeEntryRequest.Date)] = ["Date is required."];
        }

        if (string.IsNullOrWhiteSpace(project))
        {
            errors[nameof(CreateTimeEntryRequest.Project)] = ["Project is required."];
        }

        if (hours is null or <= 0 or > 24)
        {
            errors[nameof(CreateTimeEntryRequest.Hours)] = ["Hours must be greater than 0 and no more than 24."];
        }

        return errors;
    }
}
