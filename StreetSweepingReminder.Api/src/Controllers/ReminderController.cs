using Microsoft.AspNetCore.Mvc;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReminderController : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(typeof(ReminderResponseDto),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReminder([FromBody] CreateReminderDto createReminderDto)
    {
        if (createReminderDto! is null)
        {
            return BadRequest("Error: no data was sent.");
        }
        try
        {
            return CreatedAtAction("Test", new {}); // TODO finish the response
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}