using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StreetSweepingReminder.Api.Constants_Enums;

namespace StreetSweepingReminder.Api.Entities;

[Table("Reminders")]
public class Reminder
{
    [Key]
    public int Id { get; set; } //PK
    
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty; // FK to User
    
    [Required]
    [MaxLength(256)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Status { get; set; } = string.Empty; // make this into enum
    
    [Required]
    [MaxLength(35)]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    public int StreetId { get; set; } // FK to Street
    public CardinalDirection SideOfStreet { get; set; }
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