using Microsoft.AspNetCore.Mvc;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Services;

namespace StreetSweepingReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var result = await _authService.ValidateUserRegistration(registerDto);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.HasError<ValidationError>(out var validationErrors))
        {
            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            
            return ValidationProblem(ModelState);
        }

        if (result.HasError<ApplicationError>())
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error creating user account.");
        }
        
        return BadRequest(new AuthErrorDto("Error creating user account."));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var result = await _authService.ValidateUserLogin(loginDto);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.HasError<ValidationError>(out var validationErrors))
        {
            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }

            return ValidationProblem(ModelState);
        }

        if (result.HasError<ApplicationError>())
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }

        return BadRequest(new AuthErrorDto("Invalid username or password."));
    }
}