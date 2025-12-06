using Roboto.Models;

namespace Roboto.Repository
{
    public interface ICalendarEventRepository
    {
        Task<List<CalendarEvent>> GetAllEventsAsync();
        Task<CalendarEvent> GetEventByIdAsync(int id);
        Task AddEventAsync(CalendarEvent calendarEvent);
        Task<List<CalendarEvent>> GetCalendarConflictsAsync(DateTime startDateTime, DateTime endDateTime);
        Task<List<CalendarEvent>> GetEventsByDateAsync(DateTime date);
    }
}