using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using StreetSweepingReminder.Api.Entities;
using StreetSweepingReminder.Api.Messages;
using StreetSweepingReminder.Api.Repositories;
using StreetSweepingReminder.Api.Tests.Helpers;

namespace StreetSweepingReminder.Api.Tests.RepositoryTests;

[TestFixture]
public class ReminderRepositoryIntegrationTests
{
    private static readonly string DbIdentifier = $"file:memdb-{Guid.NewGuid()}?mode=memory&cache=shared";
    private readonly string _connectionString = $"DataSource={DbIdentifier}";
    private IConfiguration _configuration;
    private SqliteConnection _connection;
    
    // --- Table Schema ---
    private const string CreateTableSql = @"
            CREATE TABLE Reminders (
            ID INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId TEXT NOT NULL,
            Message TEXT NOT NULL,
            ScheduledDateTimeUtc TEXT NOT NULL,
            Status TEXT NOT NULL,
            PhoneNumber TEXT NOT NULL,
            StreetId INTEGER NOT NULL,
            CreatedAt TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%fZ', 'now')),
            ModifiedAt TEXT
            );";
    
    
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
        await _connection.ExecuteAsync("DELETE FROM Reminders");
    }

    [Test]
    public async Task CreateAsync_WhenValidReminderProvided_ShouldInsertRecordAndReturnNewId()
    {
        
        // Arrange
        var repository = new ReminderRepository(_configuration);

        var reminderToCreate = new Reminder
        {
            UserId = "test-user-123",
            Message = "Test sweeping reminder",
            ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(1),
            Status = ReminderStatus.Scheduled, 
            PhoneNumber = "+1234567890",
            StreetId = 101
        };
        
        // Act
        var newId = await repository.CreateAsync(reminderToCreate);

        // Assert
        Assert.That(newId, Is.GreaterThan(0), "CreateAsync should return a positive ID.");
        
        const string selectSql = "SELECT * FROM Reminders WHERE ID = @id";
        var insertedReminder = await _connection.QuerySingleOrDefaultAsync<Reminder>(selectSql, new { id = newId });

        Assert.That(insertedReminder, Is.Not.Null, "Inserted reminder should be found in the database.");

        Assert.Multiple(() =>
        {
            Assert.That(insertedReminder.UserId, Is.EqualTo(reminderToCreate.UserId));
            Assert.That(insertedReminder.Message, Is.EqualTo(reminderToCreate.Message));
            // truncate dates due to precision difference between .net and sqlite
            var expectedDate = reminderToCreate.ScheduledDateTimeUtc.Truncate(TimeSpan.FromMilliseconds(1));
            var actualDate = insertedReminder.ScheduledDateTimeUtc.Truncate(TimeSpan.FromMilliseconds(1));
            Assert.That(actualDate, Is.EqualTo(expectedDate), "ScheduledDateTimeUtc mismatch after truncating to milliseconds.");
            Assert.That(insertedReminder.Status, Is.EqualTo(reminderToCreate.Status));
            Assert.That(insertedReminder.PhoneNumber, Is.EqualTo(reminderToCreate.PhoneNumber));
            Assert.That(insertedReminder.StreetId, Is.EqualTo(reminderToCreate.StreetId));
            Assert.That(insertedReminder.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(insertedReminder.ModifiedAt, Is.Not.EqualTo(default(DateTime)));
        });
    }
    
    [Test]
    public void CreateAsync_WhenUserIdIsNull_ShouldThrowSqliteException()
    {
        // Arrange
        var repository = new ReminderRepository(_configuration);
        var reminderWithNullUser = new Reminder
        {
            UserId = null,
            Message = "Test message",
            ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(1),
            Status = ReminderStatus.Scheduled,
            PhoneNumber = "123456789",
            StreetId = 1
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<SqliteException>(
            async () => await repository.CreateAsync(reminderWithNullUser));

        Assert.Multiple(() =>
        {
            Assert.That(ex.Message, Does.Contain("NOT NULL constraint failed").IgnoreCase.And.Contain("Reminders.UserId"),
                "Exception message should indicate the specific NOT NULL constraint failure.");
            Assert.That(ex.SqliteErrorCode, Is.EqualTo(19)); // SQLITE_CONSTRAINT error code is often 19
        });
    }

    [Test]
    public async Task GetByIdAsync_WhenValidIdIsProvided_ShouldReturnValidReminder()
    {
        // Arrange
        var repository = new ReminderRepository(_configuration);

        var reminderToCreate = new Reminder
        {
            UserId = "test-user-123",
            Message = "Test sweeping reminder",
            ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(1),
            Status = ReminderStatus.Scheduled, 
            PhoneNumber = "+1234567890",
            StreetId = 101
        };
        
        var newId = await repository.CreateAsync(reminderToCreate);
        
        // Act
        var reminderFromDb = await repository.GetByIdAsync(newId);
        
        // Assert
        Assert.That(reminderFromDb, Is.Not.Null);
        
        Assert.Multiple(() =>
        {
            Assert.That(reminderToCreate.UserId, Is.EqualTo(reminderFromDb.UserId));
            Assert.That(reminderToCreate.Message, Is.EqualTo(reminderFromDb.Message));
            // truncate dates due to precision difference between .net and sqlite
            var expectedDate = reminderToCreate.ScheduledDateTimeUtc.Truncate(TimeSpan.FromMilliseconds(1));
            var actualDate = reminderFromDb.ScheduledDateTimeUtc.Truncate(TimeSpan.FromMilliseconds(1));
            Assert.That(actualDate, Is.EqualTo(expectedDate), "ScheduledDateTimeUtc mismatch after truncating to milliseconds.");
            Assert.That(reminderToCreate.Status, Is.EqualTo(reminderFromDb.Status));
            Assert.That(reminderToCreate.PhoneNumber, Is.EqualTo(reminderFromDb.PhoneNumber));
            Assert.That(reminderToCreate.StreetId, Is.EqualTo(reminderFromDb.StreetId));
        });
    }
}