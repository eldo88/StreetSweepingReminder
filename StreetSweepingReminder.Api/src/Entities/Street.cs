namespace StreetSweepingReminder.Api.Entities;

public class Street
{
    public int Id { get; set; } //PK
    public string UserId { get; set; } // FK to User
    public string StreetName { get; set; } = string.Empty;
    public int? HouseNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}