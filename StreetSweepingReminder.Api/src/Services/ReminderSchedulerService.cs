using FluentResults;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Services.Utils;

namespace StreetSweepingReminder.Api.Services;

public class ReminderSchedulerService : IReminderScheduler
{
    private readonly ILogger<ReminderSchedulerService> _logger;

    public ReminderSchedulerService(ILogger<ReminderSchedulerService> logger)
    {
        _logger = logger;
    }

    public Task<Result> ScheduleNotificationJobAsync(int id, DateTime nextSweepTime)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> CreateReminderNotificationSchedule(CreateReminderDto command, int reminderId)
    {
        var interval = 2;
        var startDateTime = command.ScheduledDateTimeUtc;

        var reminderSchedule =  CalculateReminderSchedule(startDateTime, interval);

        foreach (var reiminder in reminderSchedule)
        {
            _logger.Log(LogLevel.Debug, "Reminder: {r}", reiminder);
        }
        return Result.Ok();
    }

    private List<DateTime?> CalculateReminderSchedule(DateTime initialReminderDateTime, int interval)
    { //TODO add validation for parameters
        var scheduledDays = new List<DateTime?>();
        var dayOfWeek = initialReminderDateTime.DayOfWeek;
        var month = initialReminderDateTime.Month;

        if (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday && month is > 3 and < 11)
        {   // set first days for reminder since that is provided by client
            scheduledDays.Add(initialReminderDateTime);
            var nextDate = initialReminderDateTime.AddDays(interval);
            scheduledDays.Add(nextDate);
            
            // Calc remaining for the year
            for (var i = month; i < 11; i++)
            {
                var nextScheduledDate = initialReminderDateTime.AddMonths(i); // bug here
                var year = nextScheduledDate.Year;
                var nextMonth = nextScheduledDate.Month;
                var dateToAdd = DateUtils.GetNthWeekdayOfMonth(year, nextMonth, dayOfWeek, interval);
                if (dateToAdd is not null)
                {
                    scheduledDays.Add(dateToAdd);
                }
            }
        }

        return scheduledDays;
    }
}