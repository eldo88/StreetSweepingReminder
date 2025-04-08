using FluentResults;
using Microsoft.AspNetCore.Identity;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Services;

public interface IAuthService
{
    Task<User?> FindByNameAsync(string userName);
    Task<User?> FindByEmailAsync(string email);
    Task<IdentityResult> CreateAsync(User user, string password);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<Result<AuthResponseDto>> ValidateUserLogin(LoginDto loginDto);
    DateTime GetTokenExpirationTimeStamp();
    string GenerateJwtToken(User user);
}