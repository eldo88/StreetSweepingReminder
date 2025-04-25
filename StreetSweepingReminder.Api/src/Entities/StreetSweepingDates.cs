using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StreetSweepingReminder.Api.Constants_Enums;

namespace StreetSweepingReminder.Api.Entities;

[Table("StreetSweepingDates")]
public class StreetSweepingDates
{   //TODO added side of street or cardinal direction for sorting/filtering
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int StreetId { get; set; } // FK to Streets
    public DateTime StreetSweepingDate { get; set; }
    public CardinalDirection SideOfStreet { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    [ForeignKey("StreetId")]
    public virtual Street? Street { get; set; }

    public StreetSweepingDates()
    {
        CreatedAt = DateTime.UtcNow;
    }
}