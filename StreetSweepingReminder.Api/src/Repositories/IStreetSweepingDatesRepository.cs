using StreetSweepingReminder.Api.Constants_Enums;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Repositories;

public interface IStreetSweepingDatesRepository : ICrudRepository<StreetSweepingDates>
{
    Task<IEnumerable<StreetSweepingDates>> GetStreetSweepingScheduleByStreetId(int streetId);

    Task<IEnumerable<StreetSweepingDates>> GetScheduleBuStreetIdAndSideOfStreet(int streetId,
        CardinalDirection sideOfStreet);
}