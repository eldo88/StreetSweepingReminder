namespace StreetSweepingReminder.Api.DTOs;

public record CreateReminderDto(
    string Message, 
    DateTime ScheduledDateTimeUtc,
    DateTime StreetSweepingDate,
    string PhoneNumber, 
    int StreetId, 
    int WeekOfMonth,
    bool IsRecurring);