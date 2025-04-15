using FluentResults;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Services;

public interface IStreetSweepingSchedulerService
{
    Task<Result> CreateStreetSweepingSchedule(CreateReminderDto command, int reminderId);
}

public class StreetSweepingSchedulerService : IStreetSweepingSchedulerService
{
    public Task<Result> CreateStreetSweepingSchedule(CreateReminderDto command, int reminderId)
    {
        throw new NotImplementedException();
    }
}