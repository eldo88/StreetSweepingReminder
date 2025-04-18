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
            INSERT INTO StreetSweepingDates (StreetSweepingDate, StreetId)
            VALUES (@StreetSweepingDate, @StreetId);
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

    public Task<IEnumerable<StreetSweepingDates>> GetAllAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(StreetSweepingDates obj)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<StreetSweepingDates>> GetStreetSweepingScheduleByStreetId(int streetId)
    {
        const string sql = 
            """
            SELECT Id, StreetId, StreetSweepingDate
            FROM StreetSweepingDates
            WHERE StreetId = @streetId
            """;

        using var connection = CreateConnection();
        var streetSweepingDates = await connection.QueryAsync<StreetSweepingDates>(sql, new { streetId });
        return streetSweepingDates;
    }
}