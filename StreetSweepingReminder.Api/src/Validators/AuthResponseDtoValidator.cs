using FluentValidation;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Validators;

public class AuthResponseDtoValidator : AbstractValidator<AuthResponseDto>
{
    public AuthResponseDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("JWT Token cannot be empty.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID cannot be empty");

        RuleFor(x => x.TokenExpirationInMinutes)
            .GreaterThan(0).WithMessage("Token expiration cannot be 0.");

        RuleFor(x => x.TokenExpirationTimestamp)
            .GreaterThan(DateTime.Now).WithMessage("Token expiration must be in the future.");
    }
}