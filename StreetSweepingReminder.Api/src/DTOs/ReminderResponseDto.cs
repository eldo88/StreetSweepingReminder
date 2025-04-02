using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.DTOs;

public record ReminderResponseDto(string Message, DateTime ScheduledDateTimeUtc, string Status, string PhoneNumber, int StreetId);