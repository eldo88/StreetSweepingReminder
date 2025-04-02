namespace StreetSweepingReminder.Api.Errors;

public class ValidationError : ApplicationError
{
    public List<string> ValidationMessages { get; }

    public ValidationError(string message) : base(message)
    {
        ValidationMessages = new List<string>() { message };
    }
    
    public ValidationError(List<string> messages) : base("Validation failed.")
    {
        ValidationMessages = messages;
        Metadata.Add("ValidationErrors", string.Join("; ", messages));
    }
}