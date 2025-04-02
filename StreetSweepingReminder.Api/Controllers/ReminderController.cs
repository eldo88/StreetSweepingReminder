using Microsoft.AspNetCore.Mvc;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Controllers;

[ApiController]
public class ReminderController : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> CreateReminder([FromBody] CreateReminderDto createReminderDto)
    {
        if (createReminderDto! is null)
        {
            return BadRequest("Error: no data was sent.");
        }
        try
        {
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}