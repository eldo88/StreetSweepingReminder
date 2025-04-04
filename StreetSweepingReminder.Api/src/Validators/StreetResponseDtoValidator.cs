using FluentValidation;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Messages;

namespace StreetSweepingReminder.Api.Validators;

public class StreetResponseDtoValidator : AbstractValidator<StreetResponseDto>
{
    public StreetResponseDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(ValidationMessages.IdInvalid);
        
        RuleFor(x => x.StreetName)
            .NotEmpty().WithMessage(ValidationMessages.StreetNameInvalid)
            .MaximumLength(50).WithMessage(ValidationMessages.StreetNameTooLong);

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip code cannot be empty.");
    }
}