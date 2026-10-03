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
        var errors = new Dictionary<string, string[]>();

        if (request.Date is null)
        {
            errors[nameof(request.Date)] = ["Date is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Project))
        {
            errors[nameof(request.Project)] = ["Project is required."];
        }

        if (request.Hours is null or <= 0 or > 24)
        {
            errors[nameof(request.Hours)] = ["Hours must be greater than 0 and no more than 24."];
        }

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
}
