using StreetSweepingReminder.Api.Entities;
using Dapper;

namespace StreetSweepingReminder.Api.Repositories;

internal class ReminderRepository : RepositoryBase, IReminderRepository
{
    public ReminderRepository(IConfiguration configuration) : base(configuration)
    {
    }
    
    public async Task<int> CreateAsync(Reminder reminder)
    {
        const string sql =
            """
            INSERT INTO Reminders (UserId, Message, ScheduledDateTime, Status, PhoneNumber, StreetId, ModifiedAt) 
                                VALUES (@UserId, @Message, @ScheduledDateTime, @Status, @PhoneNumber, @StreetId, @ModifiedAt)
            """;

        using var connection = CreateConnection();
        var newId = await connection.ExecuteAsync(sql, reminder);
        return newId;
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