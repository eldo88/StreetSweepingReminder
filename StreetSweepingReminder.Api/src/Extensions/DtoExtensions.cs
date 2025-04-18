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
            Title = source.Title,
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
            source.Title,
            source.Status,
            source.PhoneNumber,
            source.StreetId);
    }

    public static IEnumerable<ReminderResponseDto> ToListOfReminderResponseDtos(this IEnumerable<Reminder> source)
    {
        return source.Select(r => r.ToReminderResponseDto()).ToList();
    }

    public static Reminder ToReminderEntity(this UpdateReminderDto source)
    {
        return new Reminder()
        {
            Id = source.Id,
            Title = source.Title,
            Status = source.Status,
            PhoneNumber = source.PhoneNumber,
            StreetId = source.StreetId
        };
    }

    public static UpdateReminderDto ToUpdateReminderDto(this Reminder source)
    {
        return new UpdateReminderDto(
            source.Id,
            source.Title,
            source.Status,
            source.PhoneNumber,
            source.StreetId,
            source.ModifiedAt);
    }
    
    /* Street Entity to DTO Mappings */

    public static Street ToStreetEntity(this CreateStreetDto source)
    {
        return new Street()
        {
            Id = 0,
            StreetName = source.StreetName,
            ZipCode = source.ZipCode
        };
    }

    public static StreetResponseDto ToStreetResponseDto(this Street source)
    {
        return new StreetResponseDto(
            source.Id, 
            source.StreetName, 
            source.ZipCode);
    }

    public static List<StreetResponseDto> ToListOfStreetResponseDtos(this IEnumerable<Street> source)
    {
        return source.Select(s => s.ToStreetResponseDto()).ToList();
    }
    
    /* CreateReminderDto to ReminderSchedule entity Mappings */

    public static ReminderSchedule ToReminderScheduleEntity(this CreateReminderDto source, int reminderId)
    {
        return new ReminderSchedule()
        {
            Message = string.Empty,
            WeekOfMonth = source.WeekOfMonth,
            StartMonth = source.ScheduledDateTimeUtc.Month,
            NextNotificationDate = source.ScheduledDateTimeUtc,
            DayOfWeek = source.ScheduledDateTimeUtc.DayOfWeek,
            TimeOfDay = source.ScheduledDateTimeUtc.ToShortTimeString(),
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Denver").ToString(), // hard coded for now
            EndMonth = 11, //hard coded for now
            ReminderId = reminderId,
            IsRecurring = source.IsRecurring
        };
    }
    
    /* StreetSweepingDates entity to StreetSweepingScheduleResponseDto Mappings */

    public static List<StreetSweepingScheduleResponseDto> ToSweepingScheduleResponseDtos(
        this IEnumerable<StreetSweepingDates> source)
    {
        List<StreetSweepingScheduleResponseDto> dtos = [];
        foreach (var entity in source)
        {
            var dto = new StreetSweepingScheduleResponseDto(entity.Id, entity.StreetId, entity.StreetSweepingDate);
            dtos.Add(dto);
        }

        return dtos;
    }
}