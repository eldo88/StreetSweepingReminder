using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreetSweepingReminder.Api.Entities;

[Table("Reminders")]
public class Reminder
{
    [Key]
    public int Id { get; set; } //PK
    
    [Required]
    public string UserId { get; set; } = string.Empty; // FK to User
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Status { get; set; } = string.Empty; // make this into static string
    
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    public int StreetId { get; set; } // FK to Street
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }

    [ForeignKey("UserId")] 
    public virtual User? User { get; set; }
    
    [ForeignKey("StreetId")]
    public virtual Street? Street { get; set; }
    
    public virtual ICollection<ReminderSchedule> ReminderSchedules { get; set; } = new List<ReminderSchedule>();

    public Reminder()
    {
        CreatedAt = DateTime.UtcNow;
    }
}