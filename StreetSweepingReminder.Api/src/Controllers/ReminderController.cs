using Microsoft.AspNetCore.Mvc;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Services;

namespace StreetSweepingReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReminderController : ControllerBase
{
    private readonly ILogger<ReminderController> _logger;
    private readonly IReminderService _reminderService;
    
    public ReminderController(ILogger<ReminderController> logger, IReminderService reminderService)
    {
        _logger = logger;
        _reminderService = reminderService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateReminder([FromBody] CreateReminderDto createReminderDto)
    {
        var result = await _reminderService.CreateReminderAsync(createReminderDto);
        if (result.IsSuccess)
        {
            var newId = result.Value;
            return CreatedAtAction(nameof(GetReminder), newId);
        }
        
        _logger.LogError("Failed to create reminder. Command: {@Command}, Errors: {Errors}", createReminderDto, result.Errors);

        if (result.HasError<ApplicationError>())
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An internal error occurred while saving the reminder.");
        }
        
        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReminderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReminder(int id)
    {
        var result = await _reminderService.GetReminderByIdAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        if (result.HasError<NotFoundError>())
        {
            // Optionally log result.Errors.First().Message
            return NotFound();
        }
        
        if (result.HasError<ValidationError>(out var validationErrors))
        {
            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }

            return ValidationProblem(ModelState);
        }
        
        _logger.LogError("Failed to get reminder {Id}. Errors: {Errors}", id, result.Errors);
        
        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
    }
}