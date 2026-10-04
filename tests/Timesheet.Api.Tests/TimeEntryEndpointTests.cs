using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Timesheet.Api.Data;
using Timesheet.Api.Endpoints;
using Timesheet.Shared;

namespace Timesheet.Api.Tests;

public class TimeEntryEndpointTests
{
    [Fact]
    public async Task R1_CreateEntry_WithValidValues_SavesAndReturnsEntry()
    {
        await using var db = CreateDbContext();
        var request = new CreateTimeEntryRequest(
            new DateOnly(2026, 10, 3),
            "Workshop",
            7.5m,
            "Backend implementation");

        var result = await TimeEntryEndpoints.CreateTimeEntry(request, db);

        var created = Assert.IsAssignableFrom<IValueHttpResult>(result);
        Assert.Equal(StatusCodes.Status201Created, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        var response = Assert.IsType<TimeEntryResponse>(created.Value);
        Assert.True(response.Id > 0);
        Assert.Equal(request.Date, response.Date);
        Assert.Equal(request.Project, response.Project);
        Assert.Equal(request.Hours, response.Hours);
        Assert.Equal(request.Description, response.Description);
        Assert.Equal(1, await db.TimeEntries.CountAsync());
    }

    [Theory]
    [InlineData(null, "Project", 1)]
    [InlineData("2026-10-03", null, 1)]
    [InlineData("2026-10-03", " ", 1)]
    [InlineData("2026-10-03", "Project", 0)]
    [InlineData("2026-10-03", "Project", -1)]
    [InlineData("2026-10-03", "Project", 24.01)]
    public async Task R2_CreateEntry_WithInvalidValues_Returns400AndDoesNotSave(
        string? date,
        string? project,
        decimal hours)
    {
        await using var db = CreateDbContext();
        DateOnly? parsedDate = date is null ? null : DateOnly.Parse(date);
        var request = new CreateTimeEntryRequest(parsedDate, project, hours, null);

        var result = await TimeEntryEndpoints.CreateTimeEntry(request, db);

        Assert.Equal(StatusCodes.Status400BadRequest, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        var problemResult = Assert.IsAssignableFrom<IValueHttpResult>(result);
        var problem = Assert.IsType<HttpValidationProblemDetails>(problemResult.Value);
        Assert.NotEmpty(problem.Errors);
        Assert.Empty(await db.TimeEntries.ToListAsync());
    }

    [Fact]
    public async Task R2_CreateEntry_WithMissingHours_ReturnsValidationError()
    {
        await using var db = CreateDbContext();
        var request = new CreateTimeEntryRequest(new DateOnly(2026, 10, 3), "Project", null, null);

        var result = await TimeEntryEndpoints.CreateTimeEntry(request, db);

        Assert.Equal(StatusCodes.Status400BadRequest, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        var problem = Assert.IsType<HttpValidationProblemDetails>(
            Assert.IsAssignableFrom<IValueHttpResult>(result).Value);
        Assert.Contains(nameof(request.Hours), problem.Errors.Keys);
        Assert.Empty(await db.TimeEntries.ToListAsync());
    }

    [Fact]
    public async Task R2_CreateEntry_With24Hours_AcceptsUpperBoundary()
    {
        await using var db = CreateDbContext();
        var request = new CreateTimeEntryRequest(new DateOnly(2026, 10, 3), "Project", 24, null);

        var result = await TimeEntryEndpoints.CreateTimeEntry(request, db);

        Assert.Equal(StatusCodes.Status201Created, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        Assert.Equal(24, (await db.TimeEntries.SingleAsync()).Hours);
    }

    [Fact]
    public async Task R3_GetEntries_ReturnsAllSavedEntries()
    {
        await using var db = CreateDbContext();
        db.TimeEntries.AddRange(
            new TimeEntry
            {
                Date = new DateOnly(2026, 10, 4),
                Project = "Later project",
                Hours = 2
            },
            new TimeEntry
            {
                Date = new DateOnly(2026, 10, 3),
                Project = "Earlier project",
                Hours = 4
            });
        await db.SaveChangesAsync();

        var result = await TimeEntryEndpoints.GetTimeEntries(db);

        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Collection(
            result.Value!,
            entry => Assert.Equal("Earlier project", entry.Project),
            entry => Assert.Equal("Later project", entry.Project));
    }

    [Fact]
    public async Task R3_UpdateEntry_WithValidValues_UpdatesAndReturnsEntry()
    {
        await using var db = CreateDbContext();
        var entry = AddEntry(db);
        await db.SaveChangesAsync();
        var request = new UpdateTimeEntryRequest(
            new DateOnly(2026, 10, 4),
            "Updated project",
            8.5m,
            "Updated description");

        var result = await TimeEntryEndpoints.UpdateTimeEntry(entry.Id, request, db);

        Assert.Equal(StatusCodes.Status200OK, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        var response = Assert.IsType<TimeEntryResponse>(Assert.IsAssignableFrom<IValueHttpResult>(result).Value);
        Assert.Equal(entry.Id, response.Id);
        Assert.Equal(request.Date, response.Date);
        Assert.Equal(request.Project, response.Project);
        Assert.Equal(request.Hours, response.Hours);
        Assert.Equal(request.Description, response.Description);
        var savedEntry = await db.TimeEntries.SingleAsync();
        Assert.Equal(request.Date, savedEntry.Date);
        Assert.Equal(request.Project, savedEntry.Project);
        Assert.Equal(request.Hours, savedEntry.Hours);
        Assert.Equal(request.Description, savedEntry.Description);
    }

    [Theory]
    [InlineData(null, "Project", 1)]
    [InlineData("2026-10-03", null, 1)]
    [InlineData("2026-10-03", " ", 1)]
    [InlineData("2026-10-03", "Project", 0)]
    [InlineData("2026-10-03", "Project", -1)]
    [InlineData("2026-10-03", "Project", 24.01)]
    public async Task R3_UpdateEntry_WithInvalidValues_Returns400AndDoesNotUpdate(
        string? date,
        string? project,
        decimal hours)
    {
        await using var db = CreateDbContext();
        var entry = AddEntry(db);
        await db.SaveChangesAsync();
        var originalDate = entry.Date;
        var originalProject = entry.Project;
        var originalHours = entry.Hours;
        var request = new UpdateTimeEntryRequest(
            date is null ? null : DateOnly.Parse(date),
            project,
            hours,
            "Changed description");

        var result = await TimeEntryEndpoints.UpdateTimeEntry(entry.Id, request, db);

        Assert.Equal(StatusCodes.Status400BadRequest, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        var problem = Assert.IsType<HttpValidationProblemDetails>(
            Assert.IsAssignableFrom<IValueHttpResult>(result).Value);
        Assert.NotEmpty(problem.Errors);
        Assert.Equal(originalDate, entry.Date);
        Assert.Equal(originalProject, entry.Project);
        Assert.Equal(originalHours, entry.Hours);
        Assert.Null(entry.Description);
    }

    [Fact]
    public async Task R3_UpdateEntry_WithMissingHours_ReturnsValidationError()
    {
        await using var db = CreateDbContext();
        var entry = AddEntry(db);
        await db.SaveChangesAsync();
        var request = new UpdateTimeEntryRequest(new DateOnly(2026, 10, 4), "Project", null, null);

        var result = await TimeEntryEndpoints.UpdateTimeEntry(entry.Id, request, db);

        Assert.Equal(StatusCodes.Status400BadRequest, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        var problem = Assert.IsType<HttpValidationProblemDetails>(
            Assert.IsAssignableFrom<IValueHttpResult>(result).Value);
        Assert.Contains(nameof(request.Hours), problem.Errors.Keys);
        Assert.Equal("Original project", entry.Project);
    }

    [Fact]
    public async Task R3_UpdateEntry_With24Hours_AcceptsUpperBoundary()
    {
        await using var db = CreateDbContext();
        var entry = AddEntry(db);
        await db.SaveChangesAsync();
        var request = new UpdateTimeEntryRequest(new DateOnly(2026, 10, 4), "Project", 24, null);

        var result = await TimeEntryEndpoints.UpdateTimeEntry(entry.Id, request, db);

        Assert.Equal(StatusCodes.Status200OK, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        Assert.Equal(24, entry.Hours);
    }

    [Fact]
    public async Task R7_UpdateEntry_WithMissingId_Returns404()
    {
        await using var db = CreateDbContext();
        var request = new UpdateTimeEntryRequest(new DateOnly(2026, 10, 4), "Project", 1, null);

        var result = await TimeEntryEndpoints.UpdateTimeEntry(123, request, db);

        Assert.Equal(StatusCodes.Status404NotFound, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        Assert.Empty(await db.TimeEntries.ToListAsync());
    }

    [Fact]
    public async Task R6_DeleteEntry_WithExistingId_RemovesEntryAndReturns204()
    {
        await using var db = CreateDbContext();
        var entry = AddEntry(db);
        await db.SaveChangesAsync();

        var result = await TimeEntryEndpoints.DeleteTimeEntry(entry.Id, db);

        Assert.Equal(StatusCodes.Status204NoContent, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        Assert.Empty(await db.TimeEntries.ToListAsync());
    }

    [Fact]
    public async Task R7_DeleteEntry_WithMissingId_Returns404()
    {
        await using var db = CreateDbContext();

        var result = await TimeEntryEndpoints.DeleteTimeEntry(123, db);

        Assert.Equal(StatusCodes.Status404NotFound, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        Assert.Empty(await db.TimeEntries.ToListAsync());
    }

    private static TimeEntry AddEntry(TimesheetDbContext db)
    {
        var entry = new TimeEntry
        {
            Date = new DateOnly(2026, 10, 3),
            Project = "Original project",
            Hours = 4
        };
        db.TimeEntries.Add(entry);
        return entry;
    }

    private static TimesheetDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TimesheetDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TimesheetDbContext(options);
    }
}
