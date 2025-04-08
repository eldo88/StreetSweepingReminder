using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Extensions;

namespace StreetSweepingReminder.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IValidator<AuthResponseDto> _authResponseValidator;

    public AuthService(UserManager<User> userManager, IConfiguration configuration, IValidator<AuthResponseDto> authResponseValidator)
    {
        _userManager = userManager;
        _configuration = configuration;
        _authResponseValidator = authResponseValidator;
    }

    private static int TokenExpirationInMinutes => 30;
    

    public async Task<Result<AuthResponseDto>> ValidateUserRegistration(RegisterDto registerDto)
    {
        var userByName = await _userManager.FindByNameAsync(registerDto.Username);
        if (userByName is not null)
        {
            return Result.Fail<AuthResponseDto>(new ValidationError("Username already taken."));
        }

        var userByEmail = await _userManager.FindByEmailAsync(registerDto.Email);
        if (userByEmail is not null)
        {
            return Result.Fail<AuthResponseDto>(new ValidationError("Email already registered."));
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
            return Result.Fail<AuthResponseDto>(new ApplicationError("Error creating user's account."));
        }

        var token = GenerateJwtToken(user);
        
        var authResponseDto = new AuthResponseDto(
            token,
            user.UserName,
            user.Email,
            user.Id,
            TokenExpirationInMinutes,
            GetTokenExpirationTimeStamp());

        var validatedResult = await _authResponseValidator.ValidateAsync(authResponseDto);

        if (!validatedResult.IsValid)
        {
            return validatedResult.ToFluentResult();
        }
        
        return Result.Ok(authResponseDto);
    }

    public async Task<Result<AuthResponseDto>> ValidateUserLogin(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.Username);
        if (user is null)
        {
            return Result.Fail<AuthResponseDto>(new ValidationError($"No user found for {loginDto.Username}"));
        }

        if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return Result.Fail<AuthResponseDto>(new ValidationError("Incorrect password"));
        }

        if (user.UserName is null || user.Email is null)
        {
            return Result.Fail<AuthResponseDto>(new ApplicationError("Username or email not found."));
        }
        
        var token = GenerateJwtToken(user);
        
        var authResponseDto = new AuthResponseDto(
            token,
            user.UserName,
            user.Email,
            user.Id,
            TokenExpirationInMinutes,
            GetTokenExpirationTimeStamp());

        var validatedResult = await _authResponseValidator.ValidateAsync(authResponseDto);

        if (!validatedResult.IsValid)
        {
            return validatedResult.ToFluentResult();
        }

        return Result.Ok(authResponseDto);
    }

    public DateTime GetTokenExpirationTimeStamp()
    {
        return DateTime.Now.AddMinutes(TokenExpirationInMinutes);
    }

    public string GenerateJwtToken(User user)
    {
        var jwtKey = GetJwtKey();
        var jwtIssuer = GetJwtIssuer();
        var jwtAudience = GetJwtAudience();

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = GenerateClaims(user);

        var expiration = GetTokenExpirationTimeStamp();

        var token = GenerateJwtSecurityToken(jwtIssuer, jwtAudience, claims, expiration, credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GetJwtKey()
    {
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("Error getting JWT Key from configuration.");
        }

        return jwtKey;
    }

    private string GetJwtIssuer()
    {
        var jwtIssuer = _configuration["Jwt:Issuer"];
        if (string.IsNullOrEmpty(jwtIssuer))
        {
            throw new InvalidOperationException("Error getting JWT Issuer from configuration.");
        }

        return jwtIssuer;
    }

    private string GetJwtAudience()
    {
        var jwtAudience = _configuration["Jwt:Audience"];
        if (string.IsNullOrEmpty(jwtAudience))
        {
            throw new InvalidOperationException("Error getting JWT Audience from configuration");
        }

        return jwtAudience;
    }
    
    private static Claim[] GenerateClaims(User user)
    {
        return
        [
            new Claim(JwtRegisteredClaimNames.Sub,
                user.UserName ?? throw new InvalidOperationException("Username cannot be null.")),
            new Claim(JwtRegisteredClaimNames.Email,
                user.Email ?? throw new InvalidOperationException("Email cannot be null")),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];
    }

    private static JwtSecurityToken GenerateJwtSecurityToken(string jwtIssuer, string jwtAudience, Claim[] claims, DateTime expires, SigningCredentials credentials)
    {
        return new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);
    }
}