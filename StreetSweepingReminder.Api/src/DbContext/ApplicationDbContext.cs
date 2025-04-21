using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.DbContext;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<Street> Streets { get; set; }
    public DbSet<Reminder> Reminders { get; set; }
    public DbSet<ReminderSchedule> ReminderSchedules { get; set; }
    public DbSet<StreetSweepingDates> StreetSweepingDates { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Reminder>()
            .Property(r => r.UserId)
            .IsRequired();
        
        builder.Entity<Reminder>()
            .HasOne(r => r.Street)
            .WithMany(s => s.Reminders)
            .HasForeignKey(r => r.StreetId)
            .IsRequired();
        
        builder.Entity<ReminderSchedule>()
            .HasOne(rs => rs.Reminder)
            .WithMany(r => r.ReminderSchedules)
            .HasForeignKey(rs => rs.ReminderId)
            .IsRequired();
        
        builder.Entity<StreetSweepingDates>()
            .HasOne(ssd => ssd.Street)
            .WithMany(s => s.StreetSweepingDates)
            .HasForeignKey(ssd => ssd.StreetId)
            .IsRequired();

    }
}