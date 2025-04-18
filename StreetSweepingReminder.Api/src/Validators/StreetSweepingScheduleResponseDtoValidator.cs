using FluentValidation;
using StreetSweepingReminder.Api.DTOs;

namespace StreetSweepingReminder.Api.Validators;

public class StreetSweepingScheduleResponseDtoValidator : AbstractValidator<StreetSweepingScheduleResponseDto>
{
    public StreetSweepingScheduleResponseDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid Id.");

        RuleFor(x => x.StreetId)
            .GreaterThan(0).WithMessage("Invalid Street ID.");

        RuleFor(x => x.StreetSweepingDate)
            .NotEmpty().WithMessage("Invalid street sweeping date.");

        RuleFor(x => x.DayOfWeek)
            .NotEmpty().WithMessage("Invalid day of week.");
    }
}