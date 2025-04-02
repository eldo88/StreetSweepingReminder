using StreetSweepingReminder.Api.Entities;
using Dapper;

namespace StreetSweepingReminder.Api.Repositories;

public class ReminderRepository : IReminderRepository
{
    private readonly string _connectionString;

    public ReminderRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
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