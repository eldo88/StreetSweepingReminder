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
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateReminder([FromBody] CreateReminderDto createReminderDto)
    {
        var result = await _reminderService.CreateReminderAsync(createReminderDto);
        if (result.IsSuccess)
        {
            var newId = result.Value;
            return CreatedAtAction(nameof(GetReminder), newId);
        }

        if (result.HasError<ValidationError>(out var validationErrors))
        {
            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }

            return ValidationProblem(ModelState);
        }
        
        _logger.LogError("Failed to create reminder. Command: {@Command}, Errors: {Errors}", createReminderDto, result.Errors);
        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReminderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReminder(int id)
    {
        var result = await _reminderService.GetReminderByIdAsync(id);

        // Check the result and map to IActionResult
        if (result.IsSuccess)
        {
            return Ok(result.Value); // Return the ReminderDto
        }

        // Handle specific errors for correct status codes
        if (result.HasError<NotFoundError>())
        {
            // Optionally log result.Errors.First().Message
            return NotFound(); // 404
        }

        // Log unexpected errors
        _logger.LogError("Failed to get reminder {ReminderId}. Errors: {Errors}", id, result.Errors);

        // Default catch-all for other errors
        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
    }
}