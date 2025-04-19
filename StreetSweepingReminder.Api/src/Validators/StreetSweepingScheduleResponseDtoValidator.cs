using FluentValidation;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Validators;

public class StreetSweepingScheduleResponseDtoValidator : AbstractValidator<StreetSweepingScheduleResponseDto>
{
    public StreetSweepingScheduleResponseDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("{PropertyName} is invalid.");

        RuleFor(x => x.StreetId)
            .GreaterThan(0).WithMessage("{PropertyName} is invalid.");

        RuleFor(x => x.StreetSweepingDate)
            .NotEmpty().WithMessage("{PropertyName} is invalid.");

        RuleFor(x => x.DayOfWeek)
            .NotEmpty().WithMessage("{PropertyName} is invalid.")
            .Must(DayOfWeekIsValid).WithMessage("{PropertyName} must be equal to the date.");
    }

    private static bool DayOfWeekIsValid(StreetSweepingScheduleResponseDto ctx, DayOfWeek dayOfWeekToValidate)
    {
        return ctx.StreetSweepingDate.DayOfWeek == dayOfWeekToValidate;
    }
}