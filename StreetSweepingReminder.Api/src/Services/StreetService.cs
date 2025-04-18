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
    private readonly IStreetSweepingSchedulerService _schedulerService;
    private readonly IStreetSweepingDatesRepository _streetSweepingDatesRepository;

    public StreetService(IStreetRepository streetRepository, ILogger<StreetService> logger, IValidator<StreetResponseDto> streetResponseValidator, IStreetSweepingSchedulerService schedulerService, IStreetSweepingDatesRepository streetSweepingDatesRepository)
    {
        _streetRepository = streetRepository;
        _logger = logger;
        _streetResponseValidator = streetResponseValidator;
        _schedulerService = schedulerService;
        _streetSweepingDatesRepository = streetSweepingDatesRepository;
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

    public async Task<Result> CreateStreetSweepingSchedule(CreateStreetSweepingScheduleDto dto, int streetId)
    {
        try
        { //TODO add check to see if schedule exists
            var result = await _schedulerService.CreateStreetSweepingSchedule(dto, streetId);
            if (result.IsFailed)
            {
                _logger.LogWarning("Creating street sweeping schedule for street ID: {streetId} failed", streetId);
                return result;
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("Error creating street sweeping schedule for {streetId}", streetId);
            return Result.Fail(new ApplicationError($"An unexpected error occurred creating street sweeping schedule: {e.Message}"));
        }
    }

    public async Task<Result<List<StreetSweepingScheduleResponseDto>>> GetScheduleByStreetId(int streetId)
    {
        if (streetId <= 0)
        {
            return Result.Fail(new ValidationError("Invalid Street ID."));
        }
        
        try
        {
            var result = await _streetSweepingDatesRepository.GetStreetSweepingScheduleByStreetId(streetId);
            var resultList = result.ToSweepingScheduleResponseDtos();
            
            return Result.Ok(resultList);
        }
        catch (Exception e)
        {
            _logger.LogError("Error retrieving street sweeping schedule for {streetId}", streetId);
            return Result.Fail(new ApplicationError($"An unexpected error occurred retrieving street sweeping schedule: {e.Message}"));
        }
    }
}