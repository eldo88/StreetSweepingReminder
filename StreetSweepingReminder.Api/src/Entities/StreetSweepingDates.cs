namespace StreetSweepingReminder.Api.Entities;

public class StreetSweepingDates
{
    public int Id { get; set; }
    public int StreetId { get; set; } // FK to Streets
    public DateTime StreetSweepingDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}