using FluentResults;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Repositories;
using StreetSweepingReminder.Api.Utils;

namespace StreetSweepingReminder.Api.Services;

public interface IStreetSweepingSchedulerService
{
    Task<Result> CreateStreetSweepingSchedule(CreateStreetSweepingScheduleDto command, int streetId);
}

public class StreetSweepingSchedulerService : 
    SchedulerServiceBase<CreateStreetSweepingScheduleDto, StreetSweepingDates, IStreetSweepingDatesRepository, int,StreetSweepingSchedulerService>,
    IStreetSweepingSchedulerService
{
    public StreetSweepingSchedulerService(ILogger<StreetSweepingSchedulerService> logger, IStreetSweepingDatesRepository repository) : base(logger, repository)
    {
    }
    
    public Task<Result> CreateStreetSweepingSchedule(CreateStreetSweepingScheduleDto command, int streetId)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (streetId <= 0)
        {
            _logger.LogWarning("Invalid streetId provided: {streetId}", streetId);
            return Task.FromResult(Result.Fail(new ValidationError($"{nameof(streetId)} must be positive.")));
        }

        return CreateScheduleAsync(command, streetId);
    }

    protected override bool GetIsRecurring(CreateStreetSweepingScheduleDto command)
    {
        return true; // always recurring
    }

    protected override DateTime GetBaseScheduleDate(CreateStreetSweepingScheduleDto command)
    {
        var year = command.Year;
        var dayOfWeek = (DayOfWeek)command.DayOfWeek;
        var weekOfMonth = command.WeekOfMonth;
        const int month = 4; // street sweeping starts in April
        var baseStreetSweepingDate = DateUtils.GetNthWeekdayOfMonth(year, month, dayOfWeek, weekOfMonth);
        return DateTime.SpecifyKind(baseStreetSweepingDate, DateTimeKind.Local);
    }

    protected override Task<object[]> GetRecurringParameters(CreateStreetSweepingScheduleDto command)
    {
        object[] parameters = [command.WeekOfMonth];
        return Task.FromResult(parameters);
    }

    protected override StreetSweepingDates MapToEntity(CreateStreetSweepingScheduleDto command, int parentId)
    {   //TODO add mapping for cardinal direction
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