using FluentResults;
using FluentValidation.Results;
using StreetSweepingReminder.Api.Errors;

namespace StreetSweepingReminder.Api.Extensions;

public static class FluentValidationExtensions
{
    public static Result ToFluentResult(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return Result.Ok();
        }

        var validationErrors = validationResult.Errors
            .Select(error => new ValidationError(error.ErrorMessage, error.PropertyName))
            .Cast<IError>()
            .ToList();

        return Result.Fail(validationErrors);
    }
}