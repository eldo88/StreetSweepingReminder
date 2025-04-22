
namespace StreetSweepingReminder.Api.DTOs;

public record ReminderResponseDto(
    int Id, 
    string Title, 
    string Status, 
    string PhoneNumber, 
    int StreetId,
    StreetSweepingScheduleResponseDto StreetSweepingSchedule,
    ReminderScheduleResponseDto ReminderSchedule);
    