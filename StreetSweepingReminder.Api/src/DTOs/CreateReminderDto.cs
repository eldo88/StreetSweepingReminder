namespace StreetSweepingReminder.Api.DTOs;

public record CreateReminderDto(
    string Title, 
    DateTime ScheduledDateTimeUtc,
    DateTime StreetSweepingDate,
    string PhoneNumber, 
    int StreetId, 
    int WeekOfMonth,
    bool IsRecurring);