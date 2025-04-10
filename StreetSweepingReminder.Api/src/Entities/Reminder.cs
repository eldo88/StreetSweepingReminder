namespace StreetSweepingReminder.Api.Entities;

public class Reminder
{
    public int Id { get; set; } //PK
    public string UserId { get; set; } = string.Empty; // FK to User
    public string Message { get; set; } = string.Empty;
    public DateTime ScheduledDateTimeUtc { get; set; }
    public DateTime StreetSweepingDate { get; set; }
    public string Status { get; set; } = string.Empty; // make this into static string
    public string PhoneNumber { get; set; } = string.Empty;
    public int StreetId { get; set; } // FK to Street
    public int ScheduleId { get; set; } // FK to reminder schedule
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}