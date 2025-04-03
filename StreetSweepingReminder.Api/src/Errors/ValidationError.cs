namespace StreetSweepingReminder.Api.Errors;

public class ValidationError : ApplicationError
{
    public string? PropertyName { get; }
    
    public ValidationError(string message, string? propertyName = null) : base(message)
    {
        PropertyName = propertyName;
        
        Metadata.Add("ErrorType", "Validation");
        if (!string.IsNullOrEmpty(propertyName))
        {
            Metadata.Add("PropertyName", propertyName);
        }
    }
}