using FluentResults;
using FluentValidation;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Extensions;
using StreetSweepingReminder.Api.Repositories;

namespace StreetSweepingReminder.Api.Services;

public class StreetService : IStreetService
{
    private readonly ILogger<StreetService> _logger;
    private readonly IStreetRepository _streetRepository;
    private readonly IValidator<StreetResponseDto> _streetResponseValidator;

    public StreetService(IStreetRepository streetRepository, ILogger<StreetService> logger, IValidator<StreetResponseDto> streetResponseValidator)
    {
        _streetRepository = streetRepository;
        _logger = logger;
        _streetResponseValidator = streetResponseValidator;
    }

    public async Task<Result<int>> CreateStreetAsync(CreateStreetDto command, string userId)
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

    public async Task<Result<StreetResponseDto>> GetStreetByIdAsync(int id)
    {
        try
        {
            var street = await _streetRepository.GetByIdAsync(id);
            if (street is null)
            {
                return Result.Fail<StreetResponseDto>(new NotFoundError("No street found."));
            }

            var dto = street.ToStreetResponseDto();
            var validationResult = await _streetResponseValidator.ValidateAsync(dto);
            
            if (!validationResult.IsValid)
            {
                return validationResult.ToFluentResult();
            }

            return Result.Ok(dto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving street.");
            return Result.Fail<StreetResponseDto>(
                new ApplicationError($"An unexpected error occurred while retrieving the street: {e.Message}"));
        }
    }

    public async Task<Result<List<StreetResponseDto>>> GetAllStreets()
    {
        try
        {
            var result = await _streetRepository.GetAll();
            var streetList = result.ToList();
            if (streetList.Count == 0)
            {
                return Result.Fail<List<StreetResponseDto>>(new NotFoundError("No streets found."));
            }

            var dtoList = streetList.ToListOfStreetResponseDtos();
            foreach (var dto in dtoList)
            {
                var validationResult = await _streetResponseValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToFluentResult();
                }
            }

            return Result.Ok(dtoList);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving street.");
            return Result.Fail<List<StreetResponseDto>>(new ExceptionalError(e));
        }
    }

    public async Task<Result<List<StreetResponseDto>>> GetStreetsByPartialName(string streetName)
    {
        if (string.IsNullOrEmpty(streetName))
        {
            return Result.Fail<List<StreetResponseDto>>(new ValidationError("Street name cannot be empty."));
        }

        try
        {
            var result = await _streetRepository.GetByPartialStreetName(streetName);
            var streetList = result.ToList();
            if (streetList.Count == 0)
            {
                return Result.Fail<List<StreetResponseDto>>(new NotFoundError("No streets found."));
            }

            var dtoList = streetList.ToListOfStreetResponseDtos();
            foreach (var dto in dtoList)
            {
                var validationResult = await _streetResponseValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToFluentResult();
                }
            }

            return Result.Ok(dtoList);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving street.");
            return Result.Fail<List<StreetResponseDto>>(new ExceptionalError(e));
        }
    }
}