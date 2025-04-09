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
            INSERT INTO Reminders (UserId, Message, ScheduledDateTimeUtc, Status, PhoneNumber, StreetId, ScheduleId) 
            VALUES (@UserId, @Message, @ScheduledDateTimeUtc, @Status, @PhoneNumber, @StreetId, @ScheduleId);
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

    public async Task<IEnumerable<Reminder>> GetAllAsync(string userId)
    {
        const string sql =
            """
            SELECT * 
            FROM Reminders
            WHERE UserId = @userId
            """;

        using var connection = CreateConnection();
        var reminders = await connection.QueryAsync<Reminder>(sql, new { userId });
        return reminders;
    }

    public async Task<bool> UpdateAsync(Reminder reminder)
    {
        const string sql =
            """
            UPDATE Reminders
            SET Message = @Message, ScheduledDateTimeUtc = @ScheduledDateTimeUtc, Status = @Status, 
            PhoneNumber = @PhoneNumber, StreetId = @StreetId, ModifiedAt = @ModifiedAt, ScheduleId = @ScheduleId
            WHERE ID = @Id
            """;

        using var connection = CreateConnection();
        var recordsUpdate = await connection.ExecuteAsync(sql, reminder);
        return recordsUpdate == 1;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql =
            """
            DELETE 
            FROM Reminders
            WHERE ID = @id
            """;

        using var connection = CreateConnection();
        var recordsDeleted = await connection.ExecuteAsync(sql, new { id });
        return recordsDeleted == 1;
    }
}