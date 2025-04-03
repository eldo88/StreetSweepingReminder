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
            INSERT INTO Reminders (UserId, Message, ScheduledDateTime, Status, PhoneNumber, StreetId) 
                                VALUES (@UserId, @Message, @ScheduledDateTime, @Status, @PhoneNumber, @StreetId)
            """;

        using var connection = CreateConnection();
        var newId = await connection.ExecuteAsync(sql, reminder);
        return newId;
    }

    public async Task<Reminder?> GetByIdAsync(int id)
    {
        const string sql =
            """
            SELECT *
            FROM Reminders
            WHERE ID = @id           
            """;

        using var connection = CreateConnection();
        var reminder = await connection.QuerySingleOrDefaultAsync<Reminder>(sql, new { id = id });
        return reminder;
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