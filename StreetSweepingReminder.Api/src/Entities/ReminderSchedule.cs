namespace StreetSweepingReminder.Api.Entities;

public class ReminderSchedule
{
    public int Id { get; set; }
    public int ReminderId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime ReminderDate { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public int WeekOfMonth { get; set; }
    public int StartMonth { get; set; }
    public int EndMonth { get; set; }
    public string TimeOfDay { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
}