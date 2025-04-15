using FluentResults;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Extensions;
using StreetSweepingReminder.Api.Repositories;
using StreetSweepingReminder.Api.Services.Utils;

namespace StreetSweepingReminder.Api.Services;

public class ReminderSchedulerService : IReminderScheduler
{
    private readonly ILogger<ReminderSchedulerService> _logger;
    private readonly IReminderScheduleRepository _reminderScheduleRepository;

    public ReminderSchedulerService(ILogger<ReminderSchedulerService> logger, IReminderScheduleRepository reminderScheduleRepository)
    {
        _logger = logger;
        _reminderScheduleRepository = reminderScheduleRepository;
    }
    
    public async Task<Result> CreateReminderNotificationSchedule(CreateReminderDto command, int reminderId)
    {
        if (command.IsRecurring)
        {
            var reminderDates =  DateUtils.CalculateMonthlyRecurringSchedule(command.ScheduledDateTimeUtc, command.WeekOfMonth);

            foreach (var reminder in reminderDates)
            {
                _logger.Log(LogLevel.Debug, "Reminder: {r}", reminder);
                var reminderSchedule = command.ToReminderScheduleEntity(reminderId);
                reminderSchedule.ReminderDate = reminder;
                reminderSchedule.IsActive = true;
                var result = await SaveReminderScheduleAsync(reminderSchedule);
                if (result.IsFailed)
                {
                    return result;
                }
            }
        }
        else
        {
            var reminderSchedule = command.ToReminderScheduleEntity(reminderId);
            reminderSchedule.IsActive = true;
            var result = await SaveReminderScheduleAsync(reminderSchedule);
            if (result.IsFailed)
            {
                return result;
            }
        }
        
        return Result.Ok();
    }

    private async Task<Result> SaveReminderScheduleAsync(ReminderSchedule reminderSchedule)
    {
        try
        {
            var newId = await _reminderScheduleRepository.CreateAsync(reminderSchedule);
            if (newId <= 0)
            {
                return Result.Fail(new ApplicationError("Invalid ID saved to the database"));
            }

            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating reminder schedule.");
            return Result.Fail(new ExceptionalError("Unexpected error occurred saving reminder schedule.", e));
        }
    }
}