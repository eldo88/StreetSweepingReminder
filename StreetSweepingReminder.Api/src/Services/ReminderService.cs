using FluentResults;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Services;

public class ReminderService : IReminderService
{
    public Task<Result<int>> CreateReminderAsync(CreateReminderDto command)
    {
        throw new NotImplementedException();
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