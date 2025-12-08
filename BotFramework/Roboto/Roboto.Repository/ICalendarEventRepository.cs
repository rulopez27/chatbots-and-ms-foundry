using Roboto.Models;

namespace Roboto.Repository
{
    public interface ICalendarEventRepository
    {
        Task<CalendarEvent> GetEventByIdAsync(int id);
        Task AddEventAsync(CalendarEvent calendarEvent);
        Task<List<CalendarEvent>> GetCalendarConflictsAsync(int userId,DateTime startDateTime, DateTime endDateTime);
        Task<List<CalendarEvent>> GetEventsByDateAsync(int userId, DateTime date);
        Task UpdateEventAsync(CalendarEvent calendarEvent);
        Task DeleteEventAsync(int id);
        Task<List<CalendarEvent>> GetEventsInDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<List<CalendarEvent>> GetEventsByUserIdAsync(int userId);
    }
}