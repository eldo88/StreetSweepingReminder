using Dapper;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Repositories;

internal class ReminderScheduleRepository : RepositoryBase, IReminderScheduleRepository
{
    public ReminderScheduleRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int> CreateAsync(ReminderSchedule reminderSchedule)
    {
        const string sql =
            """
            INSERT INTO ReminderSchedule (ReminderId ,Message,DayOfWeek, NextNotificationDate, WeekOfMonth, StartMonth, EndMonth, TimeOfDay, TimeZone, IsRecurring, IsActive, CreatedAt)
            VALUES (@ReminderId, @Message, @DayOfWeek, @NextNotificationDate, @WeekOfMonth, @StartMonth, @EndMonth, @TimeOfDay, @TimeZone, @IsRecurring, @IsActive, @CreatedAt);
            SELECT last_insert_rowid();
            """;
        using var connection = CreateConnection();
        var newId = await connection.ExecuteScalarAsync<int>(sql, reminderSchedule);
        return newId;
    }

    public async Task<ReminderSchedule?> GetByIdAsync(int id)
    {
        const string sql =
            """
            SELECT *
            FROM ReminderSchedule
            WHERE Id = @id
            """;

        using var connection = CreateConnection();
        var reminderDay = await connection.QuerySingleOrDefaultAsync<ReminderSchedule>(sql, new { id });
        return reminderDay;
    }

    public async Task<IEnumerable<ReminderSchedule>> GetAllAsync(string userId)
    {
        const string sql =
            """
            SELECT rs.Id, rs.ReminderId, rs.NextNotificationDate, rs.NextNotificationDate, rs.DayOfWeek, rs.WeekOfMonth,
                   rs.StartMonth, rs.EndMonth, rs.TimeOfDay, rs.TimeZone, rs.IsRecurring, rs.IsActive
            FROM ReminderSchedule as rs
                JOIN Reminders as r
                    ON rs.ReminderId = r.Id
                    AND r.UserId = @userId
            """;

        using var connection = CreateConnection();
        var schedule = await connection.QueryAsync<ReminderSchedule>(sql, new { userId });
        return schedule;
    }

    public async Task<bool> UpdateAsync(ReminderSchedule obj)
    {
        const string sql =
            """
            UPDATE ReminderSchedule
            SET Message = @Message, NextNotificationDate = @NextNotificationDate, DayOfWeek = @DayOfWeek, 
                WeekOfMonth = @WeekOfMonth, StartMonth = @StartMonth, EndMonth = @EndMonth, TimeOfDay = @TimeOfDay, 
                TimeZone = @TimeZone, IsRecurring = @IsRecurring, IsActive = @IsActive, ModifiedAt = @ModifiedAt
            WHERE Id = @Id
            """;

        using var connection = CreateConnection();
        var recordsUpdated = await connection.ExecuteAsync(sql, obj);
        return recordsUpdated == 1;
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ReminderSchedule>> GetByReminderId(int reminderId)
    {
        const string sql =
            """
            SELECT *
            FROM ReminderSchedule
            WHERE ReminderId = @reminderId
            """;

        using var connection = CreateConnection();
        var reminderDates = await connection.QueryAsync<ReminderSchedule>(sql, new { reminderId });
        return reminderDates;
    }
}