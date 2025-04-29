using FluentResults;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Services;

public interface IReminderService
{
    Task<Result<int>> CreateReminderAsync(CreateReminderDto command, string userId); //update name to add recurring
    //Task<Result<int>> CreateSingleReminderAsync(CreateReminderDto command, string userId);
    Task<Result<ReminderResponseDto>> GetReminderByIdAsync(int id);
    Task<Result<List<ReminderResponseDto>>> GetUserRemindersAsync(string userId);
    Task<Result> UpdateReminderAsync(UpdateReminderDto command); 
    Task<Result> DeleteReminderAsync(int id);
    //Task<Result> MarkReminderAsSentAsync(int reminderId);
    Task<Result> CancelIndividualReminderAsync(int id);
}