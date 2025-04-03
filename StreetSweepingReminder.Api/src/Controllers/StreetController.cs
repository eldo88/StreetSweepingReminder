using Microsoft.AspNetCore.Mvc;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Services;

namespace StreetSweepingReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var result = await _streetService.CreateStreetAsync(createStreetDto);
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
    public async Task<IActionResult> GetStreet(int id)
    {
        var result = await _streetService.GetStreetByIdAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.HasError<NotFoundError>())
        {
            return NotFound("No street found.");
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
}