using FluentResults;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Extensions;
using StreetSweepingReminder.Api.Repositories;
using StreetSweepingReminder.Api.Services.Utils;

namespace StreetSweepingReminder.Api.Services;

public class ReminderSchedulerService :
    SchedulerServiceBase<CreateReminderDto, ReminderSchedule, IReminderScheduleRepository, int, ReminderSchedulerService>,
    IReminderScheduler
{
    public ReminderSchedulerService(ILogger<ReminderSchedulerService> logger, IReminderScheduleRepository repository) : base(logger, repository)
    {
    }

    public Task<Result> CreateReminderNotificationSchedule(CreateReminderDto command, int reminderId)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (reminderId <= 0)
        {
            _logger.LogWarning("Invalid reminderId provided: {ReminderId}", reminderId);
            return Task.FromResult(Result.Fail(new ValidationError($"{nameof(reminderId)} must be positive.")));
        }
        
        return CreateScheduleAsync(command, reminderId);
    }
    
    protected override bool GetIsRecurring(CreateReminderDto command)
    {
        return command.IsRecurring;
    }

    protected override DateTime GetBaseScheduleDate(CreateReminderDto command)
    {
        return DateTime.SpecifyKind(command.ScheduledDateTimeUtc, DateTimeKind.Local);
    }

    protected override object[] GetRecurringParameters(CreateReminderDto command)
    {
        return [DateUtils.GetWeekOfMonth(command.ScheduledDateTimeUtc)];
    }

    protected override ReminderSchedule MapToEntity(CreateReminderDto command, int parentId)
    {
        return command.ToReminderScheduleEntity(parentId);
    }

    protected override void SetScheduleDateOnEntity(ReminderSchedule entity, DateTime scheduleDate)
    {
        entity.NextNotificationDate = scheduleDate;
    }
    
    protected override void CustomizeEntityBeforeSave(ReminderSchedule entity, CreateReminderDto command)
    {
        // Set properties specific to ReminderSchedule just before saving
        entity.IsActive = true;
    }
}