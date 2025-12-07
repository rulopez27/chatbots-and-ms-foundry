using Microsoft.EntityFrameworkCore;
using Roboto.Models;

namespace Roboto.Repository
{
    public class CalendarEventRepository : ICalendarEventRepository, IDisposable
    {
        private readonly RobotoCalendarSchedulerDbContext _context;

        public CalendarEventRepository(RobotoCalendarSchedulerDbContext context)
        {
            _context = context;
        }

        public async Task<List<CalendarEvent>> GetAllEventsAsync(int userId)
        {
            try
            {
                return await _context.CalendarEvents
                .Where(calendarEvent => calendarEvent.UserId == userId)
                .ToListAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieving all calendar events", ex);
            }
        }

        public async Task<CalendarEvent> GetEventByIdAsync(int id)
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

        public async Task<List<CalendarEvent>> GetCalendarConflictsAsync(int userId, DateTime startDateTime, DateTime endDateTime)
        {
            try
            {
                return await _context.CalendarEvents
                .Where(e => e.UserId == userId && e.StartDateTime < endDateTime && e.EndDateTime > startDateTime)
                .ToListAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error checking for calendar conflicts", ex);
            }
            
        }

        public async Task<List<CalendarEvent>> GetEventsByDateAsync(int userId,DateTime date)
        {
            try
            {
                DateTime startOfDay = date.Date;
                DateTime endOfDay = startOfDay.AddDays(1);

                return await _context.CalendarEvents
                    .Where(e => e.UserId == userId && e.StartDateTime >= startOfDay && e.StartDateTime < endOfDay)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving events by date", ex);
            }
        }

        public Task UpdateEventAsync(CalendarEvent calendarEvent)
        {
            try
            {
                _context.CalendarEvents.Update(calendarEvent);
                return _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating calendar event", ex);
            }
        }

        public Task DeleteEventAsync(int id)
        {
            try
            {
                var calendarEvent = _context.CalendarEvents.Find(id);
                if (calendarEvent != null)
                {
                    _context.CalendarEvents.Remove(calendarEvent);
                    return _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Calendar event with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting calendar event", ex);
            }
        }

        public async Task<List<CalendarEvent>> GetEventsInDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.CalendarEvents
                    .Where(e => e.UserId == userId && e.StartDateTime >= startDate && e.EndDateTime <= endDate)
                    .ToListAsync();
            }
            catch(Exception ex )
            {
                throw new Exception("Error retrieving events in date range", ex);
            }
        }

        public Task<List<CalendarEvent>> GetEventsByUserIdAsync(int userId)
        {
            try
            {
                return _context.CalendarEvents
                    .Where(e => e.UserId == userId)
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                throw new Exception($"Error retrieving events for user with ID {userId}", ex);
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}