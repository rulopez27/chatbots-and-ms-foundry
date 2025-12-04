using Microsoft.EntityFrameworkCore;
using Roboto.Models;

namespace Roboto.Repository
{
    public class RobotoRepository : IRobotoRepository, IDisposable
    {
        private readonly RobotoDbContext _context;

        public RobotoRepository(RobotoDbContext context)
        {
            _context = context;
        }

        public async Task<List<CalendarEvent>> GetAllEventsAsync()
        {
            try
            {
                return await _context.CalendarEvents.ToListAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieving all calendar events", ex);
            }
        }

        public async Task<CalendarEvent> GetEventByIdAsync(Guid id)
        {
            try
            {
                return await _context.CalendarEvents.SingleAsync(calendarEvent => calendarEvent.Id == id);
            }
            catch(Exception ex)
            {
                throw new Exception($"Error retrieving calendar event with ID {id}", ex);
            }
        }

        public async Task AddEventAsync(CalendarEvent calendarEvent)
        {
            try
            {
                await _context.CalendarEvents.AddAsync(calendarEvent);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error adding new calendar event", ex);
            }
        }

        public async Task<List<CalendarEvent>> GetCalendarConflictsAsync(DateTime startDateTime, DateTime endDateTime)
        {
            try
            {
                return await _context.CalendarEvents
                .Where(e => e.StartDateTime < endDateTime && e.EndDateTime > startDateTime)
                .ToListAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error checking for calendar conflicts", ex);
            }
            
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}