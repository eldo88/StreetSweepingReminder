using StreetSweepingReminder.Api.DTOs;
using StreetSweepingReminder.Api.Entities;
using StreetSweepingReminder.Api.Utils;

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

    public static ReminderResponseDto ToReminderResponseDto(
        this Reminder source, 
        StreetSweepingScheduleResponseDto streetSweepingSchedule,
        ReminderScheduleResponseDto reminderSchedule)
    {
        return new ReminderResponseDto(
            source.Id,
            source.Title,
            source.Status,
            source.PhoneNumber,
            source.StreetId,
            streetSweepingSchedule,
            reminderSchedule);
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
            WeekOfMonth = DateUtils.GetWeekOfMonth(source.ScheduledDateTimeUtc),
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

    public static StreetSweepingScheduleResponseDto ToStreetSweepingScheduleResponseDto(
        this IEnumerable<StreetSweepingDates> source)
    {
        var schedule = source.ToStreetSweepingScheduleDto();
        
        var day = 0;
        var month = 0;
        var year = 0;
        var streetId = 0;
        
        if (schedule.Count > 0)
        {
            var firstDate = schedule[0];
            day = (int)firstDate.StreetSweepingDate.DayOfWeek;
            month = DateUtils.GetWeekOfMonth(firstDate.StreetSweepingDate);
            year = firstDate.StreetSweepingDate.Year;
            streetId = firstDate.StreetId;
        }

        return new StreetSweepingScheduleResponseDto(day, month, year, streetId, schedule);
    }
    
    
    public static List<StreetSweepingScheduleResponseDto> ToStreetSweepingScheduleResponseDtoList(
        this IEnumerable<StreetSweepingDates> source)
    {   //TODO add login for separating by side of street or day
        var responseDtos = new List<StreetSweepingScheduleResponseDto>();
        var schedule = source.ToStreetSweepingScheduleDto();
        
        var day = 0;
        var month = 0;
        var year = 0;
        var streetId = 0;
        
        if (schedule.Count > 0)
        {
            var firstDate = schedule[0];
            day = (int)firstDate.StreetSweepingDate.DayOfWeek;
            month = DateUtils.GetWeekOfMonth(firstDate.StreetSweepingDate);
            year = firstDate.StreetSweepingDate.Year;
            streetId = firstDate.StreetId;
            var sssrDto = new StreetSweepingScheduleResponseDto(day, month, year, streetId, schedule);
            responseDtos.Add(sssrDto);
        }

        return responseDtos;
    }

    public static List<StreetSweepingScheduleDto> ToStreetSweepingScheduleDto(this IEnumerable<StreetSweepingDates> source)
    {
        List<StreetSweepingScheduleDto> dtos = [];
        foreach (var entity in source)
        {
            var dto = new StreetSweepingScheduleDto(
                entity.Id, 
                entity.StreetId, 
                entity.StreetSweepingDate);
            
            dtos.Add(dto);
        }

        return dtos;
    }

    public static ReminderScheduleResponseDto ToReminderScheduleResponseDto(this IEnumerable<ReminderSchedule> source)
    {
        var schedule = source.ToReminderScheduleDto();
        return new ReminderScheduleResponseDto(schedule);
    }

    public static List<ReminderScheduleDto> ToReminderScheduleDto(this IEnumerable<ReminderSchedule> source)
    {
        List<ReminderScheduleDto> dtos = [];
        foreach (var entity in source)
        {
            var dto = new ReminderScheduleDto(
                entity.Id,
                entity.ReminderId,
                entity.NextNotificationDate,
                entity.DayOfWeek,
                entity.WeekOfMonth,
                entity.StartMonth,
                entity.EndMonth,
                entity.TimeOfDay,
                entity.TimeZone,
                entity.IsRecurring,
                entity.IsActive);
            
            dtos.Add(dto);
        }

        return dtos;
    }
}