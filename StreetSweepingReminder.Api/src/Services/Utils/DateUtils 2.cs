namespace StreetSweepingReminder.Api.Services.Utils;

public static class DateUtils
{
    public static DateTime? GetNthWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek, int weekOfMonth)
    {
        if (weekOfMonth == 0 || month < 1 || month > 12)
            return null;

        if (weekOfMonth > 0)
        {
            var date = new DateTime(year, month, 1);
            
            while (date.DayOfWeek != dayOfWeek)
                date = date.AddDays(1);
            
            var result = date.AddDays((weekOfMonth - 1) * 7);
            
            return result.Month == month ? result : null;
        }
        else // Handle negative values like -1 for "last Tuesday", maybe not needed
        {
            var date = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            
            while (date.DayOfWeek != dayOfWeek)
                date = date.AddDays(-1);

            return date;
        }
    }
}