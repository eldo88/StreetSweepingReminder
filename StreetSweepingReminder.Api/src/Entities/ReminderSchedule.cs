using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreetSweepingReminder.Api.Entities;

[Table("ReminderSchedule")]
public class ReminderSchedule
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int ReminderId { get; set; }
    
    [Required]
    [MaxLength(256)]
    public string Message { get; set; } = string.Empty;
    public DateTime NextNotificationDate { get; set; }
    
    [Required]
    public DayOfWeek DayOfWeek { get; set; }
    
    [Required]
    public int WeekOfMonth { get; set; }
    
    [Required]
    public int StartMonth { get; set; }
    
    [Required]
    public int EndMonth { get; set; }
    
    [Required]
    [MaxLength(35)]
    public string TimeOfDay { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(35)]
    public string TimeZone { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    [ForeignKey("ReminderId")]
    public virtual Reminder Reminder { get; set; } = null!;

    public ReminderSchedule()
    {
        CreatedAt = DateTime.UtcNow;
    }
}