using FluentValidation;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Validators;

public class StreetSweepingScheduleResponseDtoValidator : AbstractValidator<StreetSweepingScheduleDto>
{
    public StreetSweepingScheduleResponseDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("{PropertyName} is invalid.");

        RuleFor(x => x.StreetId)
            .GreaterThan(0).WithMessage("{PropertyName} is invalid.");

        RuleFor(x => x.StreetSweepingDate)
            .NotEmpty().WithMessage("{PropertyName} is invalid.");
    }

    private static bool DayOfWeekIsValid(StreetSweepingScheduleDto ctx, DayOfWeek dayOfWeekToValidate)
    {
        return ctx.StreetSweepingDate.DayOfWeek == dayOfWeekToValidate;
    }
}