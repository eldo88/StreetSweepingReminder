using FluentValidation;
using StreetSweepingReminder.Api.Constants_Enums;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Validators;

public class CreateReminderDtoValidator : AbstractValidator<CreateReminderDto>
{
    public CreateReminderDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessages.MessageInvalid)
            .MaximumLength(200).WithMessage(ValidationMessages.MessageTooLong);
        
        RuleFor(x => x.ScheduledDateTimeUtc)
            .GreaterThan(DateTime.UtcNow).WithMessage(ValidationMessages.ScheduledDateTimeInvalid);
        
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ValidationMessages.PhoneNumberRequired)
            .Length(10, 15).WithMessage(ValidationMessages.PhoneNumberInvalidLength)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage(ValidationMessages.PhoneNumberInvalidFormat);
        
        RuleFor(x => x.StreetId)
            .GreaterThan(0).WithMessage(ValidationMessages.StreetNumberInvalid);
    }
}