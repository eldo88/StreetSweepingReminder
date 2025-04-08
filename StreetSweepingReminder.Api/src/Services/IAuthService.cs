using FluentResults;
using Microsoft.AspNetCore.Identity;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Services;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> ValidateUserLogin(LoginDto loginDto);
    Task<Result<AuthResponseDto>> ValidateUserRegistration(RegisterDto registerDto);
}