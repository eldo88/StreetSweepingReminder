namespace StreetSweepingReminder.Api.DTOs;

public record UpdateReminderDto(
    int Id, 
    string Message, 
    DateTime ScheduledDateTimeUtc,
    DateTime StreetSweepingDate,
    string Status, 
    string PhoneNumber, 
    int StreetId, 
    DateTime? ModifiedAt);