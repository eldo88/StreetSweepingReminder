
namespace StreetSweepingReminder.Api.DTOs;

public record ReminderResponseDto(int Id, string Message, DateTime ScheduledDateTimeUtc, string Status, string PhoneNumber, int StreetId);