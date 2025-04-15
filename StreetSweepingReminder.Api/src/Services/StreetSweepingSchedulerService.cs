using FluentResults;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;
using StreetSweepingReminder.Api.Repositories;

namespace StreetSweepingReminder.Api.Services;

public interface IStreetSweepingSchedulerService
{
    Task<Result> CreateStreetSweepingSchedule(CreateReminderDto command, int reminderId);
}

public class StreetSweepingSchedulerService : 
    SchedulerServiceBase<CreateReminderDto, StreetSweepingDates, IStreetSweepingDatesRepository, int,StreetSweepingSchedulerService>,
    IStreetSweepingSchedulerService
{
    public StreetSweepingSchedulerService(ILogger<StreetSweepingSchedulerService> logger, IStreetSweepingDatesRepository repository) : base(logger, repository)
    {
    }
    
    public Task<Result> CreateStreetSweepingSchedule(CreateReminderDto command, int reminderId)
    {
        throw new NotImplementedException();
    }

    protected override bool GetIsRecurring(CreateReminderDto command)
    {
        throw new NotImplementedException();
    }

    protected override DateTime GetBaseScheduleDate(CreateReminderDto command)
    {
        throw new NotImplementedException();
    }

    protected override object[] GetRecurringParameters(CreateReminderDto command)
    {
        throw new NotImplementedException();
    }

    protected override StreetSweepingDates MapToEntity(CreateReminderDto command, int parentId)
    {
        throw new NotImplementedException();
    }

    protected override void SetScheduleDateOnEntity(StreetSweepingDates entity, DateTime scheduleDate)
    {
        throw new NotImplementedException();
    }
}