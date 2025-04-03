using FluentResults;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Extensions;
using StreetSweepingReminder.Api.Repositories;

namespace StreetSweepingReminder.Api.Services;

public class StreetService : IStreetService
{
    private readonly ILogger<StreetService> _logger;
    private readonly IStreetRepository _streetRepository;

    public StreetService(IStreetRepository streetRepository, ILogger<StreetService> logger)
    {
        _streetRepository = streetRepository;
        _logger = logger;
    }

    public async Task<Result<int>> CreateStreetAsync(CreateStreetDto command)
    {
        var street = command.ToStreetEntity();

        try
        {
            var newId = await _streetRepository.CreateAsync(street);
            return newId <= 0
                ? Result.Fail<int>(new ApplicationError("Failed to add street to the database."))
                : Result.Ok(newId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating street.");
            return Result.Fail<int>(
                new ApplicationError($"An unexpected error occurred while saving the street, {e.Message}"));
        }
    }

    public Task<Result<StreetResponseDto>> GetStreetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}