using Roboto.Dtos;

namespace Roboto.Sdk.Services;

/// <summary>
/// Service for calendar event operations
/// </summary>
public interface ICalendarEventService
{
    /// <summary>
    /// Create a new calendar event
    /// </summary>
    Task<CalendarEventDto> CreateEventAsync(CalendarEventCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get calendar event by ID
    /// </summary>
    Task<CalendarEventDto> GetEventByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a calendar event
    /// </summary>
    Task<CalendarEventDto> UpdateEventAsync(CalendarEventDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a calendar event
    /// </summary>
    Task DeleteEventAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all events for a user
    /// </summary>
    Task<IEnumerable<CalendarEventDto>> GetEventsForUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get events in a date range for a user
    /// </summary>
    Task<IEnumerable<CalendarEventDto>> GetEventsInDateRangeAsync(int userId, CalendarEventsRangeDto dateRange, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get calendar conflicts for a user
    /// </summary>
    Task<IEnumerable<CalendarEventDto>> GetCalendarConflictsAsync(int userId, CalendarEventsRangeDto dateRange, CancellationToken cancellationToken = default);
}