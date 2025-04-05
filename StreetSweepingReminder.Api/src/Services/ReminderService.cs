using FluentResults;
using FluentValidation;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Extensions;
using StreetSweepingReminder.Api.Messages;
using StreetSweepingReminder.Api.Repositories;
using StreetSweepingReminder.Api.Services.Utils;

namespace StreetSweepingReminder.Api.Services;

public class ReminderService : IReminderService
{
    private readonly ILogger<ReminderService> _logger;
    private readonly IReminderRepository _reminderRepository;
    private readonly IValidator<ReminderResponseDto> _reminderResponseValidator;
    private readonly IReminderScheduler _reminderScheduler;

    public ReminderService(ILogger<ReminderService> logger, IReminderRepository reminderRepository, IValidator<ReminderResponseDto> reminderResponseValidator, IReminderScheduler reminderScheduler)
    {
        _logger = logger;
        _reminderRepository = reminderRepository;
        _reminderResponseValidator = reminderResponseValidator;
        _reminderScheduler = reminderScheduler;
    }
    
    public async Task<Result<int>> CreateReminderAsync(CreateReminderDto command, string userId)
    {
        var reminder = command.ToReminderEntity();
        reminder.UserId = userId;
        reminder.Status = ReminderStatus.Scheduled;

        var newId = -1;
        try
        {
            newId = await _reminderRepository.CreateAsync(reminder);
            if (newId <= 0)
            {
                Result.Fail<int>(new ApplicationError("Failed to save reminder to the database."));
            }
             // calculate scheduling logic
             var scheduledTime = reminder.ScheduledDateTimeUtc;
             var scheduleResult = await _reminderScheduler.ScheduleNotificationJobAsync(newId, scheduledTime);
             if (scheduleResult.IsFailed)
             {
                 _logger.LogError("Failed to schedule reminder {ReminderId} after creation. Error: {Error}", 
                     newId, scheduleResult.Errors.FirstOrDefault());
                 reminder.Status = ReminderStatus.SchedulingFailed;
                 await _reminderRepository.UpdateAsync(reminder);
                 
                 return Result.Fail<int>(new ApplicationError(
                         $"Reminder created (ID: {newId}) but scheduling failed. Status set to failed.")
                     .CausedBy(scheduleResult.Errors));
             }
             
             _logger.LogInformation("Reminder {ReminderId} created and scheduled successfully.", newId);
             return Result.Ok(newId);
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

            return Result.Ok(dto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving reminder.");
            return Result.Fail<ReminderResponseDto>(
                new ApplicationError($"An unexpected error occurred while retrieving the reminder: {e.Message}"));
        }
    }

    public Task<Result<IEnumerable<ReminderResponseDto>>> GetUserRemindersAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UpdateReminderAsync(UpdateReminderDto command)
    {
        var reminderEntity = command.ToReminderEntity();
        reminderEntity.ModifiedAt = DateTime.Now;

        try
        {
            var result = await _reminderRepository.UpdateAsync(reminderEntity);
            if (!result)
            {
                _logger.LogError("Error updating reminder.");
                return Result.Fail(new ApplicationError("Error updating reminder."));
            }

            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating reminder.");
            return Result.Fail(new ApplicationError($"An unexpected error occurred while updating reminder: {e.Message}"));
        }
    }

    private List<DateTime?> CalculateReminderSchedule(DateTime initialReminderDateTime, int interval, int weekNumber)
    { //TODO add validation for parameters
        var scheduledDays = new List<DateTime?>();
        var dayOfWeek = initialReminderDateTime.DayOfWeek;
        var month = initialReminderDateTime.Month;

        if (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday && month is > 3 and < 11)
        {   // set first days for reminder since that is provided by client
            scheduledDays.Add(initialReminderDateTime);
            var nextDate = initialReminderDateTime.AddDays(interval);
            scheduledDays.Add(nextDate);
            
            // Calc remaining for the year
            for (var i = month; i < 11; i++)
            {
                var nextScheduledDate = initialReminderDateTime.AddMonths(i);
                var year = nextScheduledDate.Year;
                var nextMonth = nextScheduledDate.Month;
                var dateToAdd = DateUtils.FindNthWeekdayOfMonth(year, nextMonth, dayOfWeek, interval);
                if (dateToAdd is not null)
                {
                    scheduledDays.Add(dateToAdd);
                }
            }
        }

        return scheduledDays;
    }
}