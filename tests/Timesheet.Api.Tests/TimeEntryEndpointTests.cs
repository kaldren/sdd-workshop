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

    private static TimesheetDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TimesheetDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TimesheetDbContext(options);
    }
}
