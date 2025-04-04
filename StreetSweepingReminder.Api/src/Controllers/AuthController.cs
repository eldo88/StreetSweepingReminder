using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var userName = await _userManager.FindByNameAsync(registerDto.Username);
        if (userName is not null)
        {
            return BadRequest("Username already exists.");
        }

        var email = await _userManager.FindByEmailAsync(registerDto.Email);
        if (email is not null)
        {
            return BadRequest("Email already registered.");
        }
        
        var user = new User()
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var token = GenerateJwtToken(user);
        return Ok(new AuthResponseDto(token, user.UserName, user.Email, user.Id));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.Username);
        if (user is not null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            var token = GenerateJwtToken(user);
            return Ok(new AuthResponseDto(
                token, 
                user.UserName ?? throw new InvalidOperationException("Error looking up username."), 
                user.Email ?? throw new InvalidOperationException("Error looking up email."),
                user.Id ?? throw new InvalidOperationException("Error looking up user ID.")));
        }

        return BadRequest("Invalid username or password.");
    }
    
    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
        {
            throw new InvalidOperationException("JWT configuration is missing or invalid.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? throw new InvalidOperationException("Username cannot be null.")), // Subject
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? throw new InvalidOperationException("Email cannot be null")),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Consider token expiration time carefully
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(120), // Set token expiration (e.g., 2 hours)
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}