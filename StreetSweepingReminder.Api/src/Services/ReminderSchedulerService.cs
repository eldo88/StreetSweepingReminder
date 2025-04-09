using FluentResults;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Services.Utils;

namespace StreetSweepingReminder.Api.Services;

public class ReminderSchedulerService : IReminderScheduler
{
    public Task<Result> ScheduleNotificationJobAsync(int id, DateTime nextSweepTime)
    {
        throw new NotImplementedException();
    }

    public Task<Result> CreateReminderNotificationSchedule(CreateReminderDto command)
    {
        throw new NotImplementedException();
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
                var nextScheduledDate = initialReminderDateTime.AddMonths(i);
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