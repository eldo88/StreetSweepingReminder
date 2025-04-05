using FluentResults;

namespace StreetSweepingReminder.Api.Services;

public class ReminderScheduler : IReminderScheduler
{
    public Task<Result> ScheduleNotificationJobAsync(int id, DateTime nextSweepTime)
    {
        throw new NotImplementedException();
    }
}