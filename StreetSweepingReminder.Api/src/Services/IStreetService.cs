using FluentResults;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Services;

public interface IStreetService
{
    Task<Result<int>> CreateStreetAsync(CreateStreetDto command, string userId);
    Task<Result<StreetResponseDto>> GetStreetByIdAsync(int id);
    Task<Result<List<StreetResponseDto>>> GetStreetsByPartialName(string streetName);
    Task<Result> CreateStreetSweepingSchedule(CreateStreetSweepingScheduleDto dto, int streetId);
    Task<Result<List<StreetSweepingScheduleResponseDto>>> GetScheduleByStreetId(int streetId);
}