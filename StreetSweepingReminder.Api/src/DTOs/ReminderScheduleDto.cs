namespace StreetSweepingReminder.Api.DTOs;

public record ReminderScheduleDto(
    int Id,
    int ReminderId,
    DateTime NextNotificationDate,
    DayOfWeek DayOfWeek,
    int WeekOfMonth,
    int StartMonth,
    int EndMonth,
    string TimeOfDay,
    string TimeZone,
    bool IsRecurring,
    bool IsActive);