using FluentResults;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Services;

public interface IReminderScheduler
{
    Task<Result> ScheduleNotificationJobAsync(int id, DateTime nextSweepTime);
    Task<Result> CreateReminderNotificationSchedule(CreateReminderDto command, int reminderId);
}