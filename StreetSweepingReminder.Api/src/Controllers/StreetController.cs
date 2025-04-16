using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Services;

namespace StreetSweepingReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class StreetController : ControllerBase
{
    private readonly ILogger<StreetController> _logger;
    private readonly IStreetService _streetService;

    public StreetController(ILogger<StreetController> logger, IStreetService streetService)
    {
        _logger = logger;
        _streetService = streetService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateStreet([FromBody] CreateStreetDto createStreetDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _streetService.CreateStreetAsync(createStreetDto, userId);
        if (result.IsSuccess)
        {
            var newId = result.Value;
            return CreatedAtAction(nameof(GetStreet), newId);
        }
        
        _logger.LogError("Failed to create street. Command: {@Command}, Errors: {Errors}", createStreetDto, result.Errors);

        if (result.HasError<ApplicationError>())
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An internal error occurred while saving the street.");
        }

        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
    }

    [HttpGet]
    [ProducesResponseType(typeof(ReminderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStreet(int id)
    {
        var result = await _streetService.GetStreetByIdAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.HasError<NotFoundError>())
        {
            return Ok();
        }
        
        if (result.HasError<ValidationError>(out var validationErrors))
        {
            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }

            return ValidationProblem(ModelState);
        }
        
        _logger.LogError("Failed to get street {Id}. Errors: {Errors}", id, result.Errors);
        
        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
    }
    

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<StreetResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByPartialStreetName([FromQuery(Name = "query")] string streetName)
    {
        var result = await _streetService.GetStreetsByPartialName(streetName);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.HasError<NotFoundError>())
        {
            return Ok(new List<StreetResponseDto>());
        }
        
        if (result.HasError<ValidationError>(out var validationErrors))
        {
            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }

            return ValidationProblem(ModelState);
        }
        
        _logger.LogError("Failed to get streets. Errors: {Errors}", result.Errors);
        
        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
    }

    [HttpPost("{streetId:int}/schedule")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateStreetSweepingSchedule([FromRoute] int streetId, [FromBody] CreateStreetSweepingScheduleDto dto)
    {
        var result = await _streetService.CreateStreetSweepingSchedule(dto, streetId);
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetStreetSweepingSchedule), new { Id = streetId }, streetId);
        }
        
        if (result.HasError<ValidationError>(out var validationErrors))
        {
            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }

            return ValidationProblem(ModelState);
        }
        
        _logger.LogError("Failed to create street sweeping schedule. Errors: {Errors}", result.Errors);
        
        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
    }

    [HttpGet("{streetId:int}/getSchedule")]
    public async Task<IActionResult> GetStreetSweepingSchedule([FromRoute] int streetId)
    {
        return Ok();
    }
}