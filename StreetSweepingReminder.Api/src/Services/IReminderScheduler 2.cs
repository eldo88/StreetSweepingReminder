using FluentResults;

namespace StreetSweepingReminder.Api.Services;

public interface IReminderScheduler
{
    Task<Result> ScheduleNotificationJobAsync(int id, DateTime nextSweepTime);
}