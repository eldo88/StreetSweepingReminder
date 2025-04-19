namespace StreetSweepingReminder.Api.DTOs;

public record StreetSweepingScheduleResponseDto(
    int DayOfWeek, 
    int WeekOfMonth, 
    int Year,
    int StreetId,
    List<StreetSweepingScheduleDto> Schedule);