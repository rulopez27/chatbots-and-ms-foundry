using Roboto.Dtos;

namespace Roboto.Service.Services
{
    public interface ICalendarEventService
    {
        Task<CalendarEventDto> CreateEventAsync(CalendarEventCreateDto dto);
        Task<CalendarEventDto?> GetEventByIdAsync(int id);
        Task<CalendarEventDto?> UpdateEventAsync(CalendarEventDto dto);
        Task<bool> DeleteEventAsync(int id);
        Task<IEnumerable<CalendarEventDto>> GetEventsByUserIdAsync(int userId);
        Task<IEnumerable<CalendarEventDto>> GetEventsInDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<CalendarEventDto>> GetCalendarConflictsAsync(int userId, DateTime startDate, DateTime endDate);
    }
}
