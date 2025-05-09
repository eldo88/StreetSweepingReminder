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

    [Test]
    public async Task CreateAsync_WhenStreetNameIsNull_ShouldThrowSqliteException()
    {
        // Arrange
        var repository = new StreetRepository(_configuration);

        var streetWithNullStreetName = new Street()
        {
            StreetName = null,
            ZipCode = 35216
        };
        
        // Act & Assert
        var ex = Assert.ThrowsAsync<SqliteException>(
            async () => await repository.CreateAsync(streetWithNullStreetName));

        Assert.Multiple(() =>
        {
            Assert.That(ex.Message, Does.Contain("NOT NULL constraint failed").IgnoreCase.And.Contain("Streets.StreetName"),
                "Exception message should indicate the specific NOT NULL constraint failure.");
            Assert.That(ex.SqliteErrorCode, Is.EqualTo(19)); // SQLITE_CONSTRAINT error code is 19
        });
    }

    [Test]
    public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnCorrectStreetEntity()
    {
        // Arrange
        var repository = new StreetRepository(_configuration);

        var newStreet = new Street()
        {
            StreetName = "Test Rd",
            ZipCode = 80212
        };

        var newId = await repository.CreateAsync(newStreet);
        Assert.That(newId, Is.Positive);
        // Act
        var streetEntity = await repository.GetByIdAsync(newId);
        // Assert
        Assert.That(streetEntity, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(newStreet.StreetName, Is.EqualTo(streetEntity.StreetName));
            Assert.That(newStreet.ZipCode, Is.EqualTo(streetEntity.ZipCode));
        });
    }

    [Test]
    public async Task GetByIdAsync_WhenIdIsInvalid_ShouldReturnNullEntity()
    {
        // Arrange
        var repository = new StreetRepository(_configuration);

        var newStreet = new Street()
        {
            StreetName = "Test Rd",
            ZipCode = 80212
        };

        var newId = await repository.CreateAsync(newStreet);
        Assert.That(newId, Is.Positive);
        var invalidId = newId + 1;
        // Act
        var streetEntity = await repository.GetByIdAsync(invalidId);
        // Assert
        Assert.That(streetEntity, Is.Null);
    }

    [Test]
    public async Task GetByPartialStreetNameAsync_WhenPartialStreetNameIsEmpty_ShouldReturnEmptyEnumerable()
    {
        // Arrange
        var repository = new StreetRepository(_configuration);

        var newStreet = new Street()
        {
            StreetName = "Test Rd",
            ZipCode = 80212
        };

        await repository.CreateAsync(newStreet);
        var searchString = "";
        // Act
        var result = await repository.GetByPartialStreetNameAsync(searchString);
        // Assert
        var enumerable = result.ToList();
        Assert.That(enumerable, Is.Not.Null);
        Assert.That(enumerable.Any, Is.False);
    }

    [Test]
    public async Task GetByPartialStreetNameAsync_WhenPartialStreetNameIsValid_ShouldReturnEnumerableOfStreets()
    {
        // Arrange
        var repository = new StreetRepository(_configuration);

        var newStreet1 = new Street()
        {
            StreetName = "Test Rd",
            ZipCode = 80212
        };
        
        var newStreet2 = new Street()
        {
            StreetName = "Test Way",
            ZipCode = 80211
        };
        
        var newStreet3 = new Street()
        {
            StreetName = "Main Rd",
            ZipCode = 80210
        };

        await repository.CreateAsync(newStreet1);
        await repository.CreateAsync(newStreet2);
        await repository.CreateAsync(newStreet3);

        var partialSearchString = "te";
        // Act
        var result = await repository.GetByPartialStreetNameAsync(partialSearchString);
        // Assert
        var enumerable = result.ToList();
        Assert.That(enumerable, Is.Not.Null);
        Assert.That(enumerable.Any, Is.True);
        Assert.That(enumerable, Has.Count.EqualTo(2));
        var street1 = enumerable[0];
        Assert.That(street1.StreetName, Is.EqualTo("Test Rd"));
        Assert.That(street1.ZipCode, Is.EqualTo(80212));
        var street2 = enumerable[1];
        Assert.That(street2.StreetName, Is.EqualTo("Test Way"));
        Assert.That(street2.ZipCode, Is.EqualTo(80211));
    }
}