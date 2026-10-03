using Microsoft.EntityFrameworkCore;

namespace Timesheet.Api.Data;

public class TimesheetDbContext(DbContextOptions<TimesheetDbContext> options) : DbContext(options)
{
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
}
