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
            INSERT INTO Streets (StreetName, ZipCode, CreatedAt) 
            VALUES (@StreetName, @ZipCode, @CreatedAt);
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
        var street = await connection.QuerySingleOrDefaultAsync<Street>(sql, new { id });
        return street;
    }
    
    public async Task<IEnumerable<Street>> GetByPartialStreetName(string partialStreetName)
    {
        if (string.IsNullOrWhiteSpace(partialStreetName)) // extra validation to avoid returning whole table of 4600+ items
        {
            return [];
        }
        
        var searchPattern = $"%{partialStreetName}%";
        
        const string sql =
            """
            SELECT *
            FROM Streets
            WHERE upper(StreetName)LIKE upper(@searchPattern)
            ORDER BY StreetName
            LIMIT 50
            """;

        using var connection = CreateConnection();
        var street = await connection.QueryAsync<Street>(sql, new { searchPattern });
        return street;
    }
}