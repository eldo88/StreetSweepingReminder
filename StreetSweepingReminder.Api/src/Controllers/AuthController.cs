using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;
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
        var userName = await _authService.FindByNameAsync(registerDto.Username);
        if (userName is not null)
        {
            return BadRequest(new AuthErrorDto("Username already exists."));
        }

        var email = await _authService.FindByEmailAsync(registerDto.Email);
        if (email is not null)
        {
            return BadRequest(new AuthErrorDto("Email already registered."));
        }
        
        var user = new User()
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _authService.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { result.Errors });
        }

        var token = _authService.GenerateJwtToken(user);
        return Ok(new AuthResponseDto(token, user.UserName, user.Email, user.Id));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _authService.FindByNameAsync(loginDto.Username);
        if (user is not null && await _authService.CheckPasswordAsync(user, loginDto.Password))
        {
            var token = _authService.GenerateJwtToken(user);
            return Ok(new AuthResponseDto(
                token, 
                user.UserName?? throw new InvalidOperationException("Error looking up username."), 
                user.Email ?? throw new InvalidOperationException("Error looking up email."),
                user.Id ?? throw new InvalidOperationException("Error looking up user ID.")));
        }

        return BadRequest(new AuthErrorDto("Invalid username or password."));
    }
}