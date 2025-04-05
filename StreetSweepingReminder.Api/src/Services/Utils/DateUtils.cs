namespace StreetSweepingReminder.Api.Services.Utils;

public static class DateUtils
{
    /// <summary>
    /// Finds the date of the Nth specified weekday in a given month and year.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month (1-12).</param>
    /// <param name="targetDayOfWeek">The desired day of the week (e.g., DayOfWeek.Tuesday).</param>
    /// <param name="occurrence">The occurrence (1 for 1st, 2 for 2nd, etc.). Must be between 1 and 4.</param>
    /// <returns>The DateTime of the Nth weekday, or null if it doesn't exist (e.g., asking for the 5th Tuesday).</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if occurrence is not between 1 and 4.</exception>
    public static DateTime? FindNthWeekdayOfMonth(int year, int month, DayOfWeek targetDayOfWeek, int occurrence)
    {
        if (occurrence < 1 || occurrence > 4) // Typically a month has max 4 full weeks + partials
        {
             throw new ArgumentOutOfRangeException(nameof(occurrence), "Occurrence must be between 1 and 4. Use FindLastWeekdayOfMonth for the last occurrence.");
        }

        try
        {
            DateTime firstDayOfMonth = new DateTime(year, month, 1);

            // Calculate the day of the week for the first day of the month (0=Sunday, 1=Monday...)
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

            // Calculate how many days to add to the 1st to get to the *first* target DayOfWeek
            int daysToAdd = (int)targetDayOfWeek - firstDayOfWeek;
            if (daysToAdd < 0)
            {
                daysToAdd += 7; // Adjust if the target day is earlier in the week than the 1st day
            }

            // Calculate the date of the *first* occurrence of the target DayOfWeek
            DateTime firstOccurrenceDate = firstDayOfMonth.AddDays(daysToAdd);

            // Calculate the date of the Nth occurrence by adding weeks
            DateTime nthOccurrenceDate = firstOccurrenceDate.AddDays(7 * (occurrence - 1));

            // *** Crucial Check ***: Ensure the calculated date is still within the same month!
            // If adding the weeks pushed it into the next month, the Nth occurrence doesn't exist.
            if (nthOccurrenceDate.Month == month)
            {
                return nthOccurrenceDate;
            }
            else
            {
                return null; // The Nth occurrence does not exist in this month
            }
        }
        catch (ArgumentOutOfRangeException ex) // Catches invalid year/month
        {
            Console.WriteLine($"Error creating date: Invalid year ({year}) or month ({month}). {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Finds the date of the last specified weekday in a given month and year.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month (1-12).</param>
    /// <param name="targetDayOfWeek">The desired day of the week (e.g., DayOfWeek.Tuesday).</param>
    /// <returns>The DateTime of the last specified weekday in the month.</returns>
    public static DateTime FindLastWeekdayOfMonth(int year, int month, DayOfWeek targetDayOfWeek)
    {
         try
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DateTime lastDayOfMonth = new DateTime(year, month, daysInMonth);

            // Calculate the day of the week for the last day of the month
            int lastDayOfWeek = (int)lastDayOfMonth.DayOfWeek;

            // Calculate how many days to subtract from the last day to get to the *last* target DayOfWeek
            int daysToSubtract = lastDayOfWeek - (int)targetDayOfWeek;
            if (daysToSubtract < 0)
            {
                daysToSubtract += 7; // Adjust if the target day is later in the week than the last day
            }

            DateTime lastOccurrenceDate = lastDayOfMonth.AddDays(-daysToSubtract);
            return lastOccurrenceDate;
         }
        catch (ArgumentOutOfRangeException ex) // Catches invalid year/month
        {
            Console.WriteLine($"Error creating date: Invalid year ({year}) or month ({month}). {ex.Message}");
             // Re-throw or handle appropriately - maybe return default(DateTime)?
             // Throwing is often better to indicate invalid input.
            throw;
        }
    }
}