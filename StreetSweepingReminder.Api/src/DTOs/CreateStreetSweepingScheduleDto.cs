namespace StreetSweepingReminder.Api.DTOs;

public record CreateStreetSweepingScheduleDto(DateTime StreetSweepingDate, int WeekOfMonth);