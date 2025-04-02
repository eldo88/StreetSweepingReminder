using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Extensions;

public static class DtoExtensions
{
    /* Reminder Entity and DTO Mappings */
    public static Reminder ToReminderEntity(this CreateReminderDto source)
    {
        return new Reminder()
        {
            Id = 0,
            UserId = string.Empty,
            Message = source.Message,
            ScheduledDateTimeUtc = source.ScheduledDateTimeUtc,
            Status = string.Empty,
            PhoneNumber = source.PhoneNumber,
            StreetId = source.StreetId,
            CreatedAt = DateTime.Now
        };
    }

    public static ReminderResponseDto ToReminderResponseDto(this Reminder source)
    {
        return new ReminderResponseDto(
            source.Id,
            source.Message,
            source.ScheduledDateTimeUtc,
            source.Status,
            source.PhoneNumber,
            source.StreetId);
    }
    
    /* Street Entity to DTO Mappings */

    public static Street ToStreetEntity(this CreateStreetDto source)
    {
        return new Street()
        {
            Id = 0,
            UserId = string.Empty,
            StreetName = source.StreetName,
            HouseNumber = source.HouseNumber,
            CreatedAt = DateTime.Now
        };
    }

    public static StreetResponseDto ToStreetResponseDto(this Street source)
    {
        return new StreetResponseDto(
            source.Id, 
            source.StreetName, 
            source.HouseNumber);
    }
}