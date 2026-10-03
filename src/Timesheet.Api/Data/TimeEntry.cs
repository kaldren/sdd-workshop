namespace Timesheet.Api.Data;

public class TimeEntry
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string Project { get; set; } = string.Empty;

    public decimal Hours { get; set; }

    public string? Description { get; set; }
}
