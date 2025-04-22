using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Repositories;

public interface IReminderScheduleRepository: ICrudRepository<ReminderSchedule>
{
    Task<IEnumerable<ReminderSchedule>> GetByReminderId(int reminderId);
}