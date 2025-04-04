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
            VALUES (@UserId, @Message, @ScheduledDateTimeUtc, @Status, @PhoneNumber, @StreetId);
            SELECT last_insert_rowid();
            """;

        using var connection = CreateConnection();
        var newId = await connection.ExecuteScalarAsync<int>(sql, reminder);
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
        var reminder = await connection.QuerySingleOrDefaultAsync<Reminder>(sql, new { id });
        return reminder;
    }

    public Task<IEnumerable<Reminder>> GetAllAsync(string userId)
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