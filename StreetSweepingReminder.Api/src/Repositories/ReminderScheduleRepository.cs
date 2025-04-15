using Dapper;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Repositories;

internal class ReminderScheduleRepository : RepositoryBase, IReminderScheduleRepository
{
    public ReminderScheduleRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public Task<int> CreateAsync(ReminderSchedule reminderSchedule)
    {
        const string sql =
            """
            INSERT INTO ReminderSchedule (ReminderId ,Message,DayOfWeek, NextNotificationDate, WeekOfMonth, StartMonth, EndMonth, TimeOfDay, TimeZone, IsRecurring, IsActive)
            VALUES (@ReminderId, @Message, @DayOfWeek, @NextNotificationDate, @WeekOfMonth, @StartMonth, @EndMonth, @TimeOfDay, @TimeZone, @IsRecurring, @IsActive);
            SELECT last_insert_rowid();
            """;
        using var connection = CreateConnection();
        var newId = connection.ExecuteScalarAsync<int>(sql, reminderSchedule);
        return newId;
    }

    public Task<ReminderSchedule?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
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
}