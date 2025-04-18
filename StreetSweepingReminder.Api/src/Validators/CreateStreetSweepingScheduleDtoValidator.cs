using FluentValidation;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Validators;

public class CreateStreetSweepingScheduleDtoValidator : AbstractValidator<CreateStreetSweepingScheduleDto>
{
    public CreateStreetSweepingScheduleDtoValidator()
    {
        RuleFor(x => x.StreetSweepingDate)
            .NotEmpty().WithMessage("Street sweeping date must be provided.");

        RuleFor(x => x.WeekOfMonth)
            .GreaterThan(0).WithMessage("Week of the month must be greater than 0.")
            .LessThan(5).WithMessage("Week of the month must be less than 5.");
    }
}