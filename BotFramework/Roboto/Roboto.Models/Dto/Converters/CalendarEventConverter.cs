namespace Roboto.Models.Dto
{
    public static class CalendarEventConverter
{
    public static CalendarEventDto ToDto(CalendarEvent entity)
    {
        return new CalendarEventDto
        {
            Id = entity.Id,
            Title = entity.Title,
            StartDateTime = entity.StartDateTime,
            EndDateTime = entity.EndDateTime,
            Duration = entity.Duration,
            IsAllDay = entity.IsAllDay,
            Details = entity.Details,
            BlockCalendar = entity.BlockCalendar,
            UserId = entity.UserId
        };
    }

    public static CalendarEvent ToEntity(CalendarEventCreateDto dto)
    {
        return new CalendarEvent(
            dto.UserId,
            dto.Title,
            dto.StartDateTime,
            dto.Duration,
            dto.IsAllDay,
            dto.BlockCalendar,
            dto.Details
        );
    }
}
}