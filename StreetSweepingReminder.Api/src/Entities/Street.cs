namespace StreetSweepingReminder.Api.Entities;

public class Street
{
    public int Id { get; set; } //PK
    public string StreetName { get; set; } = string.Empty;
    public int ZipCode { get; set; }
    public DateTime CreatedAt { get; set; }
}