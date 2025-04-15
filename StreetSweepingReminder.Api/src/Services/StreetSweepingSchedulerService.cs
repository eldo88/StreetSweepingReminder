using FluentResults;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Repositories;

namespace StreetSweepingReminder.Api.Services;

public interface IStreetSweepingSchedulerService
{
    Task<Result> CreateStreetSweepingSchedule(CreateReminderDto command, int streetId);
}

public class StreetSweepingSchedulerService : 
    SchedulerServiceBase<CreateReminderDto, StreetSweepingDates, IStreetSweepingDatesRepository, int,StreetSweepingSchedulerService>,
    IStreetSweepingSchedulerService
{
    public StreetSweepingSchedulerService(ILogger<StreetSweepingSchedulerService> logger, IStreetSweepingDatesRepository repository) : base(logger, repository)
    {
    }
    
    public Task<Result> CreateStreetSweepingSchedule(CreateReminderDto command, int streetId)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (streetId <= 0)
        {
            _logger.LogWarning("Invalid streetId provided: {streetId}", streetId);
            return Task.FromResult(Result.Fail(new ValidationError($"{nameof(streetId)} must be positive.")));
        }

        return CreateScheduleAsync(command, streetId);
    }

    protected override bool GetIsRecurring(CreateReminderDto command)
    {
        return true; // always recurring
    }

    protected override DateTime GetBaseScheduleDate(CreateReminderDto command)
    {
        return DateTime.SpecifyKind(command.StreetSweepingDate, DateTimeKind.Local); // modify here to get the first occurence in April
    }

    protected override object[] GetRecurringParameters(CreateReminderDto command)
    {
        return [command.WeekOfMonth];
    }

    protected override StreetSweepingDates MapToEntity(CreateReminderDto command, int parentId)
    {
        return new StreetSweepingDates
        {
            StreetId = parentId
        };
    }

    protected override void SetScheduleDateOnEntity(StreetSweepingDates entity, DateTime scheduleDate)
    {
        entity.StreetSweepingDate = scheduleDate;
    }
}