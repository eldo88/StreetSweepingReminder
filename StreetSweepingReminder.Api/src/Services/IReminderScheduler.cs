using FluentResults;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Services;

public interface IReminderScheduler
{
    Task<Result> CreateReminderNotificationSchedule(CreateReminderDto command, int reminderId);
}