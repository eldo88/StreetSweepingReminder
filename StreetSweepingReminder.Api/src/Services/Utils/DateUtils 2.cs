namespace StreetSweepingReminder.Api.Services.Utils;

public static class DateUtils
{
    public static DateTime GetNthWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek, int weekOfMonth)
    {
        if (weekOfMonth == 0 || month < 1 || month > 12)
        {
            return default;   
        }

        if (weekOfMonth > 0)
        {
            var date = new DateTime(year, month, 1);

            while (date.DayOfWeek != dayOfWeek)
            {
                date = date.AddDays(1);   
            }
            
            var result = date.AddDays((weekOfMonth - 1) * 7);
            
            return result;
        }
        else // Handle negative values like -1 for "last Tuesday", maybe not needed
        {
            var date = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            while (date.DayOfWeek != dayOfWeek)
            {
                date = date.AddDays(-1);   
            }

            return date;
        }
    }

    public static List<DateTime> CalculateMonthlyRecurringSchedule(DateTime initialReminderDateTime, int weekOfMonth)
    {
        var scheduledDays = new List<DateTime>();
        var dayOfWeek = initialReminderDateTime.DayOfWeek;
        var startingMonth = initialReminderDateTime.Month;

        if (startingMonth is >= 4 and <= 11)
        {   // set first days for reminder since that is provided by client
            scheduledDays.Add(initialReminderDateTime);
            
            // Calc remaining for the year
            var dateToAdd = initialReminderDateTime;
            for (var i = startingMonth; i < 11; i++)
            {
                var (year, month, _) = dateToAdd.AddMonths(1); // need to extract time
                dateToAdd = GetNthWeekdayOfMonth(year, month, dayOfWeek, weekOfMonth);
                scheduledDays.Add(dateToAdd);
            }
        }

        return scheduledDays;
    }

    public static int GetWeekOfMonth(DateTime date)
    {
        var day = date.Day;
        var year = date.Year;
        var month = date.Month;
        var weekCount = 1;
        
        var firstDayOfTheMonth = new DateTime(year, month, 1);

        for (var i = 0; i < day; i++)
        {
            if (firstDayOfTheMonth.DayOfWeek == DayOfWeek.Sunday)
            {
                weekCount++;
            }

            firstDayOfTheMonth = firstDayOfTheMonth.AddDays(1);
        }

        return weekCount;
    }
}