namespace StreetSweepingReminder.Api.DTOs;

public record CreateReminderDto(
    string Title, 
    DateTime ScheduledDateTimeUtc,
    string PhoneNumber, 
    int StreetId, 
    bool IsRecurring);