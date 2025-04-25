using StreetSweepingReminder.Api.Constants_Enums;

namespace StreetSweepingReminder.Api.DTOs;

public record StreetSweepingScheduleResponseDto(
    int DayOfWeek, 
    int WeekOfMonth, 
    int Year,
    int StreetId,
    CardinalDirection SideOfStreet,
    List<StreetSweepingScheduleDto> Schedule);