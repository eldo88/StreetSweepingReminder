using Dapper;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Repositories;

internal class StreetSweepingDatesRepository : RepositoryBase, IStreetSweepingDatesRepository
{
    public StreetSweepingDatesRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int> CreateAsync(StreetSweepingDates obj)
    {
        const string sql =
            """
            INSERT INTO StreetSweepingDates (StreetSweepingDate, StreetId, CreatedAt, SideOfStreet)
            VALUES (@StreetSweepingDate, @StreetId, @CreatedAt, @SideOfStreet);
            SELECT last_insert_rowid();
            """;

        using var connection = CreateConnection();
        var newId = await connection.ExecuteScalarAsync<int>(sql, obj);
        return newId;
    }

    public async Task<StreetSweepingDates?> GetByIdAsync(int id)
    {
        const string sql =
            """
            SELECT *
            FROM StreetSweepingDates
            WHERE ID = @id
            """;

        using var connection = CreateConnection();
        var streetSweepingDate = await connection.QuerySingleOrDefaultAsync<StreetSweepingDates>(sql, new { id });
        return streetSweepingDate;
    }

    public async Task<IEnumerable<StreetSweepingDates>> GetAllAsync(string userId)
    {
        const string sql =
            """
            SELECT ssd.Id, ssd.StreetId, ssd.StreetSweepingDate, ssd.CreatedAt, ssd.ModifiedAt
            FROM StreetSweepingDates as ssd
                JOIN Streets as s
                    ON ssd.StreetId = s.Id
                JOIN Reminders as r
                    ON s.Id = r.StreetId
                AND r.UserId = @userId
            """;

        using var connection = CreateConnection();
        var dates = await connection.QueryAsync<StreetSweepingDates>(sql, new { userId });
        return dates;
    }

    public async Task<bool> UpdateAsync(StreetSweepingDates obj)
    {
        const string sql =
            """
            UPDATE StreetSweepingDates
            SET StreetSweepingDate = @StreetSweepingDate, ModifiedAt = @ModifiedAt
            WHERE Id = @Id
            """;

        using var connection = CreateConnection();
        var recordsUpdated = await connection.ExecuteAsync(sql, obj);
        return recordsUpdated == 1;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql =
            """
            DELETE
            FROM StreetSweepingDates
            WHERE Id = @id
            """;

        using var connection = CreateConnection();
        var recordsDeleted = await connection.ExecuteAsync(sql, new { id });
        return recordsDeleted == 1;
    }

    public async Task<IEnumerable<StreetSweepingDates>> GetStreetSweepingScheduleByStreetId(int streetId)
    {
        const string sql = 
            """
            SELECT Id, StreetId, StreetSweepingDate, SideOfStreet
            FROM StreetSweepingDates
            WHERE StreetId = @streetId
            """;

        using var connection = CreateConnection();
        var streetSweepingDates = await connection.QueryAsync<StreetSweepingDates>(sql, new { streetId });
        return streetSweepingDates;
    }
}