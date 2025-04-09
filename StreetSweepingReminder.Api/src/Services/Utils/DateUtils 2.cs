namespace StreetSweepingReminder.Api.Services.Utils;

public static class DateUtils
{
    public static DateTime? GetNthWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek, int weekOfMonth)
    {
        if (weekOfMonth == 0 || month < 1 || month > 12)
            return null;

        if (weekOfMonth > 0)
        {
            // Start at the 1st of the month
            var date = new DateTime(year, month, 1);

            // Find the first occurrence of the specified day
            while (date.DayOfWeek != dayOfWeek)
                date = date.AddDays(1);

            // Add (weekOfMonth - 1) * 7 days to get to the Nth occurrence
            var result = date.AddDays((weekOfMonth - 1) * 7);

            // Ensure it’s still in the same month
            return result.Month == month ? result : null;
        }
        else // Handle negative values like -1 for "last Tuesday"
        {
            // Start at the last day of the month
            var date = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            // Find the last occurrence of the specified day
            while (date.DayOfWeek != dayOfWeek)
                date = date.AddDays(-1);

            return date;
        }
    }
}