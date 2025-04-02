using FluentResults;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Extensions;
using StreetSweepingReminder.Api.Messages;
using StreetSweepingReminder.Api.Repositories;

namespace StreetSweepingReminder.Api.Services;

public class ReminderService : IReminderService
{
    private readonly ILogger<ReminderService> _logger;
    private readonly IReminderRepository _reminderRepository;

    public ReminderService(ILogger<ReminderService> logger, IReminderRepository reminderRepository)
    {
        _logger = logger;
        _reminderRepository = reminderRepository;
    }
    
    public async Task<Result<int>> CreateReminderAsync(CreateReminderDto command)
    {
        // TODO create validation

        var reminder = command.ToReminderEntity();
        reminder.Status = ReminderStatus.Pending;

        try
        {
            var newId = await _reminderRepository.CreateAsync(reminder);
            return newId <= 0 
                ? Result.Fail<int>(new ApplicationError("Failed to save reminder to the database.")) 
                : Result.Ok(newId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating reminder.");
            return Result.Fail<int>(
                new ApplicationError($"An unexpected error occurred while saving the reminder: {e.Message}"));
        }
    }

    public Task<Result<ReminderResponseDto>> GetReminderByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<ReminderResponseDto>>> GetUserRemindersAsync(string userId)
    {
        throw new NotImplementedException();
    }
}