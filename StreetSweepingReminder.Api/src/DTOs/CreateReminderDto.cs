namespace StreetSweepingReminder.Api.DTOs;

public record CreateReminderDto(
    string Message, 
    DateTime ScheduledDateTimeUtc, 
    string PhoneNumber, 
    int StreetId, 
    int WeekOfMonth,
    bool IsRecurring);