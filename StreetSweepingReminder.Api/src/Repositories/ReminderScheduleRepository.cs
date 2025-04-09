using Dapper;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Repositories;

internal class ReminderScheduleRepository : RepositoryBase, IReminderRepository
{
    protected ReminderScheduleRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public Task<int> CreateAsync(Reminder reminder)
    {
        const string sql =
            """
            INSERT INTO ReminderSchedule (ReminderId ,Message,DayOfWeek, ReminderDate, WeekOfMonth, StartMonth, EndMonth, TimeOfDay, TimeZone, IsRecurring)
            VALUES (@ReminderId, @Message, @DayOfWeek, @ReminderDate, @WeekOfMonth, @StartMonth, @EndMonth, @TimeOfDay, @TimeZone, @IsRecurring);
            SELECT last_insert_rowid();
            """;
        using var connection = CreateConnection();
        var newId = connection.ExecuteScalarAsync<int>(sql, reminder);
        return newId;
    }

    public Task<Reminder?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
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