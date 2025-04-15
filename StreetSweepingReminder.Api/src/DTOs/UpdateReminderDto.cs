namespace StreetSweepingReminder.Api.DTOs;

public record UpdateReminderDto(
    int Id, 
    string Title, 
    string Status, 
    string PhoneNumber, 
    int StreetId, 
    DateTime? ModifiedAt);