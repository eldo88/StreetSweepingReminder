using FluentResults;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Services;

public interface IReminderService
{
    Task<Result<int>> CreateReminderAsync(CreateReminderDto command, string userId);
    Task<Result<ReminderResponseDto>> GetReminderByIdAsync(int id);
    Task<Result<IEnumerable<ReminderResponseDto>>> GetUserRemindersAsync(string userId);
    //Task<Result> UpdateReminderAsync(UpdateReminderCommand command);
    //Task<Result> DeleteReminderAsync(int id);
    //Task<Result> MarkReminderAsSentAsync(int reminderId);
}