using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Repositories;

public interface IReminderRepository
{
    Task<int> CreateReminder(Reminder reminder);
}