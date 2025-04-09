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
    private const string CreateTableSql = 
        """
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
        );
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

    [Test]
    public async Task GetByIdAsync_WhenInvalidIdIsProvided_ShouldReturnNullReminder()
    {
        // Arrange
        var repository = new ReminderRepository(_configuration);
        const int invalidId = 9999;
        
        // Act
        var reminderFromDb = await repository.GetByIdAsync(invalidId);
        
        // Assert
        Assert.That(reminderFromDb, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_WhenValidUserIdIsProvided_ShouldReturnIEnumerableOfReminders()
    {
        // Arrange
        var repository = new ReminderRepository(_configuration);

        var remindersToCreate = new List<Reminder>()
        {
            new()
            {
                UserId = "test-user-123",
                Message = "Test sweeping reminder",
                ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(1),
                Status = ReminderStatus.Scheduled, 
                PhoneNumber = "+1234567890",
                StreetId = 101
            },
            new()
            {
                UserId = "test-user-123",
                Message = "Test sweeping reminder, second entry",
                ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(1),
                Status = ReminderStatus.Scheduled, 
                PhoneNumber = "+1234567890",
                StreetId = 101
            },
            new()
            {
                UserId = "test-user-123",
                Message = "Test sweeping reminder, third entry",
                ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(1),
                Status = ReminderStatus.Scheduled, 
                PhoneNumber = "+1234567890",
                StreetId = 101
            }
        };

        foreach (var reminder in remindersToCreate)
        {
            await repository.CreateAsync(reminder);
        }

        const string validUserId = "test-user-123";
        
        // Act
        var remindersFromDb = await repository.GetAllAsync(validUserId);
        
        // Assert
        var fromDb = remindersFromDb.ToList();
        Assert.That(fromDb, Is.Not.Null);
        
        Assert.That(fromDb, Is.Not.Empty);
        Assert.That(fromDb, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task GetAllAsync_WhenInvalidUserIdIsProvided_ShouldReturnEmptyIEnumerableOfReminders()
    {
        // Arrange
        var repository = new ReminderRepository(_configuration);

        var remindersToCreate = new List<Reminder>()
        {
            new()
            {
                UserId = "test-user-123",
                Message = "Test sweeping reminder",
                ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(1),
                Status = ReminderStatus.Scheduled, 
                PhoneNumber = "+1234567890",
                StreetId = 101
            },
            new()
            {
                UserId = "test-user-123",
                Message = "Test sweeping reminder, second entry",
                ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(1),
                Status = ReminderStatus.Scheduled, 
                PhoneNumber = "+1234567890",
                StreetId = 101
            },
            new()
            {
                UserId = "test-user-123",
                Message = "Test sweeping reminder, third entry",
                ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(1),
                Status = ReminderStatus.Scheduled, 
                PhoneNumber = "+1234567890",
                StreetId = 101
            }
        };

        foreach (var reminder in remindersToCreate)
        {
            await repository.CreateAsync(reminder);
        }

        const string validUserId = "test-user";
        
        // Act
        var remindersFromDb = await repository.GetAllAsync(validUserId);
        
        // Assert
        var fromDb = remindersFromDb.ToList();
        Assert.That(fromDb, Is.Not.Null);
        
        Assert.That(fromDb, Is.Empty);
        Assert.That(fromDb, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task UpdateAsync_WhenUpdatedReminderIsProvided_ShouldUpdateTheCorrectRecord()
    {
        // Arrange
        var repository = new ReminderRepository(_configuration); 

        var initialReminder = new Reminder
        {
            UserId = "test-user-update-123",
            Message = "Original Message",
            ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(2).Truncate(TimeSpan.FromSeconds(1)),
            Status = ReminderStatus.Scheduled,
            PhoneNumber = "+1987654321",
            StreetId = 202
        };
        var newId = await repository.CreateAsync(initialReminder);
        Assert.That(newId, Is.GreaterThan(0), "Setup failed: Could not create initial reminder.");
        
        var reminderBeforeUpdate = await repository.GetByIdAsync(newId);
        Assert.That(reminderBeforeUpdate, Is.Not.Null, "Setup failed: Could not retrieve created reminder.");
        
        // Arrange >> Phase 2
        var reminderWithUpdates = new Reminder
        {
            Id = newId, // set to newly created ID so same object is updated
            UserId = reminderBeforeUpdate.UserId,
            ScheduledDateTimeUtc = reminderBeforeUpdate.ScheduledDateTimeUtc,
            PhoneNumber = reminderBeforeUpdate.PhoneNumber,
            StreetId = reminderBeforeUpdate.StreetId,
            CreatedAt = reminderBeforeUpdate.CreatedAt,

            // Updates
            Message = "Updated Message Successfully", 
            Status = ReminderStatus.Scheduled,
            ModifiedAt = DateTime.UtcNow.Truncate(TimeSpan.FromSeconds(1))
        };
        
        // Act
        var updateResult = await repository.UpdateAsync(reminderWithUpdates);

        // Assert
        Assert.That(updateResult, Is.True, "UpdateAsync should return true for a successful update.");
        
        var actualUpdatedReminder = await repository.GetByIdAsync(newId);
        Assert.That(actualUpdatedReminder, Is.Not.Null, "Reminder should still exist after update.");
        
        Assert.Multiple(() =>
        {
        
            Assert.That(actualUpdatedReminder.Message, Is.EqualTo(reminderWithUpdates.Message), "Message should be updated.");
            Assert.That(actualUpdatedReminder.Status, Is.EqualTo(reminderWithUpdates.Status), "Status should be updated.");
            
            Assert.That(actualUpdatedReminder.UserId, Is.EqualTo(reminderWithUpdates.UserId), "UserId should not change.");
            Assert.That(actualUpdatedReminder.PhoneNumber, Is.EqualTo(reminderWithUpdates.PhoneNumber), "PhoneNumber should not change.");
            
            var expectedModifiedAt = reminderWithUpdates.ModifiedAt.Value.Truncate(TimeSpan.FromMilliseconds(1));
            if (actualUpdatedReminder.ModifiedAt != null)
            {
                var actualModifiedAt = actualUpdatedReminder.ModifiedAt.Value.Truncate(TimeSpan.FromMilliseconds(1));
                Assert.That(actualModifiedAt, Is.EqualTo(expectedModifiedAt), "ModifiedAt should match the value passed in the update (truncated).");
            }

            var expectedCreatedAt = reminderBeforeUpdate.CreatedAt.Truncate(TimeSpan.FromMilliseconds(1));
            var actualCreatedAt = actualUpdatedReminder.CreatedAt.Truncate(TimeSpan.FromMilliseconds(1));
            Assert.That(actualCreatedAt, Is.EqualTo(expectedCreatedAt), "CreatedAt should not change during update.");
            
            Assert.That(actualUpdatedReminder.Id, Is.EqualTo(newId), "ID should remain the same.");
        });
    }

    [Test]
    public async Task UpdateAsync_WhenInvalidIdIsProvided_ShouldReturnFalse()
    {
        // Arrange
        var repository = new ReminderRepository(_configuration);

        var invalidReminder = new Reminder()
        {
            Id = -2, //invalid id
            UserId = "test-user-update-123",
            Message = "Original Message",
            ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(2).Truncate(TimeSpan.FromSeconds(1)),
            Status = ReminderStatus.Scheduled,
            PhoneNumber = "+1987654321",
            StreetId = 202
        };
        
        // Act
        var result = await repository.UpdateAsync(invalidReminder);
        
        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_WhenValidIdIsProvided_ShouldReturnTrue()
    {
        // Arrange
        var repository = new ReminderRepository(_configuration);

        var reminderToBeDeleted = new Reminder()
        {
            UserId = "test-user-update-123",
            Message = "Original Message",
            ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(2).Truncate(TimeSpan.FromSeconds(1)),
            Status = ReminderStatus.Scheduled,
            PhoneNumber = "+1987654321",
            StreetId = 202
        };

        var newId = await repository.CreateAsync(reminderToBeDeleted);
        Assert.That(newId, Is.GreaterThan(0), "Setup failed: Could not create initial reminder.");
        
        // Act
        var result = await repository.DeleteAsync(newId);
        
        // Assert
        Assert.That(result, Is.True);
        
        var shouldReturnNull = await repository.GetByIdAsync(newId);
        Assert.That(shouldReturnNull, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WhenInvalidIdIsProvided_ShouldReturnFalse()
    {
        // Arrange
        var repository = new ReminderRepository(_configuration);

        var reminderThatShouldNotGetDeleted = new Reminder()
        {
            UserId = "test-user-update-123",
            Message = "Original Message",
            ScheduledDateTimeUtc = DateTime.UtcNow.AddDays(2).Truncate(TimeSpan.FromSeconds(1)),
            Status = ReminderStatus.Scheduled,
            PhoneNumber = "+1987654321",
            StreetId = 202
        };

        var newId = await repository.CreateAsync(reminderThatShouldNotGetDeleted);
        Assert.That(newId, Is.GreaterThan(0), "Setup failed: Could not create initial reminder.");

        const int inValidId = 9999;
        // Act
        var result = await repository.DeleteAsync(inValidId);
        
        // Assert
        Assert.That(result, Is.False);

        var reminderShouldBeInDb = await repository.GetByIdAsync(newId);
        Assert.That(reminderShouldBeInDb, Is.Not.Null);
    }
}