namespace StreetSweepingReminder.Api.DTOs;

public record StreetSweepingScheduleResponseDto(int Id, int StreetId, DateTime StreetSweepingDate, DayOfWeek DayOfWeek);