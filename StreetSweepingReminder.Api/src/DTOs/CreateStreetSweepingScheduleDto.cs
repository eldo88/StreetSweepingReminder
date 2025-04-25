using StreetSweepingReminder.Api.Constants_Enums;

namespace StreetSweepingReminder.Api.DTOs;

public record CreateStreetSweepingScheduleDto(int WeekOfMonth, int DayOfWeek, int Year, CardinalDirection SideOfStreet);