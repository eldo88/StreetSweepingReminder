using FluentResults;
using FluentValidation;
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
    private readonly IValidator<ReminderResponseDto> _reminderResponseValidator;

    public ReminderService(ILogger<ReminderService> logger, IReminderRepository reminderRepository, IValidator<ReminderResponseDto> reminderResponseValidator)
    {
        _logger = logger;
        _reminderRepository = reminderRepository;
        _reminderResponseValidator = reminderResponseValidator;
    }
    
    public async Task<Result<int>> CreateReminderAsync(CreateReminderDto command)
    {
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

    public async Task<Result<ReminderResponseDto>> GetReminderByIdAsync(int id)
    {
        try
        {
            var reminder = await _reminderRepository.GetByIdAsync(id);
            if (reminder is null)
            {
                return Result.Fail<ReminderResponseDto>(new NotFoundError("No reminder found."));
            }

            var dto = reminder.ToReminderResponseDto();
            var validationResult = await _reminderResponseValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return validationResult.ToFluentResult();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving reminder.");
            return Result.Fail<ReminderResponseDto>(
                new ApplicationError($"An unexpected error occurred while retrieving the reminder: {e.Message}"));
        }
        
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<ReminderResponseDto>>> GetUserRemindersAsync(string userId)
    {
        throw new NotImplementedException();
    }
}