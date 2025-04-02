using StreetSweepingReminder.Api.Entities;
using Dapper;

namespace StreetSweepingReminder.Api.Repositories;

internal class ReminderRepository : RepositoryBase, IReminderRepository
{
    public ReminderRepository(IConfiguration configuration) : base(configuration)
    {
    }
    
    public Task<int> CreateAsync(Reminder reminder)
    {
        throw new NotImplementedException();
    }

    public Task<Reminder> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Reminder>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Reminder obj)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}