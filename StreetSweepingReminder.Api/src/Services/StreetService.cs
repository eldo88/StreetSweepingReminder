using FluentResults;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Services;

public class StreetService : IStreetService
{
    public Task<Result<int>> CreateStreetAsync(CreateStreetDto command)
    {
        throw new NotImplementedException();
    }

    public Task<Result<StreetResponseDto>> GetStreetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}