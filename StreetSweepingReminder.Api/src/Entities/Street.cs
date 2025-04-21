using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreetSweepingReminder.Api.Entities;

[Table("Streets")]
public class Street
{
    [Key]
    public int Id { get; set; } //PK
    
    [Required]
    public string StreetName { get; set; } = string.Empty;
    
    [Required]
    public int ZipCode { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    
    public virtual ICollection<StreetSweepingDates> StreetSweepingDates { get; set; } = new List<StreetSweepingDates>();
    
    public Street()
    {
        CreatedAt = DateTime.UtcNow;
    }
}