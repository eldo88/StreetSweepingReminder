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
    
    public Task<int> CreateReminder(Reminder reminder)
    {
        throw new NotImplementedException();
    }
}