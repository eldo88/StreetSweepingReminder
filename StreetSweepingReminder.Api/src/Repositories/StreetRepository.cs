using StreetSweepingReminder.Api.Entities;
using Dapper;

namespace StreetSweepingReminder.Api.Repositories;

internal class StreetRepository : RepositoryBase, IStreetRepository
{
    public StreetRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int> CreateAsync(Street street)
    {
        const string sql =
            """
            INSERT INTO Streets (UserId, StreetName) 
            VALUES (@UserId, @StreetName);
            SELECT last_insert_rowid();
            """;

        using var connection = CreateConnection();
        var newId = await connection.ExecuteScalarAsync<int>(sql, street);
        return newId;
    }

    public async Task<Street?> GetByIdAsync(int id)
    {
        const string sql =
            """
            SELECT *
            FROM Streets
            WHERE ID = @id
            """;

        using var connection = CreateConnection();
        var street = await connection.QuerySingleOrDefaultAsync(sql, new { id });
        return street;
    }
    
}