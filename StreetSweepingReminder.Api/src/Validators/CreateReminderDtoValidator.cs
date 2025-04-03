using FluentValidation;
using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Messages;

namespace StreetSweepingReminder.Api.Validators;

public class CreateReminderDtoValidator : AbstractValidator<CreateReminderDto>
{
    public CreateReminderDtoValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage(ValidationMessages.MessageError)
            .MaximumLength(200)
            .WithMessage(ValidationMessages.MessageLengthError);
        
        RuleFor(x => x.ScheduledDateTimeUtc)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage(ValidationMessages.ScheduledDateTimeInvalidDate);
        
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage(ValidationMessages.PhoneNumberRequired)
            .Length(10, 15)
            .WithMessage(ValidationMessages.PhoneNumberInvalidLength)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage(ValidationMessages.PhoneNumberInvalidFormat);
        
        RuleFor(x => x.StreetId)
            .GreaterThan(0)
            .WithMessage(ValidationMessages.StreetNumberInvalid);
    }
}