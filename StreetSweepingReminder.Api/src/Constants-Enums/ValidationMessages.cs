namespace StreetSweepingReminder.Api.Constants_Enums;

public readonly struct ValidationMessages
{
    public static string IdInvalid => "Valid ID required.";
    public static string MessageInvalid => "Message must not be empty.";
    public static string MessageTooLong => "Message cannot exceed 200 characters.";
    public static string ScheduledDateTimeInvalid => "Reminder must be scheduled in the future";
    public static string StreetSweepingDateInvalid => "Street sweeping date must not be in the past.";
    public static string PhoneNumberRequired => "Phone number is required.";
    public static string PhoneNumberInvalidLength => "Phone number is an invalid length.";
    public static string PhoneNumberInvalidFormat => "Invalid phone number format.";
    public static string StreetNumberInvalid => "Valid StreetId is required.";
    public static string StreetNameInvalid => "Street name must have a value.";
    public static string StreetNameTooLong => "Street name cannot exceed 50 characters.";
    public static string WeekOfMonthInvalid => "Week of month is invalid";
}