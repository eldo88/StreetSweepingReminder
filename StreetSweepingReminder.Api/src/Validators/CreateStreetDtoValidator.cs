using FluentValidation;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Messages;

namespace StreetSweepingReminder.Api.Validators;

public class CreateStreetDtoValidator : AbstractValidator<CreateStreetDto>
{
    public CreateStreetDtoValidator()
    {
        RuleFor(x => x.StreetName)
            .NotEmpty().WithMessage(ValidationMessages.StreetNameInvalid)
            .MaximumLength(50).WithMessage(ValidationMessages.StreetNameTooLong);
        
        RuleFor(x => x.HouseNumber)
            .GreaterThan(0).WithMessage(ValidationMessages.HouseNumberInvalid)
            .When(x => x.HouseNumber.HasValue && x.HouseNumber.Value != 0);
    }
}