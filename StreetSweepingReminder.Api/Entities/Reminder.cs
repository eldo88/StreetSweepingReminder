namespace StreetSweepingReminder.Api.Entities;

public class Reminder
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime ScheduledDateTimeUtc { get; set; }
    public string Status { get; set; } = string.Empty; // make this into static string
    public string PhoneNumber { get; set; } = string.Empty;
}