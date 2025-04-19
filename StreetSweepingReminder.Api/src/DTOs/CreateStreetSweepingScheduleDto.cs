namespace StreetSweepingReminder.Api.DTOs;

public record CreateStreetSweepingScheduleDto(int WeekOfMonth, int DayOfWeek, int Year);