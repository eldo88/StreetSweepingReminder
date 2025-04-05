namespace StreetSweepingReminder.Api.DTOs;

public record UpdateReminderDto(
    int Id, 
    string Message, 
    DateTime ScheduledDateTimeUtc, 
    string Status, 
    string PhoneNumber, 
    int StreetId, 
    DateTime? ModifiedAt);