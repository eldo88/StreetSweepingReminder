using FluentResults;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Extensions;
using StreetSweepingReminder.Api.Repositories;
using StreetSweepingReminder.Api.Utils;

namespace StreetSweepingReminder.Api.Services;

public class ReminderSchedulerService :
    SchedulerServiceBase<CreateReminderDto, ReminderSchedule, IReminderScheduleRepository, int, ReminderSchedulerService>,
    IReminderScheduler
{
    private readonly IStreetSweepingDatesRepository _streetSweepingDatesRepository;
    public ReminderSchedulerService(ILogger<ReminderSchedulerService> logger, IReminderScheduleRepository repository, IStreetSweepingDatesRepository streetSweepingDatesRepository) : base(logger, repository)
    {
        _streetSweepingDatesRepository = streetSweepingDatesRepository;
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
        return DateTime.SpecifyKind(command.ScheduledDateTimeUtc, DateTimeKind.Utc);
    }

    protected override async Task<object[]> GetRecurringParameters(CreateReminderDto command)
    {
        var streetSweepingSchedule = await
            _streetSweepingDatesRepository.GetScheduleBuStreetIdAndSideOfStreet(command.StreetId, command.SideOfStreet);

        return [streetSweepingSchedule];
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

    protected override async Task<List<DateTime>> CalculateScheduleDates(CreateReminderDto command)
    {
        var baseDate = GetBaseScheduleDate(command);
        if (!GetIsRecurring(command))
        {
            return [baseDate];
        }

        var parameters = await GetRecurringParameters(command);

        var streetSweepingSchedule = parameters.Length > 0 && parameters[0] is IEnumerable<StreetSweepingDates> schedule
            ? schedule
            : default;

        var offset = 0;
        var dates = new List<DateTime>();
        if (streetSweepingSchedule is not null)
        {
            var sweepingSchedule = streetSweepingSchedule as StreetSweepingDates[] ?? streetSweepingSchedule.ToArray();
            
            foreach (var date in sweepingSchedule)
            {
                dates.Add(date.StreetSweepingDate);
            }
            offset = DateUtils.CalcDateOffset(dates, baseDate);
        }

        return DateUtils.CalcMonthlyRecurringScheduleByOffset(dates, offset);
    }
    
}