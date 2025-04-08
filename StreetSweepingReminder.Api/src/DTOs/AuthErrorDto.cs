namespace StreetSweepingReminder.Api.DTOs;

public record AuthErrorDto(string ErrorMessage, int StatusCode = 400);