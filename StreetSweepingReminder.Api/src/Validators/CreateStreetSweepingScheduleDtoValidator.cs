using FluentValidation;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Validators;

public class CreateStreetSweepingScheduleDtoValidator : AbstractValidator<CreateStreetSweepingScheduleDto>
{
    public CreateStreetSweepingScheduleDtoValidator()
    {
        RuleFor(x => x.DayOfWeek)
            .GreaterThanOrEqualTo(1).WithMessage("{PropertyName} is invalid.")
            .LessThanOrEqualTo(5).WithMessage("{PropertyName} is invalid.");

        RuleFor(x => x.WeekOfMonth)
            .GreaterThan(0).WithMessage("Week of the month must be greater than 0.")
            .LessThan(5).WithMessage("Week of the month must be less than 5.");

        RuleFor(x => x.Year)
            .GreaterThan(2024).WithMessage("{PropertyName} is invalid.");
    }
}