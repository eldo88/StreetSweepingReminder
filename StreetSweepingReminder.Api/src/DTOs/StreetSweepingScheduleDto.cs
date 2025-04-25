using StreetSweepingReminder.Api.Constants_Enums;

namespace StreetSweepingReminder.Api.DTOs;

public record StreetSweepingScheduleDto(int Id, int StreetId, DateTime StreetSweepingDate, CardinalDirection SideOfStreet);