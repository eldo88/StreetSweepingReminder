using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using StreetSweepingReminder.Api.Entities;
using StreetSweepingReminder.Api.Repositories;

namespace StreetSweepingReminder.Api.Tests.RepositoryTests;

[TestFixture]
public class StreetRepositoryIntegrationTests
{
    private static readonly string DbIdentifier = $"file:memdb-{Guid.NewGuid()}?mode=memory&cache=shared";
    private readonly string _connectionString = $"DataSource={DbIdentifier}";
    private IConfiguration _configuration;
    private SqliteConnection _connection;

    private const string CreateTableSql =
        """
        CREATE TABLE IF NOT EXISTS Streets (
            ID INTEGER PRIMARY KEY AUTOINCREMENT,
            StreetName TEXT NOT NULL,
            ZipCode TEXT NOT NULL, 
            CreatedAt TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%fZ', 'now'))
        ) 
        """;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        var configBuilder = new ConfigurationBuilder();
       
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = _connectionString
        });

        _configuration = configBuilder.Build();

        _connection = new SqliteConnection(_connectionString);
        await _connection.OpenAsync();
        
        await _connection.ExecuteAsync(CreateTableSql);
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _connection.CloseAsync();
        await _connection.DisposeAsync();
    }
    
    [TearDown]
    public async Task TearDown()
    {
        await _connection.ExecuteAsync("DELETE FROM Streets");
    }


    [Test]
    public async Task CreateAsync_WhenValidStreetIsProvided_ShouldInsertRecordAndReturnNewId()
    {
        // Arrange
        var repository = new StreetRepository(_configuration);

        var streetToCreate = new Street()
        {
            StreetName = "Test Rd",
            ZipCode = 35216
        };
        
        // Act
        var newId = await repository.CreateAsync(streetToCreate);
        
        // Assert
        Assert.That(newId, Is.GreaterThan(0), "CreateAsync should return a positive ID.");
        
        const string selectSql = "SELECT * FROM Streets WHERE ID = @id";
        var insertedStreet = await _connection.QuerySingleOrDefaultAsync<Street>(selectSql, new { id = newId });
        
        Assert.That(insertedStreet, Is.Not.Null, "Inserted street should be found in the database.");
        
        Assert.That(streetToCreate.StreetName, Is.EqualTo(insertedStreet.StreetName));
        Assert.That(streetToCreate.ZipCode, Is.EqualTo(insertedStreet.ZipCode));
    }
}