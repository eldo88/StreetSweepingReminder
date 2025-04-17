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

    public Task<StreetSweepingDates?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
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
}