namespace StreetSweepingReminder.Api.Errors;

public class NotFoundError : ApplicationError
{
    public NotFoundError(string message) : base(message) {}
    
    public NotFoundError(string entityName, object id)
        : base($"{entityName} with ID '{id}' was not found.")
    {
        Metadata.Add("EntityType", entityName);
        Metadata.Add("EntityId", id);
    }
}