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

    public Task<IEnumerable<ReminderSchedule>> GetAllAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(ReminderSchedule obj)
    {
        throw new NotImplementedException();
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