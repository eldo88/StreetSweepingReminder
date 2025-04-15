using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Repositories;

internal class StreetSweepingDatesRepository : RepositoryBase, IStreetSweepingDatesRepository
{
    protected StreetSweepingDatesRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public Task<int> CreateAsync(StreetSweepingDates obj)
    {
        throw new NotImplementedException();
    }

    public Task<StreetSweepingDates?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<StreetSweepingDates>> GetAllAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(StreetSweepingDates obj)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}