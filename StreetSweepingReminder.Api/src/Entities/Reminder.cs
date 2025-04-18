namespace StreetSweepingReminder.Api.Entities;

public class Reminder
{
    public int Id { get; set; } //PK
    public string UserId { get; set; } = string.Empty; // FK to User
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // make this into static string
    public string PhoneNumber { get; set; } = string.Empty;
    public int StreetId { get; set; } // FK to Street
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}