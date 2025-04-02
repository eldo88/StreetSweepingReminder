using FluentResults;

namespace StreetSweepingReminder.Api.Errors;

public class ApplicationError : Error
{
    public ApplicationError(string message) : base(message) { }
}