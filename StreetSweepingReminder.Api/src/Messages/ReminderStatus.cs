namespace StreetSweepingReminder.Api.Messages;

public struct ReminderStatus
{
    public static string Pending => "Pending";
    public static string Scheduled => "Scheduled";
    public static string Cancelled => "Cancelled";
}