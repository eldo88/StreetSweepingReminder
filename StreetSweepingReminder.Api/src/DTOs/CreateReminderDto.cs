using StreetSweepingReminder.Api.Constants_Enums;

namespace StreetSweepingReminder.Api.DTOs;

public record CreateReminderDto(
    string Title, 
    DateTime ScheduledDateTimeUtc,
    string PhoneNumber, 
    int StreetId, 
    bool IsRecurring,
    CardinalDirection SideOfStreet);