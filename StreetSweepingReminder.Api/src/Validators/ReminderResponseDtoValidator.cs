using FluentValidation;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Messages;

namespace StreetSweepingReminder.Api.Validators;

public class ReminderResponseDtoValidator : AbstractValidator<ReminderResponseDto>
{
    public ReminderResponseDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Error getting Reminder ID.");
        
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage(ValidationMessages.MessageInvalid)
            .MaximumLength(200).WithMessage(ValidationMessages.MessageTooLong);

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Error with Reminder status.");
        
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ValidationMessages.PhoneNumberRequired)
            .Length(10, 15).WithMessage(ValidationMessages.PhoneNumberInvalidLength)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage(ValidationMessages.PhoneNumberInvalidFormat);
        
        RuleFor(x => x.StreetId)
            .GreaterThan(0).WithMessage(ValidationMessages.StreetNumberInvalid);
    }
}