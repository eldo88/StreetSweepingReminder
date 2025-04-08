namespace StreetSweepingReminder.Api.DTOs;

public record AuthResponseDto(
    string Token, 
    string Username, 
    string Email, 
    string UserId, 
    int TokenExpirationInMinutes,
    DateTime TokenExpirationTimestamp);