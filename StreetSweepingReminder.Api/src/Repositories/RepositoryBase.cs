using System.Data;
using Microsoft.Data.Sqlite;

namespace StreetSweepingReminder.Api.Repositories;

internal class RepositoryBase
{
    private readonly string _connectionString;

    protected RepositoryBase(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    protected IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
}