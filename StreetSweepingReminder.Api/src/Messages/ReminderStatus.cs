namespace StreetSweepingReminder.Api.Messages;

public struct ReminderStatus
{
    public static string Scheduled => "Scheduled";
    public static string Cancelled => "Cancelled";
    public static string SchedulingFailed => "Failed";
}