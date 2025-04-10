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

    public Task<Result> ScheduleNotificationJobAsync(int id, DateTime nextSweepTime)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> CreateReminderNotificationSchedule(CreateReminderDto command, int reminderId)
    {
        if (command.IsRecurring)
        {
            var reminderDates =  CalculateRecurringReminderSchedule(command.ScheduledDateTimeUtc, command.WeekOfMonth);

            foreach (var reminder in reminderDates)
            {
                //_logger.Log(LogLevel.Debug, "Reminder: {r}", reminder);
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
    private List<DateTime> CalculateRecurringReminderSchedule(DateTime initialReminderDateTime, int weekOfMonth)
    { 
        var scheduledDays = new List<DateTime>();
        var dayOfWeek = initialReminderDateTime.DayOfWeek;
        var startingMonth = initialReminderDateTime.Month;

        if (dayOfWeek is < DayOfWeek.Saturday and > DayOfWeek.Sunday && startingMonth is >= 4 and <= 11)
        {   // set first days for reminder since that is provided by client
            scheduledDays.Add(initialReminderDateTime);
            
            // Calc remaining for the year
            var dateToAdd = initialReminderDateTime;
            for (var i = startingMonth; i < 11; i++)
            {
                var (year, month, _) = dateToAdd.AddMonths(1); // need to extract time
                dateToAdd = DateUtils.GetNthWeekdayOfMonth(year, month, dayOfWeek, weekOfMonth);
                scheduledDays.Add(dateToAdd);
            }
        }

        return scheduledDays;
    }
}