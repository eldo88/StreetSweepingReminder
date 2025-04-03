using FluentResults;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Services;

public interface IStreetService
{
    Task<Result<int>> CreateStreetAsync(CreateStreetDto command);
    Task<Result<StreetResponseDto>> GetStreetByIdAsync(int id);
    //Task<Result> UpdateStreetAsync(UpdateStreetDto command);
    //Task<Result> DeleteStreetAsync(int id);
}