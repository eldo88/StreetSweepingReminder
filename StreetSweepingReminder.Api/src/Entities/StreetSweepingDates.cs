using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreetSweepingReminder.Api.Entities;

[Table("StreetSweepingDates")]
public class StreetSweepingDates
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int StreetId { get; set; } // FK to Streets
    public DateTime StreetSweepingDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    [ForeignKey("StreetId")]
    public virtual Street Street { get; set; } = null!;

    public StreetSweepingDates()
    {
        CreatedAt = DateTime.UtcNow;
    }
}