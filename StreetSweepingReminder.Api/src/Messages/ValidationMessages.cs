namespace StreetSweepingReminder.Api.Messages;

public readonly struct ValidationMessages
{
    public static string MessageError => "Message must not be empty.";
    public static string MessageLengthError => "Message cannot exceed 200 characters.";
    public static string ScheduledDateTimeInvalidDate => "Reminder must be scheduled in the future";
    public static string PhoneNumberRequired => "Phone number is required.";
    public static string PhoneNumberInvalidLength => "Phone number seems to be an invalid length.";
    public static string PhoneNumberInvalidFormat => "Invalid phone number format.";
    public static string StreetNumberInvalid => "Valid StreetId is required.";
    public static string StreetNameError => "Street name must have a value.";
    public static string StreetNameTooLongError => "Street name cannot exceed 50 characters.";
    public static string HouseNumberInvalid => "House number must be greater than 0.";
}