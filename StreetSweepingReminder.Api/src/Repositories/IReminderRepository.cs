using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Repositories;

public interface IReminderRepository : ICrudRepository<Reminder>
{
    Task<IEnumerable<Reminder>> GetRemindersByUserIdAsync(string userId);
}